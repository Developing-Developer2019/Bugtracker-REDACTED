using Bugtracker.Models.Common.Enum;

namespace Bugtracker.Models.Common.Status
{
    public class StatusBO
    {
        public string? StatusMessage { get; set; }
        public Enum.Status StatusType { get; set; }
    }
}
