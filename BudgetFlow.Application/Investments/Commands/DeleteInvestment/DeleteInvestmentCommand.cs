using BudgetFlow.Application.Common.Interfaces.Repositories;
using MediatR;

namespace BudgetFlow.Application.Investments.Commands.DeleteInvestment
{
    public class DeleteInvestmentCommand : IRequest<bool>
    {
        public int ID { get; set; }
        public DeleteInvestmentCommand(int ID)
        {
            this.ID = ID;
        }
        public class DeleteInvestmentCommandHandler : IRequestHandler<DeleteInvestmentCommand, bool>
        {
            private readonly IInvestmentRepository investmentRepository;
            public DeleteInvestmentCommandHandler(IInvestmentRepository investmentRepository)
            {
                this.investmentRepository = investmentRepository;
            }
            public async Task<bool> Handle(DeleteInvestmentCommand request, CancellationToken cancellationToken)
            {
                var result = await investmentRepository.DeleteInvestmentAsync(request.ID);
                return result;
            }
        }
    }
}
