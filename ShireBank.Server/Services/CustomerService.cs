using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShireBank.Shared;
using ShireBank.Shared.Data.Models;
using ShireBank.Shared.Data.Repositories;

namespace ShireBank.Server.Services
{
    public class CustomerService : Customers.CustomersBase
    {
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IBankTransactionRepository _bankTransactionRepository;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(IBankAccountRepository bankAccountRepository, IBankTransactionRepository bankTransactionRepository, ILogger<CustomerService> logger)
        {
            _bankTransactionRepository = bankTransactionRepository;
            _bankAccountRepository = bankAccountRepository;
            _logger = logger;

        }

        public override async Task<OpenAccountReply> OpenAccount(OpenAccountRequest request, ServerCallContext context)
        {
            var account = new BankAccount()
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                DebtLimit = request.DebtLimit
            };

            try
            {
                await _bankAccountRepository.Open(account);
                return new OpenAccountReply()
                {
                    Account = account.AccountId
                };
            }
            catch (DbUpdateConcurrencyException concurrencyEx)
            {
                _logger.LogError("Concurrent account opening detected", concurrencyEx);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unknown error durning account creation", ex);
            }

            return new OpenAccountReply();
        }

        public override async Task<WithdrawReply> Withdraw(WithdrawRequest request, ServerCallContext context)
        {
            if (request.Amount <= 0)
                throw new InvalidOperationException("Can't withdraw zero or negative funds");

            float withdrawnAmount = await _bankAccountRepository.Withdraw(request.Account, request.Amount);
            await _bankTransactionRepository.Create(request.Account, -withdrawnAmount);

            return new WithdrawReply()
            {
                Value = withdrawnAmount
            };
        }

        public override async Task<DepositReply> Deposit(DepositRequest request, ServerCallContext context)
        {
            if (request.Amount <= 0)
                throw new InvalidOperationException("Can't deposit zero or negative funds");

            await _bankAccountRepository.Deposit(request.Account, request.Amount);
            await _bankTransactionRepository.Create(request.Account, request.Amount);

            return new DepositReply();
        }

        public override async Task<HistoryReply> GetHistory(HistoryRequest request, ServerCallContext context)
        {
            var transactions = await _bankTransactionRepository.GetAllForAccount(request.Account);
            var text = string.Join("\n", transactions.Select(a => a.ToString()));

            return new HistoryReply()
            {
                History = text
            };
        }

        public override async Task<CloseAccountReply> CloseAccount(CloseAccountRequest request, ServerCallContext context)
        {
            var closeStatus = await _bankAccountRepository.Close(request.Account);

            return new CloseAccountReply()
            {
                Status = closeStatus
            };
        }
    }
}