using ShireBank.Shared.Data.Interfaces;

namespace ShireBank.Shared.Protos
{
    public partial class WithdrawRequest : IInspectable
    {
        public string GetSummary()
        {
            return $"Requested withdrawal of {(decimal)Amount} from account {Account}";
        }
    }
}
