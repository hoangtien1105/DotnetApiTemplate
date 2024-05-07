using Microsoft.AspNetCore.Http;
using Repositories.Interfaces;
using Repositories.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Commons
{
    public class ClaimsService : IClaimsService
    {

        public ClaimsService(IHttpContextAccessor httpContextAccessor) 
        {
            // todo implementation to get the current userId
            var identity = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            var extractedId = AuthenTools.GetCurrentAccountId(identity);
            GetCurrentUserId = string.IsNullOrEmpty(extractedId) ? Guid.Empty : Guid.Parse(extractedId);
        }

        public Guid GetCurrentUserId { get; }

    }
}
