﻿using BudgetFlow.Application.Common.Interfaces.Repositories;
using BudgetFlow.Application.Common.Utils;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace BudgetFlow.Application.Budget.Queries.GetLastEntries
{
    public class GetLastEntriesQuery : IRequest<List<LastEntryResponse>>
    {
        public class GetLastFiveEntriesQueryHandler : IRequestHandler<GetLastEntriesQuery, List<LastEntryResponse>>
        {
            private readonly IBudgetRepository _budgetRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;
            public GetLastFiveEntriesQueryHandler(IBudgetRepository budgetRepository, IHttpContextAccessor httpContextAccessor)
            {
                _budgetRepository = budgetRepository;
                _httpContextAccessor = httpContextAccessor;
            }
            public async Task<List<LastEntryResponse>> Handle(GetLastEntriesQuery request, CancellationToken cancellationToken)
            {
                GetCurrentUser getCurrentUser = new(_httpContextAccessor);
                int userID = getCurrentUser.GetCurrentUserID();
                return await _budgetRepository.GetLastFiveEntriesAsync(userID);
            }
        }
    }
}
