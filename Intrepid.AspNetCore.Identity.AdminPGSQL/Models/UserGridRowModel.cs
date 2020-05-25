using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intrepid.AspNetCore.Identity.AdminPGSql.Models
{
    public class UserGridRowModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
        public bool IsLocked { get; set; }
    }
}
