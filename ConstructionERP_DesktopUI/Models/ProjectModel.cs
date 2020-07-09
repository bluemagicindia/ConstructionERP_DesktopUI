using System;

namespace ConstructionERP_DesktopUI.Models
{
    public class ProjectModel
    {

        public long ID { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public StatusModel Status { get; set; }
        public long? ProjectStatusID { get; set; }

        public TypeModel Type { get; set; }
        public long? ProjectTypeID { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime DueDate { get; set; }

        public string Address { get; set; }    

        public ContractorModel Contractor { get; set; }
        public long? ContractorID { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

    }
}
