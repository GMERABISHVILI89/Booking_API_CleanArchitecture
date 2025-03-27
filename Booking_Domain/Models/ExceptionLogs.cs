using Booking_Domain.Entities;

namespace Booking_Domain.Models
{
    public class ExceptionLogs : BaseClass
    {
        public int Id { get; set; }

        public int StatusCode { get; set; }
        
        public string Message { get; set; }

        public string StackTrace { get; set; }
    }
}
