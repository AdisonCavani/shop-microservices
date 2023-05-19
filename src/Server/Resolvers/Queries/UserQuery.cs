using System.Security.Claims;
using HotChocolate.AspNetCore.Authorization;
using Server.Contracts.Dtos;
using Server.Repositories;

namespace Server.Resolvers.Queries;

[ExtendObjectType(typeof(Query))]
public class UserQuery
{
    [Authorize]
    public async Task<UserDto> Me(
        [Service] IUserRepository repository,
        [Service] IHttpContextAccessor accessor)
    {
        if (accessor.HttpContext is null)
            throw new GraphQLException(ExceptionMessages.HttpContextNull);

        var userIdStr = accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdStr is null)
            throw new GraphQLException(ExceptionMessages.NameIdentifierNull);

        var userId = Guid.Parse(userIdStr);

        return await repository.GetUserByIdAsync(userId);
    }
}