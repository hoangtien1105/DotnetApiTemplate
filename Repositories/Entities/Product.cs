using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Entities
{
    public class Product : BaseEntity
    {
        public string ProductName { get; set; } = "";
        public string ProductDescription { get; set; } = "";
        public float ProductPrice { get; set; }
        //Navigation
        public virtual ICollection<OrderDetails>? OrderDetails { get; set; }
    }
}
