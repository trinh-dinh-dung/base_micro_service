using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Base
{
    public class JwtSettings
    {
        /// <summary>OpenIddict / OIDC authority (vd. http://auth-server:8080). Nếu có → validate RS256 qua metadata.</summary>
        public string? Authority { get; set; }

        /// <summary>Issuer trong access token (vd. http://localhost:5010) — khớp URL browser đăng nhập SSO.</summary>
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
