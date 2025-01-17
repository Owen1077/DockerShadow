using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DockerShadow.Domain.Entities.Base
{
    public class EntityBase<T> : IEntityBase<T>
    {
        [Column("ID")]
        [Key]
        public T Id { get; set; }

        [Column("CREATED_AT")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual void SetNewId()
        {
            throw new NotImplementedException();
        }
    }
}
