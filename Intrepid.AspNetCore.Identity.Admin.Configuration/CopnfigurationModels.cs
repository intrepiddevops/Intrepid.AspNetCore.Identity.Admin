using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Intrepid.AspNetCore.Identity.Admin.Configuration
{
    public class Claim
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
  

    public class User
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Claim> Claims { get; set; }
        public List<string> Roles { get; set; }

    }
    public class Role { 
        public string Name { get; set; }
        public List<Claim> Claims { get; set; }
    }


    public class IdentityDataConfiguration
    {
        public List<User> Users { get; set; }
        public List<Role> Roles { get; set; }
        public List<CustomClaim> Claims { get; set; }
    }

    

    public class CustomClaim
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ClaimType { get; set; }
    }
}
