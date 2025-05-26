using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Users.Queries.GetLogPagination;
public class GetLogPaginationQuery : IRequest<Result<PaginatedList<LogResponse>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public LogType LogType { get; set; } = LogType.All;
    public int UserID { get; set; }
}
public class GetLogPaginationQueryHandler : IRequestHandler<GetLogPaginationQuery, Result<PaginatedList<LogResponse>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetLogPaginationQueryHandler(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _httpContextAccessor = httpContextAccessor;
    }
    public async Task<Result<PaginatedList<LogResponse>>> Handle(GetLogPaginationQuery request, CancellationToken cancellationToken)
    {
        var result = await _userRepository.GetLogsPaginatedAsync(request.Page, request.PageSize, request.LogType, request.UserID);
        if (result == null)
            return Result.Failure<PaginatedList<LogResponse>>(UserErrors.LogNotFound);

        return Result.Success(result);
    }
}
