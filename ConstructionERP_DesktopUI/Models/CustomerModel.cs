using System.Collections.Generic;

namespace ConstructionERP_DesktopUI.Models
{
    public class CustomerModel
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string PhonePrimary { get; set; }

        public string PhoneSecondary { get; set; }

        public string CurrentAddress { get; set; }

        public string PAN { get; set; }

        public string Aadhaar { get; set; }

        public string ReferredBy { get; set; }

        public IEnumerable<FlatModel> Flats { get; set; }
    }
}
