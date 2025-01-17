using DockerShadow.Core.DTO.Request;
using DockerShadow.Core.DTO.Response;

namespace DockerShadow.Core.Contract
{
    public interface IAPIImplementation
    {
        Task<SendMailResponse> SendMail(SendMailRequest request);
    }
}
