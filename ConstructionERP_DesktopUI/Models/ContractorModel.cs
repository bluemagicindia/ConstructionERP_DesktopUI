using System;

namespace ConstructionERP_DesktopUI.Models
{
    public class ContractorModel 
    {

        public long ID { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string GSTN { get; set; }

        public string WorkDescription { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public bool Status { get; set; } = true;

    }

}
