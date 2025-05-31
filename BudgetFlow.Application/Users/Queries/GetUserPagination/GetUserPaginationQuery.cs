using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Users.Queries.GetUserPagination;
public class GetUserPaginationQuery : IRequest<Result<PaginatedList<UserPaginationResponse>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string Search { get; set; }
    public bool? IsEmailConfirmed { get; set; }

    public class GetUserPaginationQueryHandler : IRequestHandler<GetUserPaginationQuery, Result<PaginatedList<UserPaginationResponse>>>
    {
        private readonly IUserRepository _userRepository;
        public GetUserPaginationQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<Result<PaginatedList<UserPaginationResponse>>> Handle(GetUserPaginationQuery request, CancellationToken cancellationToken)
        {
            var result = await _userRepository.GetPaginatedAsync(request.Page, request.PageSize, request.Search, request.IsEmailConfirmed);
            if (result == null)
                return Result.Failure<PaginatedList<UserPaginationResponse>>(UserErrors.LogNotFound);

            return Result.Success(result);
        }
    }
}
