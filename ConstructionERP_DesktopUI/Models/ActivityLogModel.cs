using System;

namespace ConstructionERP_DesktopUI.Models
{
    public class ActivityLogModel
    {
        public long ID { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public long? ProjectID { get; set; }
        public ProjectModel Project { get; set; }

    }
}