using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

namespace BudgetFlow.Application.Category.Commands.UpdateCategory
{
    public class UpdateCategoryCommand : IRequest<bool>
    {
        public int ID { get; set; }
        public string Color { get; set; }

        public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, bool>
        {
            private readonly ICategoryRepository categoryRepository;

            public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
            {
                this.categoryRepository = categoryRepository;
            }

            public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
            {
                var result = await categoryRepository.UpdateCategoryAsync(request.ID, request.Color);
                return result;
            }
        }
    }
}
