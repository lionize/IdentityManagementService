using CommandLine;

namespace TIKSN.Lionize.IdentityManagementService.Shell
{
    [Verb("AddApiSecret", HelpText = "Add Api Secret.")]
    public class AddApiSecretOptions
    {
        public AddApiSecretOptions(int id)
        {
            Id = id;
        }

        [Option]
        public int Id { get; }
    }
}