using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intrepid.AspNetCore.Identity.AdminPGSql.Models
{
    public class ClaimGridRowModel
    {
        public string ClaimId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool IsReadOnly { get; set; }
    }
}
