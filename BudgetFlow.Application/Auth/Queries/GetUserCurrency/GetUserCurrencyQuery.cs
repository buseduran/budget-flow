using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Auth.Queries.GetUserCurrency;
public class GetUserCurrencyQuery : IRequest<Result<CurrencyType>>
{
    public class GetUserCurrencyQueryHandler(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository) : IRequestHandler<GetUserCurrencyQuery, Result<CurrencyType>>
    {
        private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
        private readonly IUserRepository userRepository = userRepository;

        public async Task<Result<CurrencyType>> Handle(GetUserCurrencyQuery request, CancellationToken cancellationToken)
        {
            GetCurrentUser getCurrentUser = new(httpContextAccessor);
            int userID = getCurrentUser.GetCurrentUserID();

            var result = await userRepository.GetUserCurrencyAsync(userID);
            if (Enum.IsDefined(typeof(CurrencyType), result))
            {
                return Result.Success(result);
            }
            return Result.Success(CurrencyType.USD);
        }
    }
}
