using System;

namespace ShireBank.Shared.Data.Models;

/// <summary>
/// Entity representing single transaction in Shire Bank
/// </summary>
public class BankTransaction
{
    public BankTransaction(BankAccount account, decimal value) : this(value)
    {
        ArgumentNullException.ThrowIfNull(account);
        Account = account;
    }

    private BankTransaction(decimal value)
    {
        Value = value;
    }

    public Guid TransactionId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public BankAccount Account { get; }

    public decimal Value { get; }

    public byte[] Timestamp { get; private set; }

    public override string ToString()
    {
        return $"Transaction {TransactionId} at {CreatedAt} for {Value}";
    }
}