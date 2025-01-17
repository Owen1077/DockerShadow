using DockerShadow.Core.DTO.Request;
using DockerShadow.Core.DTO.Response;
using DockerShadow.Domain.Common;
using DockerShadow.Domain.QueryParameters;

namespace DockerShadow.Core.Contract
{
    public interface IUserService
    {
        Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request, CancellationToken cancellationToken);
        Task<Response<string>> LogoutAsync();
        Task<PagedResponse<List<UserResponse>>> GetUsersAsync(UserQueryParameters queryParameters, CancellationToken cancellationToken);
        Task<Response<UserResponse>> GetUserById(string id, CancellationToken cancellationToken);
        Task<Response<string>> AddUserAsync(AddUserRequest request, CancellationToken cancellationToken);
        Task<Response<string>> EditUserAsync(EditUserRequest request, CancellationToken cancellationToken);
        Task<Response<string>> DeleteUserAsync(DeleteUserRequest request, CancellationToken cancellationToken);
        Task<Response<string>> ResetUserAsync(ResetUserRequest request);
        Task<Response<string>> ChangePasswordWithToken(ChangePasswordRequest request);
        Task<Response<string>> ResetUserLockoutAsync(ResetUserRequest request);
    }
}
