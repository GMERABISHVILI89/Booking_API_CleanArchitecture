using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking_Domain.Models
{
    public class EmailSettings
    {
        public string Email { get; set; }
        public string Password { get; set; } = "qoeluciemkrewumz";
        public string Host { get; set; }
        public string DisplayName { get; set; }
        public int Port { get; set; }

    }
}
