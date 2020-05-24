using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intrepid.AspNetCore.Identity.Admin.Models
{
    public class GridControlModel
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get { return (int)Math.Ceiling((decimal)TotalRecords / (decimal)PageSize); } }
    }
}
