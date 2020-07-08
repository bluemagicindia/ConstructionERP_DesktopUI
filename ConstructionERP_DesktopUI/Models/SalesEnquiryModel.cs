using System;

namespace ConstructionERP_DesktopUI.Models
{
    public class SalesEnquiryModel 
    {

        public long ID { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string RelatedTo { get; set; }

        public DateTime EnquiryDate { get; set; }

        public DateTime FollowUpDate { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public bool Status { get; set; } = true;

    }
}
