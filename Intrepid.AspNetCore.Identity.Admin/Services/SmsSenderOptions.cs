using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intrepid.AspNetCore.Identity.Admin.Services
{
    public class SmsSenderOptions
    {
        public string AccountSid { get; set; }
        public string AuthToken { get; set; }
        public string FromNumber { get; set; }
    }
}
