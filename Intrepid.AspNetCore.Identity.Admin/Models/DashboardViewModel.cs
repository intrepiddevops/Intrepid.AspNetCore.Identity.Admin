using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Intrepid.AspNetCore.Identity.Admin.Models
{
    public class DashboardViewModel
    {
        public DashboardViewModel()
        {
            RoleCounts = new List<RoleCountModel>();
        }

        public int TotalUsers { get; set; }
        public int LockedUsers { get; set; }
        public int PhoneNumberConfirmUsers { get; set; }
        public int EmailUnconfirmedUsers { get; set; }
        public List<RoleCountModel> RoleCounts { get; set; }
        
    }

    public class RoleCountModel
    {
        public string RoleId { get; set; }
        [Required]
        public string Name { get; set; }
        public string ConcurrencyStamp { get; set; }
        public int Count { get; set; }
    }
}
