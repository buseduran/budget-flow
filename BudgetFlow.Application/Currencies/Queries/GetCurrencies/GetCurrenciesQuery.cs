using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Results;
using BudgetFlow.Application.Currencies;
using BudgetFlow.Domain.Enums;
using MediatR;

namespace BudgetFlow.Application.Currencies.Queries.GetCurrencies;
public class GetCurrenciesQuery : IRequest<Result<List<CurrencyResponse>>>
{
    public class GetCurrenciesQueryHandler : IRequestHandler<GetCurrenciesQuery, Result<List<CurrencyResponse>>>
    {
        private readonly ICurrencyRateRepository _currencyRateRepository;

        public GetCurrenciesQueryHandler(ICurrencyRateRepository currencyRateRepository)
        {
            _currencyRateRepository = currencyRateRepository;
        }

        public async Task<Result<List<CurrencyResponse>>> Handle(GetCurrenciesQuery request, CancellationToken cancellationToken)
        {
            var currencies = new List<CurrencyResponse>();

            foreach (CurrencyType currencyType in Enum.GetValues(typeof(CurrencyType)))
            {
                var rate = await _currencyRateRepository.GetCurrencyRateByType(currencyType);
                if (rate != null)
                {
                    currencies.Add(new CurrencyResponse
                    {
                        CurrencyType = rate.CurrencyType,
                        ForexBuying = rate.ForexBuying,
                        ForexSelling = rate.ForexSelling,
                        RetrievedAt = rate.RetrievedAt
                    });
                }
            }

            return Result.Success(currencies);
        }
    }
}