using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Application.Investments;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetFlow.Application.Wallets.Queries.GetWalletPagination;
public class GetWalletPaginationQuery:IRequest<Result<PaginatedList<WalletResponse>>>
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public class GetWalletPaginationQueryHandler : IRequestHandler<GetWalletPaginationQuery, Result<PaginatedList<WalletResponse>>>
    {
        private readonly IWalletRepository walletRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public GetWalletPaginationQueryHandler(IWalletRepository walletRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.walletRepository = walletRepository;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<Result<PaginatedList<WalletResponse>>> Handle(GetWalletPaginationQuery request, CancellationToken cancellationToken)
        {
            int userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();
            var result = await walletRepository.GetWalletsAsync(request.Page, request.PageSize, userID);
            return result != null
                ? Result.Success(result)
                : Result.Failure<PaginatedList<WalletResponse>>(WalletErrors.WalletNotFound);
        }
    }
}
