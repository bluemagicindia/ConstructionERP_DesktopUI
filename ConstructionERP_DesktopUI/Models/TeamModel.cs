using System;
using System.Collections.Generic;

namespace ConstructionERP_DesktopUI.Models
{
    public class TeamModel
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public long? ProjectID { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; }

        //public DateTime ModifiedOn { get; set; }

        //public string ModifiedBy { get; set; }

        public IEnumerable<TeamSiteManagersModel> TeamSiteManagers { get; set; }
    }
}
