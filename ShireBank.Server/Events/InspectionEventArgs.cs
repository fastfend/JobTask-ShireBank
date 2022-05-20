namespace ShireBank.Server.Events
{
    public class InspectionEventArgs
    {
        public InspectionEventArgs(object inspected) { Inspected = inspected; }
        public object Inspected { get; }
    }
}
