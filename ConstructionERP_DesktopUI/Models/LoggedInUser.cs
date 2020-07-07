using System;

namespace ConstructionERP_DesktopUI.Models
{
    public class LoggedInUser
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public string Token { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
