using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using ShireBank.Shared;

namespace ShireBank.Client
{
    internal class Program
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly object historyPrintLock = new();

        private static void Main(string[] args)
        {
            _logger.Info("Starting tasks...");

            using var channel = GrpcChannel.ForAddress(Constants.BankBaseAddress, new GrpcChannelOptions
            {
                HttpHandler = new SocketsHttpHandler
                {
                    EnableMultipleHttp2Connections = true,
                    ResponseDrainTimeout = TimeSpan.FromSeconds(30),
                    MaxConnectionsPerServer = 1000
                }
            });

            Thread.Sleep(3000);
            Task[] tasks = new Task[3]
            {
                TaskOne(channel),
                TaskTwo(channel),
                TaskThree(channel)
            };

            Task.WaitAll(tasks);
            _logger.Info("Tasks finished. Press key to exit...");
            Console.ReadKey();
        }

        private static async Task TaskOne(GrpcChannel channel)
        {
            var customer = new Customers.CustomersClient(channel);

            Thread.Sleep(TimeSpan.FromSeconds(10));

            _logger.Info("Testing C1 Opening");
            var accountId = await customer.OpenAccountAsync(new OpenAccountRequest()
            {
                FirstName = "Henrietta",
                LastName = "Baggins",
                DebtLimit = 100.0f
            });

            if (!accountId.Account.HasValue)
            {
                throw new Exception("Failed to open account");
            }

            _logger.Info("Testing C1 Deposit 500");
            await customer.DepositAsync(new DepositRequest()
            {
                Account = accountId.Account.Value,
                Amount = 500.0f
            });

            Thread.Sleep(TimeSpan.FromSeconds(10));

            _logger.Info("Testing C1 Deposit 500");
            await customer.DepositAsync(new DepositRequest()
            {
                Account = accountId.Account.Value,
                Amount = 500.0f
            });

            _logger.Info("Testing C1 Deposit 1000");
            await customer.DepositAsync(new DepositRequest()
            {
                Account = accountId.Account.Value,
                Amount = 1000.0f
            });

            _logger.Info("Testing C1 Withdraw 500");
            var withdrawResponse = await customer.WithdrawAsync(new WithdrawRequest()
            {
                Account = accountId.Account.Value,
                Amount = 2000.0f
            });

            if (2000.0f != withdrawResponse.Value)
            {
                throw new Exception("Can't withdraw a valid amount");
            }

            lock (historyPrintLock)
            {
                _logger.Info("=== Customer 1 ===");

                var historyResponse = customer.GetHistory(new HistoryRequest()
                {
                    Account = accountId.Account.Value
                });

                foreach (var line in historyResponse.History.Split("\n"))
                {
                    _logger.Info(line);
                }
            }

            _logger.Info("Testing C1 Close");
            var closeAccountResponse = await customer.CloseAccountAsync(new CloseAccountRequest()
            {
                Account = accountId.Account.Value
            });

            if (!closeAccountResponse.Status)
            {
                throw new Exception("Failed to close account");
            }
        }

        private static async Task TaskTwo(GrpcChannel channel)
        {
            var customer = new Customers.CustomersClient(channel);

            var openAccRequest = new OpenAccountRequest()
            {
                FirstName = "Barbara",
                LastName = "Tuk",
                DebtLimit = 50.0f
            };

            _logger.Info("Testing C2 Opening");
            var accountId = await customer.OpenAccountAsync(openAccRequest);

            if (!accountId.Account.HasValue)
            {
                throw new Exception("Failed to open account");
            }

            _logger.Info("Testing C2 Second opening");
            if ((await customer.OpenAccountAsync(openAccRequest)).Account.HasValue)
            {
                throw new Exception("Opened account for the same name twice!");
            }

            _logger.Info("Testing C2 Withdraw over limit");
            var withdrawResponse = await customer.WithdrawAsync(new WithdrawRequest()
            {
                Account = accountId.Account.Value,
                Amount = 2000.0f
            });

            if (50.0f != withdrawResponse.Value)
            {
                throw new Exception("Can only borrow up to debit limit only");
            }

            Thread.Sleep(TimeSpan.FromSeconds(10));

            _logger.Info("Testing C2 Close with debt");
            var closeAccountRequest = new CloseAccountRequest()
            {
                Account = accountId.Account.Value
            };

            if ((await customer.CloseAccountAsync(closeAccountRequest)).Status)
            {
                throw new Exception("Can't close the account with outstanding debt");
            }

            _logger.Info("Testing C2 Deposit 100");
            await customer.DepositAsync(new DepositRequest()
            {
                Account = accountId.Account.Value,
                Amount = 100.0f
            });

            _logger.Info("Testing C2 Close with money");
            if ((await customer.CloseAccountAsync(closeAccountRequest)).Status)
            {
                throw new Exception("Can't close the account before clearing all funds");
            }

            _logger.Info("Testing C2 Withdraw 50");
            var withdrawResponse2 = await customer.WithdrawAsync(new WithdrawRequest()
            {
                Account = accountId.Account.Value,
                Amount = 50.0f
            });

            if (50.0f != withdrawResponse2.Value)
            {
                throw new Exception("Can't withdraw a valid amount");
            }

            lock (historyPrintLock)
            {
                _logger.Info("=== Customer 2 ===");
                var history = customer.GetHistory(new HistoryRequest() { Account = accountId.Account.Value });
                foreach (var line in history.History.Split("\n"))
                {
                    _logger.Info(line);
                }
            }

            _logger.Info("Testing C2 Close");
            if ((await customer.CloseAccountAsync(closeAccountRequest)).Status == false)
            {
                throw new Exception("Failed to close account");
            }
        }

        private static async Task TaskThree(GrpcChannel channel)
        {
            var customer = new Customers.CustomersClient(channel);

            var openAccRequest = new OpenAccountRequest()
            {
                FirstName = "Gandalf",
                LastName = "Grey",
                DebtLimit = 10000.0f
            };

            _logger.Info("Testing C3 Opening");
            var accountId = await customer.OpenAccountAsync(openAccRequest);
            if (!accountId.Account.HasValue)
            {
                throw new Exception("Failed to open account");
            }

            Thread.Sleep(TimeSpan.FromSeconds(10));

            List<Task> tasks = new List<Task>();

            for (var i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                      var withdrawRequest = new WithdrawRequest()
                      {
                          Account = accountId.Account.Value,
                          Amount = 10.0f
                      };

                      _logger.Info("Testing C3 Withdraw 10");
                      if ((await customer.WithdrawAsync(withdrawRequest)).Value != 10.0f)
                      {
                          throw new Exception("Can't withdraw a valid amount!");
                      }
                    _logger.Info("Testing C3 Withdraw 10 OK");

                }));
            }

            for (var i = 0; i < 100; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var depositRequest = new DepositRequest()
                    {
                        Account = accountId.Account.Value,
                        Amount = 10.0f
                    };

                    _logger.Info("Testing C3 Deposit 10");
                    await customer.DepositAsync(depositRequest);
                    _logger.Info("Testing C3 Deposit 10 OK");
                }));
            }

            Task.WaitAll(tasks.ToArray());

            lock (historyPrintLock)
            {
                _logger.Info("=== Customer 3 ===");
                var history = customer.GetHistory(new HistoryRequest() { Account = accountId.Account.Value });
                foreach (var line in history.History.Split("\n"))
                {
                    _logger.Info(line);
                }
            }

            var closeAccountRequest = new CloseAccountRequest()
            {
                Account = accountId.Account.Value
            };

            _logger.Info("Testing C3 Close");
            if ((await customer.CloseAccountAsync(closeAccountRequest)).Status == false)
            {
                throw new Exception("Failed to close account");
            }
        }
    }
}