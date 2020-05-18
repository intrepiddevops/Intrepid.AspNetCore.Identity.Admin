using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Intrepid.AspNetCore.Identity.Admin.Common.Models
{
    public class ResultDto<T>
    {
        public ResultDto()
        {
            this.ErrorMsg = new List<string>();
        }
        public bool IsSuccess { get; set; }
        public List<string> ErrorMsg { get; set; }
        public string DetailErrorException { get; set; }
        public T ReturnObject { get; set; }
        public Exception GenericException { get; set; }
    }
}
