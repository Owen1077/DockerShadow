namespace DockerShadow.Domain.Entities;

public partial class BankToBrokerFundWalletLog
{
    public long Id { get; set; }

    public string? Amount { get; set; }

    public int? Attempts { get; set; }

    public DateTime CreatedDate { get; set; }

    public string? DebitAccountNumber { get; set; }

    public DateTime? LastModifiedDate { get; set; }

    public string? ResponseCode { get; set; }

    public string? ResponseMessage { get; set; }

    public int? RvslResponseCode { get; set; }

    public string? RvslResponseMessage { get; set; }

    public string? Status { get; set; }

    public string? TransactionReference { get; set; }

    public string? UserId { get; set; }

    public string? JobHostName { get; set; }

    public string? OriginHostName { get; set; }
}
