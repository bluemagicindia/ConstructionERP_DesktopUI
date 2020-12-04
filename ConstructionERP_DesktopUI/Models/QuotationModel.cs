using System;

namespace ConstructionERP_DesktopUI.Models
{
    public class QuotationModel
    {

        public long ID { get; set; }

        public long Cost { get; set; }

        public string Vendor { get; set; }

        public string DocUrl { get; set; }

        public MaterialModel Material { get; set; }

        public long? MaterialID { get; set; }

        public string Narration { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

    }
}
