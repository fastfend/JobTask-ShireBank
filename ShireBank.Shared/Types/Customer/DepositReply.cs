using ShireBank.Shared.Data.Interfaces;

namespace ShireBank.Shared.Protos
{
    public partial class DepositReply : IInspectable
    {
        public string GetSummary()
        {
            return $"Deposited money";
        }
    }
}
