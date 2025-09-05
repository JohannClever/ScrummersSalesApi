using ScrummersSalesApi.Backend.Orders.Domain.Enums;

namespace ScrummersSalesApi.Backend.Orders.Domain.Commons.Exception
{
    public static class OrderStatusExtension
    {
        /// <summary>
        /// Converts an integer value to the corresponding OrderStatus enum name.
        /// </summary>
        /// <param name="value">The integer value representing an OrderStatus.</param>
        /// <returns>The name of the enum if valid; otherwise "Unknown".</returns>
        public static string ToOrderStatusString(this int value)
        {
            if (System.Enum.IsDefined(typeof(OrderStatus), value))
            {
                return ((OrderStatus)value).ToString();
            }
            return "Unknown";
        }
    }
}
