namespace Demo.PL.ViewModels.User
{
    public class UsersViewModel
    {
        public string LName { get; set; }
        public string FName { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
    }
}
