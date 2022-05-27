using Bugtracker.Models.Common.Status;

namespace Bugtracker.Models.Common
{
    public class CommonDTO
    {
        public StatusBO Status { get; set; } = new StatusBO();
    }
}
