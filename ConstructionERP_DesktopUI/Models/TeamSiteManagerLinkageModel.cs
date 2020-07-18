using System;

namespace ConstructionERP_DesktopUI.Models
{
    public class TeamSiteManagerLinkageModel
    {
        public long ID { get; set; }

        public long? TeamID { get; set; }

        public TeamModel Team { get; set; }

        public long? SiteManagerID { get; set; }

        public SiteManagerModel SiteManager { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; }

    }
}
