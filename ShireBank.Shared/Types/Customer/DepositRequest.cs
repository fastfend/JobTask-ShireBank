using ShireBank.Shared.Data.Interfaces;

namespace ShireBank.Shared.Protos
{
    public partial class DepositRequest : IInspectable
    {
        public string GetSummary()
        {
            return $"Requested deposit of {(decimal)Amount} to account {Account}";
        }
    }
}
