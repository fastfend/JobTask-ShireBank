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
    public async void AddingAccountWorks()
    {
        var _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        // These options will be used by the context instances in this test suite, including the connection opened above.
        var _contextOptions = new DbContextOptionsBuilder<BankDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new BankDbContext(_contextOptions);

        var repository = new BankAccountRepository(context, Mock.Of<ILogger<BankAccountRepository>>());

        var account = new BankAccount
        {
            FirstName = "John",
            LastName = "Smith",
            DebtLimit = 100.0f
        };

        await repository.Open(account);

        var createdAccount = await repository.GetById(account.AccountId);

        Assert.NotNull(createdAccount);
    }

    [Fact]
    public async void AddingSameAccountBl()
    {
        var _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        // These options will be used by the context instances in this test suite, including the connection opened above.
        var _contextOptions = new DbContextOptionsBuilder<BankDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new BankDbContext(_contextOptions);

        var repository = new BankAccountRepository(context, Mock.Of<ILogger<BankAccountRepository>>());

        var account = new BankAccount
        {
            FirstName = "John",
            LastName = "Smith",
            DebtLimit = 100.0f
        };

        await repository.Open(account);

        var createdAccount = await repository.GetById(account.AccountId);

        await Assert.ThrowsAsync<DbUpdateException>(async () => { await repository.Open(account); });
    }
}