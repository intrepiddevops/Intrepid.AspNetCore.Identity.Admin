using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Intrepid.AspNetCore.Identity.Admin.Common.Models
{
    public class IdentityRoleDTO
    {
        public string Id { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string Name { get; set; }
        public string NormalizedName { get; set; }
        
        public List<string> Claims { get; set; }
        public int UserCount { get; set; }
    }
}
