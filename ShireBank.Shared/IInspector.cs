namespace ShireBank.Shared
{
    internal interface IInspector
    {
        string GetFullSummary();

        void StartInspection();

        void FinishInspection();
    }
}