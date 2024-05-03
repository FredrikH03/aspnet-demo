// koppla in en databas
// göra post
// kunna få in data via vår path
// error handling och status codes
// ---session hantering / auth---
// login
// skydda routes
// veta vem som gör en request

using System.ComponentModel;
using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.Data;
using Npgsql;
using Npgsql.Replication.PgOutput.Messages;
using Server;

string connection = "Host=localhost;Username=postgres;Password=postgres;Database=aspnet_demo;Port=5455";
await using var db = NpgsqlDataSource.Create(connection);

State state = new(db);

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication().AddCookie("sys23m.teachers.aspnetdemo");
builder.Services.AddAuthorizationBuilder().AddPolicy("admin", policy => policy.RequireRole("admin"));
builder.Services.AddSingleton(state);
var app = builder.Build();

app.MapPost("/login", Auth.Login);
app.MapGet("/login", (ClaimsPrincipal user) => user.FindFirstValue(ClaimTypes.NameIdentifier));

app.MapGet("/users", Users.All);
app.MapGet("/users/{id}", Users.Single);
app.MapPost("/users", Users.Post);
app.MapPost("/message/{sender_id}/{receiver_id}/{message}", SendMessage);

app.Run("http://localhost:3000");

void SendMessage(int sender_id, int receiver_id, string message, State state)
{
    var cmd = state.DB.CreateCommand("insert into messages(sender_id, receiver_id, message_content) values($1,$2,'$3')");
    cmd.Parameters.AddWithValue(sender_id);
    cmd.Parameters.AddWithValue(receiver_id);
    cmd.Parameters.AddWithValue(message);
    cmd.ExecuteNonQuery();
}

public record State(NpgsqlDataSource DB);