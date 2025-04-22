using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Auth.Commands.UpdateUserCurrency;
public class UpdateUserCurrencyCommand : IRequest<Result<CurrencyType>>
{
    public CurrencyType CurrencyType { get; set; }
    public class UpdateUserCurrencyCommandHandler : IRequestHandler<UpdateUserCurrencyCommand, Result<CurrencyType>>
    {
        private readonly IUserRepository userRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public UpdateUserCurrencyCommandHandler(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.userRepository = userRepository;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<Result<CurrencyType>> Handle(UpdateUserCurrencyCommand request, CancellationToken cancellationToken)
        {
            GetCurrentUser getCurrentUser = new(httpContextAccessor);
            int userID = getCurrentUser.GetCurrentUserID();
            var currency = await userRepository.UpdateUserCurrencyAsync(userID, request.CurrencyType);
            return Result.Success(currency);
        }
    }
}
