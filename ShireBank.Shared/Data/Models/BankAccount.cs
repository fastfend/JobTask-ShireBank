using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShireBank.Shared.Data.Models
{
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
}
