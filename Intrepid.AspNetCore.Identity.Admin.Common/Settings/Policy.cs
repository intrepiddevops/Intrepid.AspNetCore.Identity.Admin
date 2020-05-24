using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Intrepid.AspNetCore.Identity.Admin.Common.Settings
{
    public class Policy
    {
        public List<RoleElement> Roles { get; set; }
    }
    public class RoleElement
    {
        public string Role { get; set; }
    }
}
