using System;
using System.Threading.Tasks;

using AutoMapper;
using MassTransit;

using MessageBus.Contracts.Events.Worker;
using FixtureDtoMsg = MessageBus.Contracts.Common.Dto.FixtureDto;

using Worker.Application.Common.Interfaces;
using Worker.Application.Jobs.OneOff.FootballDataCollection.Dto;

namespace Worker.Infrastructure.Livescore {
    public class FixtureLivescoreNotifier : IFixtureLivescoreNotifier {
        private readonly IBus _bus;
        private readonly IMapper _mapper;

        public FixtureLivescoreNotifier(IBus bus, IMapper mapper) {
            _bus = bus;
            _mapper = mapper;
        }

        public async Task NotifyFixtureActivated(long fixtureId, long teamId) {
            await _bus.Publish(new FixtureActivated {
                CorrelationId = Guid.NewGuid(),
                FixtureId = fixtureId,
                TeamId = teamId
            });
        }

        public async Task NotifyFixturePrematchUpdated(
            long fixtureId, long teamId, FixtureDto fixture
        ) {
            await _bus.Publish(new FixturePrematchUpdated {
                CorrelationId = Guid.NewGuid(),
                FixtureId = fixtureId,
                TeamId = teamId,
                Fixture = _mapper.Map<FixtureDto, FixtureDtoMsg>(fixture)
            });
        }

        public async Task NotifyFixtureLiveUpdated(
            long fixtureId, long teamId, FixtureDto fixture
        ) {
            await _bus.Publish(new FixtureLiveUpdated {
                CorrelationId = Guid.NewGuid(),
                FixtureId = fixtureId,
                TeamId = teamId,
                Fixture = _mapper.Map<FixtureDto, FixtureDtoMsg>(fixture)
            });
        }
    }
}
