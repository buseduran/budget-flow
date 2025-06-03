using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Common.Services.Abstract;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using BudgetFlow.Domain.Errors;
using MediatR;

namespace BudgetFlow.Application.Categories.Commands.CreateCategory;
public class CreateCategoryCommand : IRequest<Result<int>>
{
    public string Name { get; set; }
    public string Color { get; set; }
    public EntryType Type { get; set; }
    public int WalletID { get; set; }

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<int>>
    {
        private readonly ICategoryRepository _categoryRepository;
        public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<Result<int>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Color))
                return Result.Failure<int>(CategoryErrors.NameOrColorCannotBeEmpty);

            Category categoryDto = new Category
            {
                Name = request.Name,
                Color = request.Color,
                WalletID = request.WalletID,
                Type = request.Type
            };
            var response = await _categoryRepository.CreateCategoryAsync(categoryDto);
            if (response == 0)
                return Result.Failure<int>(CategoryErrors.CreationFailed);

            return Result.Success(response);
        }
    }
}
