namespace DockerShadow.Domain.Entities;

public partial class BankToBrokerWithdrawalLog
{
    public long Id { get; set; }

    public string? Amount { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? CreditAccountNumber { get; set; }

    public string? EsbResponse { get; set; }

    public string? OriginHostName { get; set; }

    public int? ResponseCode { get; set; }

    public string? ResponseMessage { get; set; }

    public string? Status { get; set; }

    public string? TransactionReference { get; set; }

    public string? UserId { get; set; }
}
