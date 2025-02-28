using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using BudgetFlow.Domain.Entities;
using BudgetFlow.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Category.Commands.CreateCategory
{
    public class CreateCategoryCommand : IRequest<bool>
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public EntryType Type { get; set; }

        public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, bool>
        {
            private readonly ICategoryRepository categoryRepository;
            private readonly IHttpContextAccessor httpContextAccessor;
            public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IHttpContextAccessor httpContextAccessor)
            {
                this.categoryRepository = categoryRepository;
                this.httpContextAccessor = httpContextAccessor;
            }
            public async Task<bool> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
            {
                GetCurrentUser getCurrentUser = new(httpContextAccessor);

                CategoryDto categoryDto = new CategoryDto
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
