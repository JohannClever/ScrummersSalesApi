namespace ScrummersSalesApi.Backend.Orders.Domain.Commons.Exception
{
    public class DataAccessException : System.Exception
    {
        public DataAccessException(string message, System.Exception innerException) : base($"DataAccess: {message}", innerException)
        {
        }
    }
}
