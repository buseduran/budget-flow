using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Categories.Commands.CreateCategory
{
    public class CreateCategoryCommand : IRequest<int>
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public EntryType Type { get; set; }

        public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, int>
        {
            private readonly ICategoryRepository categoryRepository;
            private readonly IHttpContextAccessor httpContextAccessor;
            public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IHttpContextAccessor httpContextAccessor)
            {
                this.categoryRepository = categoryRepository;
                this.httpContextAccessor = httpContextAccessor;
            }
            public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
            {
                GetCurrentUser getCurrentUser = new(httpContextAccessor);

                Category categoryDto = new Category
                {
                    Name = request.Name,
                    Color = request.Color,
                    UserID = getCurrentUser.GetCurrentUserID()
                };
                var response = await categoryRepository.CreateCategoryAsync(categoryDto);
                return response;
            }
        }
    }
}
