namespace Demo.PL.ViewModels.DashBoard
{
    public class ActivityViewModel
    {
        public string LogLevel { get; set; }

        public bool Status { get; set; }
        public string Message { get; set; }

        public string Exception { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
