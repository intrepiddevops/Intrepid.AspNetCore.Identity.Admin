using System;
using System.Collections.Generic;
using System.Text;

namespace Intrepid.AspNetCore.Identity.Admin.Common.Models
{
    public class IdentityUserMetaData
    {
        public int TotalNumberUsers { get; set; }
        public int TotlaLockedOut { get; set; }
        public int TotalEmailNotConfirm { get; set; }
    }
}
