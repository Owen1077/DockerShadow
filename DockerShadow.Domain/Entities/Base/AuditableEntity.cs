using System.ComponentModel.DataAnnotations.Schema;

namespace DockerShadow.Domain.Entities.Base
{
    public class AuditableEntity : EntityBase<string>, IAuditableEntity
    {
        [Column("CREATED_BY")]
        public string CreatedBy { get; set; }
        [Column("UPDATED_BY")]
        public string UpdatedBy { get; set; }
        [Column("UPDATED_AT")]
        public DateTime? UpdatedAt { get; set; }
    }
}
