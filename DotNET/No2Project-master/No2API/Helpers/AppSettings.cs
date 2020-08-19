using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace No2API.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string GlobalDomainName { get; set; }
        public string MainDomainName { get; set; }
        public string EmailConfirmationUrl { get; set; }
        public string ResetPasswordUrl { get; set; }
    }
}
