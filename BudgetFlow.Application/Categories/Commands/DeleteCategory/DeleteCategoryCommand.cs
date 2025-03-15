using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

namespace BudgetFlow.Application.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryCommand : IRequest<bool>
    {
        public int ID { get; set; }
        public DeleteCategoryCommand(int ID)
        {
            this.ID = ID;
        }
        public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
        {
            private readonly ICategoryRepository categoryRepository;
            public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
            {
                this.categoryRepository = categoryRepository;
            }
            public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
            {
                return await categoryRepository.DeleteCategoryAsync(request.ID);
            }
        }
    }
}
