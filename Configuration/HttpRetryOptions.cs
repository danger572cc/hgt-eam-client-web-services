namespace HGT.EAM.Client.Configuration;

public sealed class HttpRetryOptions
{
    public int MaxRetries { get; set; } = 3;
    public int BaseDelayMilliseconds { get; set; } = 500;
}
