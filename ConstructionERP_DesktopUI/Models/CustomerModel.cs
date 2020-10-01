using System;
using System.Collections.Generic;

namespace ConstructionERP_DesktopUI.Models
{
    public class CustomerModel
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string GSTN { get; set; }

        public string PhoneNumbers { get; set; }

        public string CurrentAddress { get; set; }

        public string PAN { get; set; }

        public string Aadhaar { get; set; }

        public string ReferredBy { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public string ModifiedBy { get; set; }

        public bool Status { get; set; } = true;

        public IEnumerable<FlatModel> Flats { get; set; }
    }
}
