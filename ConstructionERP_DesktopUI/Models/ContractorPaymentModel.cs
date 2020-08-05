using System;

namespace ConstructionERP_DesktopUI.Models
{
    public class ContractorPaymentModel
    {
        public long ID { get; set; }

        public long? ProjectID { get; set; }

        public long? ContractorID { get; set; }

        public long TentativeAmount { get; set; }

        public long PaidAmount { get; set; }

        public string Remarks { get; set; }

        public DateTime PaymentDate { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public bool Status { get; set; }

        public ContractorModel Contractor { get; set; }

        public ProjectModel Project { get; set; }

        public long Cumulative { get; set; }
    }
}
