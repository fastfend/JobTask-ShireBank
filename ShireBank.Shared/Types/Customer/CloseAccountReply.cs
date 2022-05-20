using ShireBank.Shared.Data.Interfaces;

namespace ShireBank.Shared.Protos
{
    public partial class CloseAccountReply : IInspectable
    {
        public string GetSummary()
        {
            return $"Closed account with status {Status}";
        }
    }
}
