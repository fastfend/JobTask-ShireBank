using Microsoft.EntityFrameworkCore;
using ShireBank.Shared.Data.Models;
using ShireBank.Shared.Utils.Sqlite;

namespace ShireBank.Shared.Data;

public class BankDbContext : DbContext
{
    public BankDbContext(DbContextOptions<BankDbContext> options) : base(options)
    {
    }

    public DbSet<BankTransaction> Transactions { get; set; }
    public DbSet<BankAccount> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Account
        modelBuilder.Entity<BankAccount>()
            .ToTable("Accounts");

        modelBuilder.Entity<BankAccount>()
            .HasKey(a => a.AccountId);

        modelBuilder.Entity<BankAccount>()
            .Property(a => a.FirstName)
            .IsRequired();

        modelBuilder.Entity<BankAccount>()
            .Property(a => a.LastName)
            .IsRequired();

        modelBuilder.Entity<BankAccount>()
            .Property(a => a.DebtLimit)
            .IsRequired();

        modelBuilder.Entity<BankAccount>()
            .Property(a => a.Balance)
            .HasDefaultValue(0f)
            .IsRequired();

        modelBuilder.Entity<BankAccount>()
            .Property(a => a.Timestamp)
            .IsRowVersion();

        modelBuilder.Entity<BankAccount>()
            .HasAlternateKey(a => new { a.FirstName, a.LastName });

        // Transaction
        modelBuilder.Entity<BankTransaction>()
            .ToTable("Transactions");

        modelBuilder.Entity<BankTransaction>()
            .HasKey(t => t.TransactionId);

        modelBuilder.Entity<BankTransaction>()
            .HasOne(t => t.Account)
            .WithMany(t => t.Transactions)
            .IsRequired();

        modelBuilder.Entity<BankTransaction>()
            .Property(t => t.Value)
            .IsRequired();

        modelBuilder.Entity<BankTransaction>()
            .Property(t => t.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        modelBuilder.Entity<BankTransaction>()
            .Property(t => t.Timestamp)
            .IsRowVersion();

        modelBuilder.AddSqliteCompatibility(this);
    }
}