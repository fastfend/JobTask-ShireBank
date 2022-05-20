using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ShireBank.Shared.Data;
using ShireBank.Shared.Data.Models;
using ShireBank.Shared.Data.Repositories;
using Xunit;

namespace ShireBank.Shared.Tests;

public class BankAccountRepositoryTests
{
    [Fact]
    public async Task AddingAccountWorks()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var contextOptions = new DbContextOptionsBuilder<BankDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new BankDbContext(contextOptions);
        await context.Database.EnsureCreatedAsync();

        var repository = new BankAccountRepository(context, Mock.Of<ILogger<BankAccountRepository>>());

        var account = await repository.Open("John", "Smith", 100.0m);

        var createdAccount = await repository.GetById(account.AccountId);

        Assert.NotNull(createdAccount);
    }

    [Fact]
    public async Task AddingSameAccountBlocked()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var contextOptions = new DbContextOptionsBuilder<BankDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new BankDbContext(contextOptions);
        await context.Database.EnsureCreatedAsync();

        var repository = new BankAccountRepository(context, Mock.Of<ILogger<BankAccountRepository>>());

        await repository.Open("John", "Smith", 100.0m);
        var second = await repository.Open("John", "Smith", 100.0m);

        Assert.Null(second);
    }
}