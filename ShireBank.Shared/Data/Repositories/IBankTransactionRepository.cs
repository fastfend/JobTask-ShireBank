using ShireBank.Shared.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShireBank.Shared.Data.Repositories
{
    public interface IBankTransactionRepository
    {
        /// <summary>
        /// Creates transaction for account
        /// </summary>
        /// <param name="accountId">ID of account connected with transaction</param>
        /// <param name="amount">Value of transaction</param>
        /// <returns>Transaction details</returns>
        Task<BankTransaction> Create(uint accountId, float amount);

        /// <summary>
        /// Gets all transaction for desired account
        /// </summary>
        /// <param name="accountId">ID of desired account</param>
        /// <returns>Transactions of desired account</returns>
        Task<IEnumerable<BankTransaction>> GetAllForAccount(uint accountId);
    }
}
