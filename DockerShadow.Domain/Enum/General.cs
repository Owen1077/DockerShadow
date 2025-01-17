namespace DockerShadow.Domain.Enum
{
    public enum ClearedStatus
    {
        Processing = 0,
        Batched = 1,
        Cleared = 2,
    }
    public enum TransactionDirection
    {
        Inflow = 1,
        Outflow = 2,
    }

    // Populate this enum with the list of events that you want to audit
    public enum AuditEventType
    {
        Account = 1,
        Participant = 2,
        BasicUser = 3,
        User = 4,
        LiquidityTransfer = 5,

    }

    public enum UserStatus
    {
        Active = 1,
        Inactive = 2,
    }
}
