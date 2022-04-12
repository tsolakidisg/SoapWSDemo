using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SoapDemo.Models
{
    public class ResponseModel<T>
    {
        public T Data { get; set; }
        public int ResultCode { get; set; }
        public string Message { get; set; }
    }
}