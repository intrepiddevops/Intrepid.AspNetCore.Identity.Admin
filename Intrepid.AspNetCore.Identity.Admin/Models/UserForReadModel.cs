using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intrepid.AspNetCore.Identity.Admin.Models
{
    public class UserForReadModel : UserBaseModel
    {
        public bool IsEmailConfirmed { get; set; }
        public bool IsPhoneConfirmed { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
        public bool IsLockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; }
        public List<RoleForReadModel> Roles { get; set; }
        public List<ClaimForReadModel> Claims { get; set; }
    }
}
