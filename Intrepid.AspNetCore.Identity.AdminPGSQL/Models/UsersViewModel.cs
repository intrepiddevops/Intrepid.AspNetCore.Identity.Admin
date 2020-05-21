using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intrepid.AspNetCore.Identity.Admin.Models
{
    public class UsersViewModel
    {
        public UsersViewModel()
        {
            GridControl = new GridControlModel();
            GridData = new List<UserGridRowModel>();
        }

        public GridControlModel GridControl { get; set; }
        public List<UserGridRowModel> GridData { get; set; }
    }


}
