using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Interfaces.Services;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetFlow.Application.Users.Commands.UpdatePassword;
public class UpdatePasswordCommand:IRequest<Result<bool>>
{
    public string OldPassword { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, Result<bool>>
    {
        private readonly IUserRepository userRepository;
        private readonly IPasswordHasher passwordHasher;
        private readonly IHttpContextAccessor httpContextAccessor;
        public UpdatePasswordCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IHttpContextAccessor httpContextAccessor)
        {
            this.userRepository = userRepository;
            this.passwordHasher = passwordHasher;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<Result<bool>> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
        {
            var userID = new GetCurrentUser(httpContextAccessor).GetCurrentUserID();
            var user = await userRepository.GetByIdAsync(userID);
            if (user == null)
                return Result.Failure<bool>(UserErrors.UserNotFound);

            if (string.IsNullOrWhiteSpace(request.OldPassword))
                return Result.Failure<bool>(UserErrors.OldPasswordCannotBeEmpty);
            if (string.IsNullOrWhiteSpace(request.NewPassword))
                return Result.Failure<bool>(UserErrors.NewPasswordCannotBeEmpty);
            if (request.NewPassword != request.ConfirmNewPassword)
                return Result.Failure<bool>(UserErrors.PasswordsDoNotMatch);


            if (!passwordHasher.Verify(request.OldPassword, user.PasswordHash))
                return Result.Failure<bool>(UserErrors.InvalidOldPassword);


            var newPasswordHash = passwordHasher.Hash(request.NewPassword);
            var result = await userRepository.UpdateAsync(user.Name, user.Email, user.Email, newPasswordHash);
            if (!result)
                return Result.Failure<bool>(UserErrors.UpdateFailed);

            return Result.Success(true);
        }
    }
}
