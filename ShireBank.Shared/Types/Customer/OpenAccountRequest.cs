using ShireBank.Shared.Data.Interfaces;

namespace ShireBank.Shared.Protos
{
    public partial class OpenAccountRequest : IInspectable
    {
        public string GetSummary()
        {
            return $"Requested account opening for {FirstName} {LastName} with debt limit {(decimal)DebtLimit}";
        }
    }
}
