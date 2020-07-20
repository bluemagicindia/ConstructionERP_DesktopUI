using ConstructionERP_DesktopUI.Models;
using System;

namespace ConstructionERP_DesktopUI.Models
{
    public class DocumentModel
    {
        public long ID { get; set; }

        public string Title { get; set; }

        public long? ProjectID { get; set; }

        public ProjectModel Project { get; set; }

        public string DocUrl { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; }
    }
}
