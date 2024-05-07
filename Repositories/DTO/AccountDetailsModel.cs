using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.DTO
{
    public class AccountDetailsModel
    {
        public required string Id { get; set; }  // Sử dụng kiểu string cho Id
        public string Email { get; set; } = null!;


        public string? UnsignFullName { get; set; } = "";

        public string? FullName { get; set; }

        public DateTime? Dob { get; set; }

        public string? Gender { get; set; }

        public string? Image { get; set; } = "";
        public bool? IsDeleted { get; set; } = false;
        public List<RoleInfoModel>? Role { get; set; } = null;// 

    }
}
