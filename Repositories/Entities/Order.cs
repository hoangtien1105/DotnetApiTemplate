using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Entities
{
    public class Order : BaseEntity
    {
        // ID
        // OrderDate = CreatedDate
        // LastUpdateDate = ModifiedDate 
        public float TotalPrice { get; set; }
        public string? Description { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public byte Status { get; set; }
        public string AccountId { get; set; }

        //Navigation
        public virtual Account Account { get; set; } = null!;
        public virtual ICollection<OrderDetails>? OrderDetails {get ; set ;}

    }
}
