using CommandLine;

namespace TIKSN.Lionize.IdentityManagementService.Shell
{
    [Verb("Sha256")]
    public class Sha256Options
    {
        public Sha256Options(string originalSecret)
        {
            OriginalSecret = originalSecret;
        }

        [Option]
        public string OriginalSecret { get; }
    }
}