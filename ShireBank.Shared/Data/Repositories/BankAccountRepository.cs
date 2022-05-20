using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShireBank.Shared.Data.Models;

namespace ShireBank.Shared.Data.Repositories;

public class BankAccountRepository : IBankAccountRepository, IDisposable
{
    private readonly BankDbContext _context;
    private readonly ILogger<BankAccountRepository> _logger;

    private bool disposed;

    public BankAccountRepository(BankDbContext context, ILogger<BankAccountRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public Task<BankAccount> GetById(uint accountId)
    {
        return _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == accountId);
    }

    public async Task Open(BankAccount account)
    {
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Close(uint accountId)
    {
        try
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null) return false;
            if (account.Balance != 0) return false;

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to close account", ex);
            return false;
        }
    }

    public async Task<float> Withdraw(uint accountId, float amount)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        var account = await _context.Accounts.FindAsync(accountId);

        var maxAmount = Math.Clamp(amount, float.MinValue, account.Balance + account.DebtLimit);
        account.Balance -= maxAmount;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return maxAmount;
    }

    public async Task Deposit(uint accountId, float amount)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        var account = await _context.Accounts.FindAsync(accountId);

        account.Balance += amount;

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
            if (disposing)
                _context.Dispose();
        disposed = true;
    }
}