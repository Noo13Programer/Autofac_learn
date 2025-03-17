using IdentityServer4_Application_Contracts.IdentityResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4_Application.IdentityResource
{
    public class IdentityResourceAppService : IdentityService4Service, IIdentityResourceAppService
    {
        public async Task<string> GetAsync()
        {
            return "成功";
        }
    }
}
