using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Common;

namespace Domain.Entities;

public class UserRefreshToken : BaseEntity, IAuditedEntity
{
    [Required]
    [Column(TypeName = "varchar(50)")]
    public required string UserId { get; set; }

    [Required]
    public required string AccessToken { get; set; }

    [Required]
    [Column(TypeName = "varchar(1000)")]
    public required string RefreshToken { get; set; }

    public required DateTime ExpiryTime { get; set; }

    [DefaultValue(false)]
    public bool IsRevoked { get; set; }

    [DefaultValue(false)]
    public bool IsUsed { get; set; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}