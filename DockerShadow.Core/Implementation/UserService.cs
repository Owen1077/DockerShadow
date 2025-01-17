using AutoMapper;
using DockerShadow.Core.Contract;
using DockerShadow.Core.DTO.Request;
using DockerShadow.Core.DTO.Response;
using DockerShadow.Core.Exceptions;
using DockerShadow.Core.Extension;
using DockerShadow.Domain.Common;
using DockerShadow.Domain.Entities;
using DockerShadow.Domain.Enum;
using DockerShadow.Domain.QueryParameters;
using DockerShadow.Domain.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace DockerShadow.Core.Implementation
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly JWTSettings _jwtSettings;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IAppSessionService _appSession;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ExternalApiOptions _externalApiOptions;

        public UserService(IMapper mapper,
            ILogger<UserService> logger,
            IOptions<JWTSettings> jwtSettings,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager,
            IAppSessionService appSession,
            INotificationService notificationService,
            IHttpContextAccessor httpContextAccessor,
            IOptions<ExternalApiOptions> externalApiOptions)
        {
            _mapper = mapper;
            _logger = logger;
            _jwtSettings = jwtSettings.Value;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _appSession = appSession;
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
            _externalApiOptions = externalApiOptions.Value;
        }

        public async Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request, CancellationToken cancellationToken)
        {
            // Check for the username
            User user = await _userManager.Users
                .Where(x => x.NormalizedUserName == request.Username.ToUpper())
                .FirstOrDefaultAsync(cancellationToken) ?? throw new ApiException("No Accounts Registered.");

            if (user.Status != UserStatus.Active)
            {
                throw new ApiException($"Inactive account.");
            }

            // Verify the username and password
            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                {
                    throw new ApiException($"This user has been locked. Kindly contact the administrator.");
                }
                throw new ApiException("Invalid Credentials.");
            }
            var roles = await _userManager.GetRolesAsync(user);

            JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user, roles, cancellationToken);
            AuthenticationResponse response = _mapper.Map<User, AuthenticationResponse>(user);

            response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            response.ExpiresIn = _jwtSettings.DurationInMinutes * 60;
            response.ExpiryDate = DateTime.Now.AddSeconds(_jwtSettings.DurationInMinutes * 60);
            response.Roles = roles.ToList();

            user.IsLoggedIn = true;
            user.LastLoginTime = DateTime.Now;
            await _userManager.UpdateAsync(user);

            return new Response<AuthenticationResponse>(response, $"Authenticated {user.UserName}");
        }

        private async Task<JwtSecurityToken> GenerateJWToken(User user, IList<string> roles, CancellationToken cancellationToken)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            DateTime utcNow = DateTime.UtcNow;
            string ipAddress = IpHelper.GetIpAddress();
            string sessionKey = Guid.NewGuid().ToString();
            await _appSession.CreateSession(sessionKey, user.UserName);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim("userId", user.Id),
                new Claim("name", user.Name),
                new Claim("emailAddress", user.Email),
                new Claim("username", user.UserName),
                new Claim("ip", ipAddress)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        public async Task<Response<string>> LogoutAsync()
        {
            var username = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "username")?.Value;

            if (string.IsNullOrEmpty(username))
            {
                return new Response<string>("Already logged out", (int)HttpStatusCode.OK, true);
            }

            _appSession.DeleteSession(username);

            // [TODO] Handle situations where the JWT token is expired
            User user = await _userManager.FindByNameAsync(username) ?? throw new ApiException("User not found.");
            user.IsLoggedIn = false;
            await _userManager.UpdateAsync(user);
            return new Response<string>("Successfully logged out", (int)HttpStatusCode.OK, true);
        }

        public async Task<PagedResponse<List<UserResponse>>> GetUsersAsync(UserQueryParameters queryParameters, CancellationToken cancellationToken)
        {
            IQueryable<User> pagedData = _userManager.Users;

            string? query = queryParameters.Query;
            string? role = queryParameters.Role;
            UserStatus? status = queryParameters.Status;

            // Check if there is a query and apply it
            if (!string.IsNullOrEmpty(query))
            {
                pagedData = pagedData.Where(x => x.Id.ToLower().Contains(query.ToLower())
                   || x.UserName.ToLower().Contains(query.ToLower())
                   || x.Email.ToLower().Contains(query.ToLower())
                   || x.Name.ToLower().Contains(query.ToLower()));
            }

            if (!string.IsNullOrEmpty(role))
            {
                pagedData = pagedData.Where(x => x.UserRoles.Any(x => x.Role!.Name == role));
            }

            // Check the status passed in the query parameters and if available use it to filter the result
            if (status.HasValue)
            {
                pagedData = pagedData.Where(x => x.Status == status.Value);
            }

            List<User> userList = await pagedData
                .OrderByDescending(x => x.CreatedAt)
                .Skip((queryParameters.PageNumber - 1) * queryParameters.PageSize)
                .Take(queryParameters.PageSize)
                .ToListAsync(cancellationToken);

            List<UserResponse> response = _mapper.Map<List<User>, List<UserResponse>>(userList);

            int totalRecords = await pagedData.CountAsync(cancellationToken);

            foreach (var userResponse in response)
            {
                var user = userList.First(x => x.Id == userResponse.Id);
                userResponse.Roles = await _userManager.GetRolesAsync(user);
            }

            return new PagedResponse<List<UserResponse>>(response, queryParameters.PageNumber, queryParameters.PageSize, totalRecords, $"Successfully retrieved users");
        }

        public async Task<Response<UserResponse>> GetUserById(string id, CancellationToken cancellationToken)
        {
            User userData = await _userManager.Users
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync(cancellationToken) ?? throw new ApiException($"No user found for User ID - {id}.");

            UserResponse response = _mapper.Map<User, UserResponse>(userData);

            return new Response<UserResponse>(response, $"Successfully retrieved user details for user with Id - {id}");
        }

        public async Task<Response<string>> AddUserAsync(AddUserRequest request, CancellationToken cancellationToken)
        {
            var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
            if (userWithSameUserName != null)
            {
                throw new ApiException($"Username '{request.UserName}' is already registered.");
            }

            // Check if the role exists
            if (!await _roleManager.RoleExistsAsync(request.Role))
            {
                throw new ApiException($"Invalid role specified.");
            }

            string username = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "username")?.Value ?? throw new ApiException("Username not found.");
            string email = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "emailAddress")?.Value ?? throw new ApiException("Email not found.");

            User newUser = _mapper.Map<User>(request);
            newUser.CreatedAt = DateTime.Now;
            newUser.UpdatedAt = DateTime.Now;
            newUser.Status = UserStatus.Active;

            var result = await _userManager.CreateAsync(newUser);

            if (!result.Succeeded)
            {
                throw new ApiException($"{result.Errors.FirstOrDefault()?.Description}");
            }

            IdentityResult roleResult = await _userManager.AddToRoleAsync(newUser, request.Role);

            if (!roleResult.Succeeded)
            {
                // Roll back user creation and throw an error
                await _userManager.DeleteAsync(newUser);
                throw new ApiException($"An error occured while adding the user to the role");
            }

            string token = await _userManager.GeneratePasswordResetTokenAsync(newUser);

            var resetUserRequest = new
            {
                UserName = newUser.UserName!,
                Token = token
            };
            IDictionary<string, string?> param = resetUserRequest.ToDictionary();

            Uri url = new(QueryHelpers.AddQueryString(_externalApiOptions.PasswordResetUrl, param));

            string userName = newUser.UserName!;
            string firstName = newUser.Name;
            string userEmail = newUser.Email!;

            await _notificationService.SendPasswordResetToken(userName, url.ToString(), firstName, userEmail);

            return new Response<string>(newUser.Id, message: $"Successfully registered user with username - {request.UserName}");
        }

        public async Task<Response<string>> EditUserAsync(EditUserRequest request, CancellationToken cancellationToken)
        {
            User user = await _userManager.FindByNameAsync(request.UserName) ?? throw new ApiException("Username could not be found.");

            // Check if the new role exists
            if (request.Role is not null && !await _roleManager.RoleExistsAsync(request.Role))
            {
                throw new ApiException($"This role doesn't exists. Please check your roles and try again");
            }

            string username = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "username")?.Value ?? throw new ApiException("Username not found.");
            string email = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "emailAddress")?.Value ?? throw new ApiException("Email not found.");

            user.Name = string.IsNullOrEmpty(request.Name) ? user.Name : request.Name;
            user.Email = string.IsNullOrEmpty(request.Email) ? user.Email : request.Email;
            user.UpdatedAt = DateTime.Now;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                throw new ApiException(updateResult.Errors.FirstOrDefault()?.Description ?? "An error occured while updating users.");
            }

            if (request.Role is not null)
            {
                IList<string> existingRoles = await _userManager.GetRolesAsync(user);

                if (!existingRoles.Contains(request.Role))
                {
                    // If there are existing roles then delete them
                    if (existingRoles.Count > 0)
                    {
                        IdentityResult roleResult = await _userManager.RemoveFromRolesAsync(user, existingRoles);

                        if (!roleResult.Succeeded)
                        {
                            throw new ApiException(roleResult.Errors.FirstOrDefault()?.Description ?? "An error occured while removing existing roles.");
                        }
                    }

                    IdentityResult addRoleResult = await _userManager.AddToRoleAsync(user, request.Role);

                    if (!addRoleResult.Succeeded)
                    {
                        throw new ApiException(addRoleResult.Errors.FirstOrDefault()?.Description ?? "An error occured while adding new role.");
                    }
                }
            }

            return new Response<string>(user.Id, message: $"Successfully edited user.");
        }

        public async Task<Response<string>> DeleteUserAsync(DeleteUserRequest request, CancellationToken cancellationToken)
        {
            User user = await _userManager.FindByNameAsync(request.UserName) ?? throw new ApiException($"The user does not exist.");

            // Work on the request logging
            string username = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "username")?.Value ?? throw new ApiException("Username not found.");
            string email = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == "emailAddress")?.Value ?? throw new ApiException("Email not found.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new ApiException(result.Errors.FirstOrDefault()?.Description ?? "An error occured.");
            }

            return new Response<string>(user.Id, message: $"Successfully deleted the user.");
        }

        public async Task<Response<string>> ResetUserAsync(ResetUserRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                // This is a security measure to prevent a bad actor from knowing the list of users on the platform.
                return new Response<string>("", "Reset user successful.");
            }

            string userName = user.UserName!;
            string userFirstName = user.Name;
            string userEmail = user.Email!;
            string token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetUserRequest = new
            {
                UserName = userName,
                Token = token
            };
            IDictionary<string, string?> param = resetUserRequest.ToDictionary();

            Uri url = new(QueryHelpers.AddQueryString(_externalApiOptions.PasswordResetUrl, param));

            await _notificationService.SendPasswordResetToken(userName, url.ToString(), userFirstName, userEmail);

            return new Response<string>("", "Reset user successful.");
        }

        public async Task<Response<string>> ResetUserLockoutAsync(ResetUserRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                throw new ApiException($"Username '{request.UserName}' could not be found.");
            }

            var resetResult = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now);

            if (!resetResult.Succeeded)
            {
                _logger.LogError("An error occured while resetting lockout");
                _logger.LogError(CoreHelpers.ClassToJsonData(resetResult.Errors));
                throw new ApiException($"An error occured while resetting lockout.");
            }

            return new Response<string>(user.Id, message: "Successfully reset user lockout.");
        }

        public async Task<Response<string>> ChangePasswordWithToken(ChangePasswordRequest request)
        {
            User user = await _userManager.FindByNameAsync(request.UserName) ?? throw new ApiException($"No user found with username '{request.UserName}'.");

            IdentityResult resetResponse = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);

            if (!resetResponse.Succeeded)
            {
                throw new ApiException(resetResponse.Errors.FirstOrDefault()?.Description ?? "An error occured.");
            }

            return new Response<string>(user.Id, message: "Successfully changed password.");
        }
    }
}
