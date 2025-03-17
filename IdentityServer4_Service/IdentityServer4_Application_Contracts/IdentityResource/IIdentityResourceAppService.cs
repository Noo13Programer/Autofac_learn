using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityServer4_Application_Contracts.IdentityResource
{
    public interface IIdentityResourceAppService
    {
        Task<string> GetAsync();
    }
}
