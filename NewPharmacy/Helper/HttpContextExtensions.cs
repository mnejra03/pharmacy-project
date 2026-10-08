using Microsoft.AspNetCore.Http;
using NewPharmacy.Data.Models.Auth;
using System.Text.Json;

public static class HttpContextExtensions
{
    private const string UserKey = "loggedInUser";

    public static MyAppUser? GetLoggedInUser(this HttpContext context)
    {
        var json = context.Session.GetString(UserKey);
        return json == null ? null : JsonSerializer.Deserialize<MyAppUser>(json);
    }

    public static void SetLoggedInUser(this HttpContext context, MyAppUser user)
    {
        var json = JsonSerializer.Serialize(user);
        context.Session.SetString(UserKey, json);
    }

    public static void RemoveLoggedInUser(this HttpContext context)
    {
        context.Session.Remove(UserKey);
    }
}
