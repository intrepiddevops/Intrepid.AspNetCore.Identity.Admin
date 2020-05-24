using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intrepid.AspNetCore.Identity.Admin.Models
{
    public class ClaimsViewModel
    {
        public ClaimsViewModel()
        {
            GridControl = new GridControlModel();
            GridData = new List<ClaimGridRowModel>();
        }

        public GridControlModel GridControl { get; set; }
        public List<ClaimGridRowModel> GridData { get; set; }
    }
}
