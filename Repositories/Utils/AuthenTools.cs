using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable CS8603 // Possible null reference return =))
namespace Repositories.Utils
{
    public static class AuthenTools
    {
        public static string GetCurrentAccountId(ClaimsIdentity identity)
        {
            if (identity != null)
            {
                var userClaims = identity.Claims;
                return userClaims.FirstOrDefault(x => x.Type == "UserId")?.Value;
            }
            return null;
        }
    }
}
