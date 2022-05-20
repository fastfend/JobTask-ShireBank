using ShireBank.Shared.Data.Interfaces;

namespace ShireBank.Shared.Protos
{
    public partial class CloseAccountRequest : IInspectable
    {
        public string GetSummary()
        {
            return $"Requested closing account {Account}";
        }
    }
}
