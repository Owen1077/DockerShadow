namespace DockerShadow.Domain.Entities.Base
{
    public interface IEntityBase<T>
    {
        T Id { get; }
        DateTime CreatedAt { get; set; }
        void SetNewId();
    }
}
