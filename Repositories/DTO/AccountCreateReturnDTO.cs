using Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.DTO
{
    public class AccountCreateReturnDTO
    {
        public Account Account { get; set; }
        public string token { get; set; } = null!;
    }
}
