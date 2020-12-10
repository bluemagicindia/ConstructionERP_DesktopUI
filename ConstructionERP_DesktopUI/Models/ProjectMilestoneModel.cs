using System;

namespace ConstructionERP_DesktopUI.Models
{
    public class ProjectMilestoneModel
    {
        public long ID { get; set; }


        public long? ProjectID { get; set; }

        public ProjectModel Project { get; set; }
        
        public string Title { get; set; }

        public float Percentage { get; set; }

        public DateTime TargetEnd { get; set; }

        public bool IsComplete { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; }

    }
}
