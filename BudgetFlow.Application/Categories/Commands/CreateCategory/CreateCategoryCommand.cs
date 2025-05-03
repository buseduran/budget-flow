using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Categories.Commands.CreateCategory;
public class CreateCategoryCommand : IRequest<Result<int>>
{
    public string Name { get; set; }
    public string Color { get; set; }
    public EntryType Type { get; set; }

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<int>>
    {
        private readonly ICategoryRepository categoryRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.categoryRepository = categoryRepository;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<Result<int>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            GetCurrentUser getCurrentUser = new(httpContextAccessor);
            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Color))
                return Result.Failure<int>(CategoryErrors.NameOrColorCannotBeEmpty);

            Category categoryDto = new Category
            {
                Name = request.Name,
                Color = request.Color,
                UserID = getCurrentUser.GetCurrentUserID(),
                Type = request.Type
            };
            var response = await categoryRepository.CreateCategoryAsync(categoryDto);
            if (response == 0)
                return Result.Failure<int>(CategoryErrors.CreationFailed);

            return Result.Success(response);
        }
    }
}
