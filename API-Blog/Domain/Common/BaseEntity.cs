using System.ComponentModel.DataAnnotations;

namespace Domain.Common
{
    public class BaseEntity : IAuditedEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
