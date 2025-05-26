using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Users.Queries.GetUserPagination;
public class GetUserPaginationQuery : IRequest<Result<PaginatedList<UserResponse>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public class GetUserPaginationQueryHandler : IRequestHandler<GetUserPaginationQuery, Result<PaginatedList<UserResponse>>>
    {
        private readonly IUserRepository _userRepository;
        public GetUserPaginationQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<Result<PaginatedList<UserResponse>>> Handle(GetUserPaginationQuery request, CancellationToken cancellationToken)
        {
            var result = await _userRepository.GetPaginatedAsync(request.Page, request.PageSize);
            if (result == null)
                return Result.Failure<PaginatedList<UserResponse>>(UserErrors.LogNotFound);

            return Result.Success(result);
        }
    }
}
