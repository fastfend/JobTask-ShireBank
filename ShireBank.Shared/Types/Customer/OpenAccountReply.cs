using ShireBank.Shared.Data.Interfaces;

namespace ShireBank.Shared.Protos
{
    public partial class OpenAccountReply : IInspectable
    {
        public string GetSummary()
        {
            return $"Opened account {Account}";
        }
    }
}
