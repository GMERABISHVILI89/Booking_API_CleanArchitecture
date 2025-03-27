using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking_Domain.Entities
{
    public class BaseClass
    {
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime? LastModifiedDate { get; set; }
        public int? CreatorId { get; set; }
        public int? ModifierId { get; set; }
    }
}
