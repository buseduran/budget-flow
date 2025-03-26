using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Auth.Commands.UpdateUserCurrency
{
    public class UpdateUserCurrencyCommand : IRequest<CurrencyType>
    {
        public CurrencyType CurrencyType { get; set; }
        public class UpdateUserCurrencyCommandHandler : IRequestHandler<UpdateUserCurrencyCommand, CurrencyType>
        {
            private readonly IUserRepository userRepository;
            private readonly IHttpContextAccessor httpContextAccessor;
            public UpdateUserCurrencyCommandHandler(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
            {
                this.userRepository = userRepository;
                this.httpContextAccessor = httpContextAccessor;
            }
            public async Task<CurrencyType> Handle(UpdateUserCurrencyCommand request, CancellationToken cancellationToken)
            {
                GetCurrentUser getCurrentUser = new(httpContextAccessor);
                int userID = getCurrentUser.GetCurrentUserID();
                return await userRepository.UpdateUserCurrencyAsync(userID, request.CurrencyType);
            }
        }
    }
}
