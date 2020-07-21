using System;

namespace ConstructionERP_DesktopUI.Models
{
    public class MaterialModel 
    {

        public long ID { get; set; }

        public string Name { get; set; }

        public long? UnitID { get; set; }

        public UnitModel Unit { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; }

    }
}
