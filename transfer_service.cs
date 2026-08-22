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
    if (request.balance < 0)
        return Results.BadRequest("Баланс не может быть отрицательным");

    User user = new User { giud = Guid.NewGuid().ToString(), balance = request.balance };
    
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
    if (request.amount <= 0)
        return Results.BadRequest("Сумма перевода должна быть положительной");

    var fromUserId = await db.Users.FirstOrDefaultAsync(u => u.giud == request.fromUserId);
    var toUserId = await db.Users.FirstOrDefaultAsync(u => u.giud == request.toUserId);

    if (fromUserId == null) return Results.NotFound("Пользователя отправителя не существует");
    if (toUserId == null) return Results.NotFound("Пользователя получателя не существует");
    if (request.fromUserId == request.toUserId) return Results.BadRequest("Нельзя переводить деньги самому себе");
    
    // Добавить провернку на idempotencyKey
    if (fromUserId.balance < request.amount)
        return Results.BadRequest("У пользователя не хватает денег для переревода");
    else {
        fromUserId.balance -= request.amount;
        toUserId.balance += request.amount;

        var transfer = new Transfer
        {
            FromUserId = request.fromUserId,
            ToUserId = request.toUserId,
            Amount = request.amount,
            IdempotencyKey = request.idempotencyKey
        };
        db.Transfers.Add(transfer);

        await db.SaveChangesAsync();
        return Results.Ok("Перевод успешно выполнен");
    }
    
})
.WithOpenApi();


// Информаци о переводе 
api.MapGet("/transfers/{id}", async(
    int id,
    ApplicationContext db
) =>
{
    var transfer = await db.Transfers.FindAsync(id);
    if (transfer == null) return Results.NotFound("нет такого перевода");
    else 
        return Results.Ok(transfer);
})
.WithOpenApi();


// Получение баналанса
api.MapGet("/users/{id}/balance", async (
    int id,
    ApplicationContext db
) =>
{
    var user = await db.Users.FindAsync(id);

    if (user == null) 
        return Results.NotFound("No such user"); 
    else 
        return Results.Ok(user.balance);
}).WithOpenApi();


app.Run();


record CreateUserRequest(int balance);
record TransferRequest(string fromUserId, string toUserId, decimal amount, string idempotencyKey);
