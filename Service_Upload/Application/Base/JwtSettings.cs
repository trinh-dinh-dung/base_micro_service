using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Base
{
    public class JwtSettings
    {
        public string? Authority { get; set; }
        public string[]? ValidIssuers { get; set; }
        public bool ValidateIssuerSigningKey { get; set; }
        public string IssuerSigningKey { get; set; }
        public bool ValidateIssuer { get; set; } = true;
        public string ValidIssuer { get; set; }
        public bool ValidateAudience { get; set; } = true;
        public string ValidAudience { get; set; }
        public bool RequireExpirationTime { get; set; }
        public bool ValidateLifetime { get; set; } = true;
        public int ExpaiTime { get; set; }
    }
}
