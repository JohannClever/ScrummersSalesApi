namespace ScrummersSalesApi.Backend.Orders.Infrastructure.Extensions
{
    public class StoredProcedureParameters
    {
        public Dictionary<string, object> Parameters { get; } = new Dictionary<string, object>();

        public void AddParameter(string name, object value)
        {
            Parameters[name] = value;
        }
    }
}
