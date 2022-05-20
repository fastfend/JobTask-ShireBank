using ShireBank.Shared.Data.Interfaces;

namespace ShireBank.Shared.Protos
{
    public partial class WithdrawReply : IInspectable
    {
        public string GetSummary()
        {
            return $"Withdrawn {(decimal)Value}";
        }
    }
}
