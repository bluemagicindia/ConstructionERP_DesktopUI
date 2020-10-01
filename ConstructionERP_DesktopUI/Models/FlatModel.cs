namespace ConstructionERP_DesktopUI.Models
{
    public class FlatModel
    {
        public long Id { get; set; }

        public string Number { get; set; }

        public long? ProjectID { get; set; }

        public decimal EstimatedAmount { get; set; }

        public decimal AverageTotal { get; set; }

        public decimal EMI { get; set; }

        public int Days { get; set; }

        public ProjectModel Project { get; set; }
    }
}
