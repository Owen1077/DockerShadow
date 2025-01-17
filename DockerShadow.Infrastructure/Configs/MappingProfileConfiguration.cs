using AutoMapper;
using DockerShadow.Core.DTO.Request;
using DockerShadow.Core.DTO.Response;
using DockerShadow.Domain.Entities;

namespace DockerShadow.Infrastructure.Configs
{
    public class MappingProfileConfiguration : Profile
    {
        public MappingProfileConfiguration()
        {
            CreateMap<User, AuthenticationResponse>(MemberList.None);
            CreateMap<User, UserResponse>(MemberList.None);
            CreateMap<AddUserRequest, User>(MemberList.None);
            CreateMap<BankToBrokerFundWalletLog, FundLogResponse>(MemberList.None);
            CreateMap<BankToBrokerWithdrawalLog, WithdrawLogResponse>(MemberList.None);
            CreateMap<BankToBrokerLog, AccountLogResponse>(MemberList.None);
        }
    }
}
