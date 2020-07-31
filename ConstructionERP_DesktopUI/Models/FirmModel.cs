using System;

namespace ConstructionERP_DesktopUI.Models
{
    public class FirmModel
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; }

    }
}
