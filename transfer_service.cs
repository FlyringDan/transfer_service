using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
var api = app.MapGroup("/api");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


api.MapPost("/addUser", async (
    ApplicationContext db, 
    CreateUserRequest request) =>
{
    User user = new User { giud = Guid.NewGuid().ToString(), balance = request.Balance };
    
    db.Users.Add(user);
    await db.SaveChangesAsync();
    
    var users = await db.Users.ToListAsync();
    return Results.Ok(users);
});

api.MapGet("/users", async (ApplicationContext db) =>
{
    var users = await db.Users.ToListAsync();
    return Results.Ok(users);
});

// Перевод
api.MapPost("/transfers", async (
    ApplicationContext db,
    TransferRequest request
) =>
{
    
})
.WithOpenApi();


// Информаци о переводе 
api.MapGet("/transfers/{id}", () =>
{
    return 2;
})
.WithOpenApi();


// Получение баналанса
api.MapGet("/users/{id}/balance", async (
    string guid,
    ApplicationContext db
) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.giud == guid);

    if (user == null) 
        return Results.NotFound("No such user"); 
    else 
        return Results.Ok(user.balance);
}).WithOpenApi();


app.Run();


record CreateUserRequest(int Balance);
record TransferRequest(string fromUserId, string toUserId, decimal amount, string idempotencyKey);
