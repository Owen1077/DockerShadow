using DockerShadow.Core.DTO.Request;
using System.Diagnostics.CodeAnalysis;

namespace DockerShadow.Core.Contract
{
    public interface IQueueManager
    {
        ValueTask<SendMailRequest> PullEmailAsync(CancellationToken cancellationToken);
        ValueTask PushEmailAsync([NotNull] SendMailRequest request);
    }
}
