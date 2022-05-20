using ShireBank.Shared.Data.Interfaces;

namespace ShireBank.Shared.Protos
{
    public partial class HistoryRequest : IInspectable
    {
        public string GetSummary()
        {
            return $"Requested transaction history for {Account}";
        }
    }
}
