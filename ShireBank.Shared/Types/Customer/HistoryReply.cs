using ShireBank.Shared.Data.Interfaces;

namespace ShireBank.Shared.Protos
{
    public partial class HistoryReply : IInspectable
    {
        public string GetSummary()
        {
            return $"Shown transaction history {History}";
        }
    }
}
