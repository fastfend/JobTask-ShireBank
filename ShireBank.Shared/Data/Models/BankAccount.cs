using System.Collections.Generic;

namespace ShireBank.Shared.Data.Models;

/// <summary>
/// Entity representing account in Shire Bank
/// </summary>
public class BankAccount
{
    public uint AccountId { get; private set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public decimal DebtLimit { get; set; }

    public decimal Balance { get; set; }

    public byte[] Timestamp { get; private set; }

    public List<BankTransaction> Transactions { get; private set; }
}