using DockerShadow.Domain.Common;
using DockerShadow.Domain.Entities.Base;
using DockerShadow.Domain.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DockerShadow.Domain.Entities
{
    public class Transaction : EntityBase<string>
    {
        public Transaction()
        {
            SetNewId();
        }
        [Column("SENDER_ACCOUNT_NUMBER")]
        [Required]
        public string SenderAccountNumber { get; set; }
        [Column("BENEFICIARY_ACCOUNT_NUMBER")]
        [Required]
        public string BeneficiaryAccountNumber { get; set; }
        [Column("SENDER_ACCOUNT_NAME")]
        [Required]
        public string SenderAccountName { get; set; }
        [Column("BENEFICIARY_ACCOUNT_NAME")]
        [Required]
        public string BeneficiaryAccountName { get; set; }
        [Column("SENDER_BANK_CODE")]
        [Required]
        public string SenderBankCode { get; set; }
        [Column("BENEFICIARY_BANK_CODE")]
        [Required]
        public string BeneficiaryBankCode { get; set; }
        [Column("SENDER_ACCOUNT_ID")]
        [Required]
        public string SenderAccountId { get; set; }
        [Column("BENEFICIARY_ACCOUNT_ID")]
        [Required]
        public string BeneficiaryAccountId { get; set; }
        [Column("TRANSACTION_REFERENCE")]
        [Required]
        public string TransactionReference { get; set; }
        [Column("SESSION_ID")]
        [Required]
        public string SessionId { get; set; }
        [Column("TRANSACTION_AMOUNT", TypeName = "decimal(18, 2)")]
        [Required]
        public decimal TransactionAmount { get; set; }
        [Column("CHANNEL")]
        [Required]
        public string Channel { get; set; }
        [Column("NARRATION")]
        [Required]
        public string Narration { get; set; }
        [Column("IS_CLEARED")]
        public ClearedStatus ClearedStatus { get; set; }
        [Column("IS_SETTLED")]
        public bool IsSettled { get; set; }
        [Column("LEDGER_NO")]
        public long LedgerNo { get; set; }
        [Column("UPDATED_AT")]
        public DateTime? UpdatedAt { get; set; }
        [Column("SETTLEMENT_WINDOW_ID")]
        [Required]
        public long SettlementWindowId { get; set; }
        public override void SetNewId()
        {
            Id = $"TRN_{CoreHelpers.CreateUlid(DateTimeOffset.Now)}";
        }
    }
}
