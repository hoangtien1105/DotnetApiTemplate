using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.DTO
{
    public class AccountFilterModel
    {

        public string Sort { get; set; } = "id";
        public string SortDirection { get; set; } = "desc";
        public string? Role { get; set; }
        public bool? isDeleted { get; set; }
        public string? Gender { get; set; }
        public string? Search { get; set; }

    }
}
