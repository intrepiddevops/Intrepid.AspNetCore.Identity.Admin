using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intrepid.AspNetCore.Identity.Admin.Models
{
    public class RolesViewModel
    {
        public RolesViewModel()
        {
            GridControl = new GridControlModel();
            GridData = new List<RoleGridRowModel>();
        }

        public GridControlModel GridControl { get; set; }
        public List<RoleGridRowModel> GridData { get; set; }
    }
}
