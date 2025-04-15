using Gateway.Database;
using Gateway.Mappers;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Endpoints;

public class IdentityGrpcService(AppDbContext dbContext) : IdentityAPI.IdentityAPIBase
{
    public override async Task<UserDto?> GetUser(GetUserReq request, ServerCallContext context)
    {
        var userEntity = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == Guid.Parse(request.Id));
        return userEntity?.ToUserDto();
    }
}