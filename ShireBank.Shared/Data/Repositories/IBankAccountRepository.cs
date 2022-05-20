using ShireBank.Shared.Data.Models;
using System.Threading.Tasks;

namespace ShireBank.Shared.Data.Repositories
{
    public interface IBankAccountRepository
    {
        /// <summary>
        /// Gets account by it's id
        /// </summary>
        /// <param name="accountId">ID of desired account</param>
        /// <returns></returns>
        Task<BankAccount> GetById(uint accountId);
        
        /// <summary>
        /// Opens new account
        /// </summary>
        /// <param name="account">Account data to be opened</param>
        Task Open(BankAccount account);

        /// <summary>
        /// Withdraws money up to account's debt limit
        /// </summary>
        /// <param name="accountId">ID of account to withdraw from</param>
        /// <param name="amount">Amount to withdraw</param>
        /// <returns>Withdrawn amount of money</returns>
        Task<float> Withdraw(uint accountId, float amount);

        /// <summary>
        /// Deposits money to desired account
        /// </summary>
        /// <param name="accountId">ID of account to deposit to</param>
        /// <param name="amount">Amount to deposit</param
        Task Deposit(uint accountId, float amount);

        /// <summary>
        /// Closes specifed account and deletes all transactions connected with it
        /// </summary>
        /// <param name="accountId">ID of account to be closed</param>
        /// <returns>Success state of the operation</returns>
        Task<bool> Close(uint accountId);
    }
}
