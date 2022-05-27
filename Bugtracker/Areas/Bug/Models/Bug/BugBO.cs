using Bugtracker.Areas.Identity.Data;
using Bugtracker.Models.Common.Enum;
using Bugtracker.Models.Common.Extensions;

namespace Bugtracker.Areas.Bug.Models.Bug
{
    public class BugBO : BaseEntity<int>
    {
        public string Bug_Name { get; set; }
        public string Bug_Description { get; set; }
        public DateTime Bug_CreatedDateT { get; set; }
        public DateTime Bug_ModifiedDateT { get; set; }
        public Status Bug_Status { get; set; }
        public string Bug_CreatedByID { get; set; }
        public string Bug_ModifiedByID { get; set; }
    }
}
