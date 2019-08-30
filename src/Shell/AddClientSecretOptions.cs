using CommandLine;

namespace TIKSN.Lionize.IdentityManagementService.Shell
{
    [Verb("AddClientSecret", HelpText = "Add Client Secret.")]
    public class AddClientSecretOptions
    {
        public AddClientSecretOptions(int id)
        {
            Id = id;
        }

        [Option]
        public int Id { get; }
    }
}