using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Intrepid.AspNetCore.Identity.Admin.Common.Models
{
    public class ResultDTO<T>
    {
        public ResultDTO()
        {
            this.ErrorMsg = new List<string>();
            this.IdentityError = new List<IdentityErrorDTO>();
        }
        public bool IsSuccess { get; set; }
        public List<string> ErrorMsg { get; set; }
        public List<IdentityErrorDTO> IdentityError { get; set; }
        public string DetailErrorException { get; set; }
        public T ReturnObject { get; set; }
        public Exception GenericException { get; set; }
    }
}
