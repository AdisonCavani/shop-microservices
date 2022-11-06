using System.Security.Claims;
using AutoMapper;
using HotChocolate.AspNetCore.Authorization;
using Server.Database;
using Microsoft.EntityFrameworkCore;
using Server.Contracts.Dtos;

namespace Server.Resolvers.Queries;

[ExtendObjectType(typeof(Query))]
public class UserQuery
{
    [Authorize]
    public async Task<UserDto> Me(
        [Service] IMapper mapper,
        [Service] AppDbContext context,
        [Service] IHttpContextAccessor accessor)
    {
        if (accessor.HttpContext is null)
            throw new GraphQLException(ExceptionMessages.HttpContextNull);

        var userIdStr = accessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userIdStr is null)
            throw new GraphQLException(ExceptionMessages.NameIdentifierNull);

        var userId = Guid.Parse(userIdStr);

        return mapper.Map<UserDto>(await context.Users.FirstOrDefaultAsync(x => x.Id == userId));
    }
}