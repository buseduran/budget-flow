using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using MediatR;

namespace BudgetFlow.Application.Wallets.Queries.GetWalletUsers;
public class GetWalletUsersQuery : IRequest<Result<List<GetWalletUsersResponse>>>
{
    public int WalletID { get; set; }
    public class GetWalletUsersQueryHandler : IRequestHandler<GetWalletUsersQuery, Result<List<GetWalletUsersResponse>>>
    {
        private readonly IUserWalletRepository _userWalletRepository;
        public GetWalletUsersQueryHandler(IUserWalletRepository userWalletRepository)
        {
            _userWalletRepository = userWalletRepository;
        }
        public async Task<Result<List<GetWalletUsersResponse>>> Handle(GetWalletUsersQuery request, CancellationToken cancellationToken)
        {
            var userWallets = await _userWalletRepository.GetUsersByWalletIdAsync(request.WalletID);
            var users = userWallets.Select(uw => new GetWalletUsersResponse
            {
                ID = uw.UserID,
                Name = uw.User.Name,
                Role = uw.Role
            }).ToList();
            return Result.Success(users);
        }
    }
}