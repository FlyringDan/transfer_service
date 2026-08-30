using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace TransferService.Tests;

public class TransferApiTests
{
    private readonly HttpClient client = new()
    {
        BaseAddress = new Uri("http://localhost:5206")
    };

    [Fact]
    public async Task create_user()
    {
        var response = await client.PostAsJsonAsync(
            "/api/addUser",
            new CreateUserRequest(100));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task create_user_with_negative_balance()
    {
        var response = await client.PostAsJsonAsync(
            "/api/addUser",
            new CreateUserRequest(-1));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task transfer_zero_amount()
    {
        var response = await client.PostAsJsonAsync(
            "/api/transfers",
            CreateTransferRequest(0, "invalid-amount"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task transfer_without_reciever()
    {
        var sender = await CreateUser(100);

        var response = await client.PostAsJsonAsync(
            "/api/transfers",
            new TransferRequest(sender.Guid, Guid.NewGuid().ToString(), 10, UniqueKey()));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task transfer_to_self()
    {
        var user = await CreateUser(100);

        var response = await client.PostAsJsonAsync(
            "/api/transfers",
            new TransferRequest(user.Guid, user.Guid, 10, UniqueKey()));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task transfer_more_money_than_sender_balance()
    {
        var sender = await CreateUser(100);
        var recipient = await CreateUser(0);

        var response = await client.PostAsJsonAsync(
            "/api/transfers",
            new TransferRequest(sender.Guid, recipient.Guid, 101, UniqueKey()));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task successful_transfer()
    {
        var sender = await CreateUser(100);
        var reciever = await CreateUser(0);
        var request = new TransferRequest(sender.Guid, reciever.Guid, 30, UniqueKey());

        var transferResponse = await client.PostAsJsonAsync("/api/transfers", request);
        transferResponse.EnsureSuccessStatusCode();

        var senderBalance = await GetBalance(sender.Id);
        var recipientBalance = await GetBalance(reciever.Id);

        Assert.Equal(70, senderBalance);
        Assert.Equal(30, recipientBalance);
    }

    [Fact]
    public async Task transfer_with_same_idempotency_key()
    {
        var sender = await CreateUser(100);
        var reciever = await CreateUser(0);
        var request = new TransferRequest(sender.Guid, reciever.Guid, 30, UniqueKey());

        var firstResponse = await client.PostAsJsonAsync("/api/transfers", request);
        var secondResponse = await client.PostAsJsonAsync("/api/transfers", request);

        firstResponse.EnsureSuccessStatusCode();
        secondResponse.EnsureSuccessStatusCode();

        Assert.Equal(70, await GetBalance(sender.Id));
        Assert.Equal(30, await GetBalance(reciever.Id));
    }



    private async Task<UserResponse> CreateUser(decimal balance)
    {
        var response = await client.PostAsJsonAsync(
            "/api/addUser",
            new CreateUserRequest(balance));

        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<UserResponse>();

        Assert.NotNull(user);
        return user;
    }

    private async Task<decimal> GetBalance(int userId)
    {
        var response = await client.GetAsync($"/api/users/{userId}/balance");

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<decimal>();
    }

    private async Task<HttpResponseMessage> SendTransfer(
        string senderGuid,
        string recipientGuid,
        decimal amount,
        string idempotencyKey)
    {
        return await client.PostAsJsonAsync(
            "/api/transfers",
            new TransferRequest(senderGuid, recipientGuid, amount, idempotencyKey));
    }

    private static TransferRequest CreateTransferRequest(
        decimal amount,
        string idempotencyKey
        ) => new
        (
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            amount,
            idempotencyKey
        );

    private static string UniqueKey() => Guid.NewGuid().ToString();

    private sealed record CreateUserRequest(decimal balance);

    private sealed record TransferRequest(
        string fromUserId,
        string toUserId,
        decimal amount,
        string idempotencyKey);

    private sealed record UserResponse(
        int Id,
        [property: JsonPropertyName("giud")] string Guid,
        [property: JsonPropertyName("balance")] decimal Balance
        );

}