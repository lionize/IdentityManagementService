using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TIKSN.Lionize.IdentityManagementService.Shell
{
    public class OriginalSecret
    {
        public OriginalSecret(string originalSecret, string hashedSecret)
        {
            Original = originalSecret ?? throw new System.ArgumentNullException(nameof(originalSecret));
            Hashed = hashedSecret ?? throw new System.ArgumentNullException(nameof(hashedSecret));
        }

        public string Hashed { get; }

        public string Original { get; }
    }
}
