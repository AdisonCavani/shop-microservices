using Gateway.Repositories;
using Grpc.Core;

namespace Gateway.Endpoints;

public class IdentityGrpcService(IUserRepository userRepository) : IdentityAPI.IdentityAPIBase
{
    public override async Task<UserDto?> GetUser(GetUserReq request, ServerCallContext context)
    {
        return await userRepository.FindUserByIdAsync(Guid.Parse(request.Id));
    }
}