using ShireBank.Shared.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShireBank.Shared.Data.Repositories
{
    public class BankTransactionRepository : IBankTransactionRepository, IDisposable
    {
        private readonly BankDbContext _context;

        public BankTransactionRepository(BankDbContext context)
            => _context = context;

        public async Task<BankTransaction> Create(uint accountId, float amount)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var account = await _context.Accounts.FindAsync(accountId);

            var bankTransaction = new BankTransaction(account, amount);

            _context.Transactions.Add(bankTransaction);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return bankTransaction;
        }

        public async Task<IEnumerable<BankTransaction>> GetAllForAccount(uint accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            return _context.Transactions.Where(transaction => transaction.Account == account).ToList();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
