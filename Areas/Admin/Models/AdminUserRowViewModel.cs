namespace JobSequenceApp.Areas.Admin.Models
{
    public class AdminUserRowViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Roles { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
    }
}