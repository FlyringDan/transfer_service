using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Transfer> Transfers => Set<Transfer>();
}

public class User
{
    public int Id { get; set; }
    public string? giud { get; set; }
    [Range(0, int.MaxValue)]
    public decimal balance { get; set; }
}


public class Transfer
{
    public int Id { get; set; }
    public string FromUserId { get; set; } = string.Empty;
    public string ToUserId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string IdempotencyKey { get; set; } = string.Empty;
}
