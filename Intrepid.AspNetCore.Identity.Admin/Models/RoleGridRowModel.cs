using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Intrepid.AspNetCore.Identity.Admin.Models
{
    public class RoleGridRowModel
    {
        public string RoleId { get; set; }
        [Required]
        public string Name { get; set; }
        public int Count { get; set; }
    }
}
