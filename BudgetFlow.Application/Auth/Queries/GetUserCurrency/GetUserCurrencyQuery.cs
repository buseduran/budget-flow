using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Auth.Queries.GetUserCurrency;
public class GetUserCurrencyQuery : IRequest<Result<CurrencyType>>
{
    public class GetUserCurrencyQueryHandler : IRequestHandler<GetUserCurrencyQuery, Result<CurrencyType>>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IUserRepository userRepository;
        public GetUserCurrencyQueryHandler(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userRepository = userRepository;
        }

        public async Task<Result<CurrencyType>> Handle(GetUserCurrencyQuery request, CancellationToken cancellationToken)
        {
            GetCurrentUser getCurrentUser = new(httpContextAccessor);
            int userID = getCurrentUser.GetCurrentUserID();

            var result = await userRepository.GetUserCurrencyAsync(userID);
            if (result != null)
            {
                return Result.Success(result);
            }
            return Result.Success(CurrencyType.USD);
        }
    }
}
