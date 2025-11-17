namespace Shared.Models
{
    public class JwtUserInformation
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? UniqueName { get; set; }
        public string? Email { get; set; }
    }
}
