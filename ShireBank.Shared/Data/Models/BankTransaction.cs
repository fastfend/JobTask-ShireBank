using System;

namespace ShireBank.Shared.Data.Models;

public class BankTransaction
{
    public BankTransaction(BankAccount account, float value) : this(value)
    {
        ArgumentNullException.ThrowIfNull(account);
        Account = account;
    }

    private BankTransaction(float value)
    {
        Value = value;
    }

    public Guid TransactionId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public BankAccount Account { get; }

    public float Value { get; }

    public byte[] Timestamp { get; private set; }

    public override string ToString()
    {
        return $"Transaction {TransactionId} at {CreatedAt} for {Value}";
    }
}