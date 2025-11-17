using Domain.Common;
using Microsoft.AspNetCore.Identity;


namespace Domain.Identity
{
    public class AppUser : IdentityUser<Guid>, IAuditedEntity
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateBirth { get; set; }
        public ICollection<AppUserRole> UserRoles { get; set; } = new List<AppUserRole>();
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
