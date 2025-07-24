using UserManagementAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global error handler
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\":\"Something went wrong. Please try again later.\"}");
    });
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


var users = new List<User>
{
    new User { Id = 1, FirstName = "Alice", LastName = "Smith", Email = "alice@techhive.com", Department = "HR" },
    new User { Id = 2, FirstName = "Bob", LastName = "Johnson", Email = "bob@techhive.com", Department = "IT" }
};

app.MapGet("/", () => "Welcome to the User Management API!");

// GET all users
app.MapGet("/users", () => users);

// GET user by ID
app.MapGet("/users/{id}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    return user is not null ? Results.Ok(user) : Results.NotFound();
});

// POST new user
app.MapPost("/users", (User newUser) =>
{
    newUser.Id = users.Count > 0 ? users.Max(u => u.Id) + 1 : 1;
    users.Add(newUser);
    return Results.Created($"/users/{newUser.Id}", newUser);
});

// PUT update user
app.MapPut("/users/{id}", (int id, User updatedUser) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user is null) return Results.NotFound();

    user.FirstName = updatedUser.FirstName;
    user.LastName = updatedUser.LastName;
    user.Email = updatedUser.Email;
    user.Department = updatedUser.Department;

    return Results.Ok(user);
});

// DELETE user
app.MapDelete("/users/{id}", (int id) =>
{
    var user = users.FirstOrDefault(u => u.Id == id);
    if (user is null) return Results.NotFound();

    users.Remove(user);
    return Results.Ok();
});


app.Run();
