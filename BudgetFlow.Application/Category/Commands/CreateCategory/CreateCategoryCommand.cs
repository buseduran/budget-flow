using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Domain.Entities;
using MediatR;

namespace BudgetFlow.Application.Category.Commands.CreateCategory
{
    public class CreateCategoryCommand : IRequest<bool>
    {
        public string Name { get; set; }
        public string Color { get; set; }

        public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, bool>
        {
            private readonly ICategoryRepository categoryRepository;
            public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
            {
                this.categoryRepository = categoryRepository;
            }
            public async Task<bool> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
            {
                CategoryDto categoryDto = new CategoryDto
                {
                    Name = request.Name,
                    Color = request.Color
                };
                var response= await categoryRepository.CreateCategoryAsync(categoryDto);

                return response;
            }
        }

    }
}
