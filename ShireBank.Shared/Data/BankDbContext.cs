using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ShireBank.Shared.Data.Models;
using System;
using System.Linq;

namespace ShireBank.Shared.Data
{
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
                .IsRowVersion()
                .HasConversion(new SqliteTimestampConverter())
                .HasColumnType("BLOB")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            //modelBuilder.Entity<BankAccount>()
            //    .Property(a => a.Version)
            //    .HasDefaultValue(0)
            //    .IsConcurrencyToken()
            //    .ValueGeneratedOnAddOrUpdate();

            modelBuilder.Entity<BankAccount>()
                .HasAlternateKey(a => new { a.FirstName, a.LastName });

            // Transaction
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
                .HasDefaultValue(DateTime.Now)
                .IsRequired();

            modelBuilder.Entity<BankTransaction>()
                .Property(t => t.Timestamp)
                .IsRowVersion()
                .HasConversion(new SqliteTimestampConverter())
                .HasColumnType("BLOB")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
