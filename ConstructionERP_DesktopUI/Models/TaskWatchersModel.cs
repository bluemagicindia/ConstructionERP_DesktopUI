namespace ConstructionERP_DesktopUI.Models
{
    public class TaskWatchersModel
    {
        public long ID { get; set; }

        public long? TaskID { get; set; }

        public long? SiteManagerID { get; set; }

        public TaskModel Task { get; set; }

        public SiteManagerModel SiteManager { get; set; }
    }
}
