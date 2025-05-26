using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Categories.Commands.UpdateCategory;
public class UpdateCategoryCommand : IRequest<Result<bool>>
{
    public int ID { get; set; }
    public string Color { get; set; }

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<bool>>
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICurrentUserService _currentUserService;

        public UpdateCategoryCommandHandler(
            ICategoryRepository categoryRepository,
            ICurrentUserService currentUserService)
        {
            _categoryRepository = categoryRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<bool>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var userID = _currentUserService.GetCurrentUserID();
            var result = await _categoryRepository.UpdateCategoryAsync(request.ID, request.Color, userID);
            return result
                ? Result.Success(true)
                : Result.Failure<bool>(CategoryErrors.UpdateFailed);
        }
    }
}
