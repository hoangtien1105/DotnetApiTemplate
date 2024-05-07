using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Entities
{
    public class OrderDetails : BaseEntity
    {
        public byte Quantity { get; set; }
        public float ProductPrice { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }

        //NAVIGATION
        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;



    }
}
