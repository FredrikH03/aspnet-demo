// koppla in en databas
// göra post
// kunna få in data via vår path
// error handling och status codes
// ---session hantering / auth---
// login
// skydda routes
// veta vem som gör en request

using Npgsql;

string connection = "Host=localhost;Username=postgres;Password=postgres;Database=aspnet_demo;Port=5455";
await using var db = NpgsqlDataSource.Create(connection);

State state = new(db, "Thomas");

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(state);
var app = builder.Build();

app.MapGet("/users", GetUsers);
app.MapPost("/users", PostUser);

app.Run("http://localhost:3000");

void PostUser(State state){
    state.name[0] = 'A';
}

string GetUsers(State state){

    return state.name;
}

record State(NpgsqlDataSource DB);