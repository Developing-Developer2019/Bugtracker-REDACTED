using Bugtracker.Models.Common;
using Bugtracker.Models.Common.Status;

namespace Bugtracker.Models.Account
{
    public class AccountDTO : CommonDTO
    {
        public LoginBO Login { get; set; } = new LoginBO();
        public RegisterBO Register { get; set; } = new  RegisterBO();
        public SettingsBO Settings { get; set; } = new SettingsBO();
    }
}
