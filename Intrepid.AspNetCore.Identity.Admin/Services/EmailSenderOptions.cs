using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Intrepid.AspNetCore.Identity.Admin.Services
{
    public class EmailSenderOptions
    {
        public string SendGridUser { get; set; }
        public string SendGridKey { get; set; }
        public string FromEmail { get; set; }
    }
}
