using AutoMapper;
using BudgetFlow.Application.Common.Dtos;
using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

namespace BudgetFlow.Application.Investments.Commands.UpdateInvestment
{
    public class UpdateInvestmentCommand : IRequest<bool>
    {
        public int ID { get; set; }
        public InvestmentDto Investment { get; set; }
        public class UpdateInvestmentCommandHandler : IRequestHandler<UpdateInvestmentCommand, bool>
        {
            private readonly IInvestmentRepository repository;
            private readonly IMapper mapper;
            public UpdateInvestmentCommandHandler(IInvestmentRepository repository, IMapper mapper)
            {
                this.repository = repository;
                this.mapper = mapper;
            }
            public async Task<bool> Handle(UpdateInvestmentCommand request, CancellationToken cancellationToken)
            {
                var result = await repository.UpdateInvestmentAsync(request.ID, request.Investment);
                return result;
            }
        }
    }
}
