using Newtonsoft.Json;

namespace ConstructionERP_DesktopUI.Models
{
    public class FlatModel
    {
        public long Id { get; set; }

        public string Number { get; set; }

        public decimal EstimatedAmount { get; set; }

        public decimal AggregateAmountTotal { get; set; }

        public decimal EMI { get; set; }

        public int Days { get; set; }

        public long ProjectId { get; set; }

        public long CustomerId { get; set; }

        public ProjectModel Project { get; set; }

        //public CustomerModel Customer { get; set; }
    }
}
