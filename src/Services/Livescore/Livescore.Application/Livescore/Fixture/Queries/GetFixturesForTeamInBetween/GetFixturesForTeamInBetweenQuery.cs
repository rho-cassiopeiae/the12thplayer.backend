﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Livescore.Application.Common.Interfaces;
using Livescore.Application.Common.Results;
using Livescore.Application.Livescore.Fixture.Common.Dto;

namespace Livescore.Application.Livescore.Fixture.Queries.GetFixturesForTeamInBetween {
    public class GetFixturesForTeamInBetweenQuery : IRequest<
        HandleResult<IEnumerable<FixtureSummaryDto>>
    > {
        public long TeamId { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
    }

    public class GetFixturesForTeamInBetweenQueryHandler : IRequestHandler<
        GetFixturesForTeamInBetweenQuery, HandleResult<IEnumerable<FixtureSummaryDto>>
    > {
        private readonly ILivescoreQueryable _livescoreQueryable;

        public GetFixturesForTeamInBetweenQueryHandler(ILivescoreQueryable livescoreQueryable) {
            _livescoreQueryable = livescoreQueryable;
        }

        public async Task<HandleResult<IEnumerable<FixtureSummaryDto>>> Handle(
            GetFixturesForTeamInBetweenQuery query, CancellationToken cancellationToken
        ) {
            var fixtures = await _livescoreQueryable.GetFixturesForTeamInBetween(
                query.TeamId, query.StartTime, query.EndTime
            );

            return new HandleResult<IEnumerable<FixtureSummaryDto>> {
                Data = fixtures
            };
        }
    }
}
