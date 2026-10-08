using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NewPharmacy.Data;
using NewPharmacy.Data.Models.Auth;
using NewPharmacy.Helper;
using NewPharmacy.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
var configuration = builder.Configuration;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("db1")));

builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<MyAuthService>();
builder.Services.AddScoped<AzureBlobService>();
builder.Services.AddScoped<OrderPricingService>();
builder.Services.AddSignalR();
builder.Services.Configure<StripeSettings>(configuration.GetSection("Stripe"));

var stripeSettings = configuration.GetSection("Stripe").Get<StripeSettings>();
if (!string.IsNullOrWhiteSpace(stripeSettings?.SecretKey))
{
    Stripe.StripeConfiguration.ApiKey = stripeSettings.SecretKey;
}

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Unesite samo JWT token ili vrijednost u formatu: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = configuration.GetSection("JwtSettings");
    var secretKey = jwtSettings["SecretKey"];

    if (string.IsNullOrEmpty(secretKey))
    {
        throw new InvalidOperationException("JWT SecretKey is not configured in appsettings.json");
    }

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger("JwtAuthentication");
            logger.LogWarning(context.Exception, "Token validation failed.");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            return ValidateTokenAgainstDatabaseAsync(context);
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

await EnsureBootstrapAdminAsync(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAngularApp");
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NewPharmacy.SignalR.ChatHub>("/chatHub");

app.Run();

static async Task ValidateTokenAgainstDatabaseAsync(TokenValidatedContext context)
{
    var authService = context.HttpContext.RequestServices.GetRequiredService<MyAuthService>();
    var logger = context.HttpContext.RequestServices
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("JwtAuthentication");

    var token = context.Request.Headers.Authorization.ToString();
    if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
    {
        token = token["Bearer ".Length..].Trim();
    }

    if (string.IsNullOrWhiteSpace(token))
    {
        token = context.Request.Query["access_token"].ToString();
    }

    var isActiveToken = await authService.ValidateTokenAsync(token, context.HttpContext.RequestAborted);
    if (!isActiveToken)
    {
        logger.LogWarning("Rejected JWT because it is no longer active in the database.");
        context.Fail("Token is no longer active.");
    }
}

static async Task EnsureBootstrapAdminAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var configuration = services.GetRequiredService<IConfiguration>();
    var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("BootstrapAdmin");
    var db = services.GetRequiredService<ApplicationDbContext>();

    var adminExists = await db.MyAppUsers.AnyAsync(u => u.IsAdmin && !u.IsDeleted);
    if (adminExists)
    {
        return;
    }

    var username = configuration["BootstrapAdmin:Username"];
    var password = configuration["BootstrapAdmin:Password"];
    var firstName = configuration["BootstrapAdmin:FirstName"] ?? "Initial";
    var lastName = configuration["BootstrapAdmin:LastName"] ?? "Admin";
    var email = configuration["BootstrapAdmin:Email"] ?? "admin@bootstrap.local";
    var phoneNumber = configuration["BootstrapAdmin:PhoneNumber"] ?? string.Empty;

    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
    {
        logger.LogWarning(
            "No bootstrap admin was created because no admin exists in the database and BootstrapAdmin credentials were not provided via environment configuration.");
        return;
    }

    var usernameTaken = await db.MyAppUsers.AnyAsync(u => u.Username == username);
    if (usernameTaken)
    {
        logger.LogWarning(
            "Bootstrap admin was not created because username '{Username}' already exists.", username);
        return;
    }

    var bootstrapAdmin = new MyAppUser
    {
        Username = username,
        Password = FileHelper.HashPassword(password),
        FirstName = firstName,
        LastName = lastName,
        Email = email,
        PhoneNumber = phoneNumber,
        IsAdmin = true,
        IsPharmacist = false,
        IsCustomer = false,
        IsDeleted = false
    };

    db.MyAppUsers.Add(bootstrapAdmin);
    await db.SaveChangesAsync();

    logger.LogWarning(
        "Bootstrap admin '{Username}' was created because no admin existed in the database.", username);
}
