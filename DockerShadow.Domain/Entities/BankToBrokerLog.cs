namespace DockerShadow.Domain.Entities;

public partial class BankToBrokerLog
{
    public long Id { get; set; }

    public string? AccountNumber { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? UserId { get; set; }
}
