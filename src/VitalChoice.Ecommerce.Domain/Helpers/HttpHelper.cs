using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace VitalChoice.Ecommerce.Domain.Helpers
{
    public static class HttpHelper
    {
        public static string GetCurrentUrl(this HttpRequest request)
        {
            string str2 = request.PathBase.Value;
            string str3 = request.Path.Value;
            string str4 = request.QueryString.Value;
            return
                new StringBuilder("/".Length + str2.Length + str3.Length + str4.Length).Append("/")
                    .Append(str2)
                    .Append(str3)
                    .Append(str4)
                    .ToString();
        }
    }
}
