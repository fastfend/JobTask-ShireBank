using System.Collections.Generic;

namespace ShireBank.Shared.Data.Models;

public class BankAccount
{
    public uint AccountId { get; private set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public float DebtLimit { get; set; }

    public float Balance { get; set; }

    public byte[] Timestamp { get; private set; }

    public List<BankTransaction> Transactions { get; private set; }
}