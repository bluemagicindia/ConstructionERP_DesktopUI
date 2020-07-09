using System;

namespace ConstructionERP_DesktopUI.Models
{
    public class TypeModel
    {

        public long ID { get; set; }

        public string Title { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; }

    }

}
