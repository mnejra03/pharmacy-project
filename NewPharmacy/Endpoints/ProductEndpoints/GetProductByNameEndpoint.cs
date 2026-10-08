using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data;

public static class GetProductByNameEndpoint
{
        public static void MapGetProductByName(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/products/by-name/{name}", async (string name, ApplicationDbContext db) =>
            {
                var products = await db.Products
                                       .Where(p => p.Name.Contains(name))
                                       .ToListAsync();

                return Results.Ok(products);
            })
            .WithName("GetProductByName")
            .AllowAnonymous();
        }
    
}
