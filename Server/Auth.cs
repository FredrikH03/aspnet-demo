using System.Reflection;
using System.Text.Unicode;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Server;

public class Auth
{

    public record loginData(string Email, string Password);

    public static async Task<IResult> Login(loginData user, State state, HttpContext ctx)
    {
        var cmd = state.DB.CreateCommand("select user_id, role from users where email = $1 and pasword = $2");

        cmd.Parameters.AddWithValue(user.Email);
        cmd.Parameters.AddWithValue(user.Password);

        using var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            string id = reader.GetInt32(0).ToString();
            string role = reader.GetString(1);

            await ctx.SignInAsync("sys23m.teacher.aspnetdemo", new ClaimsPrincipal(
                new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.NameIdentifier, id),
                    new Claim(ClaimTypes.Role, role),
                },
                "sys23m.teachers.aspnetdemo"
                )
            ));
            return TypedResults.Ok("signed in");
        }
        else
        {
            return TypedResults.Problem("No such user exists");
        }
    }
}