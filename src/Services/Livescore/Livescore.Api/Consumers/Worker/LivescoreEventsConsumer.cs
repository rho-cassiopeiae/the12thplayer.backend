using System.Threading.Tasks;

using AutoMapper;
using MassTransit;
using MediatR;

using MessageBus.Contracts.Events.Worker;
using FixtureDtoMsg = MessageBus.Contracts.Common.Dto.FixtureDto;

using Livescore.Application.Livescore.Commands.ActivateFixture;
using Livescore.Application.Livescore.Commands.UpdateFixturePrematch;
using Livescore.Application.Common.Dto;

namespace Livescore.Api.Consumers.Worker {
    public class LivescoreEventsConsumer :
        IConsumer<FixtureActivated>,
        IConsumer<FixturePrematchUpdated> {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public LivescoreEventsConsumer(ISender mediator, IMapper mapper) {
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<FixtureActivated> context) {
            var command = new ActivateFixtureCommand {
                FixtureId = context.Message.FixtureId,
                TeamId = context.Message.TeamId
            };

            await _mediator.Send(command);
        }

        public async Task Consume(ConsumeContext<FixturePrematchUpdated> context) {
            var command = new UpdateFixturePrematchCommand {
                FixtureId = context.Message.FixtureId,
                TeamId = context.Message.TeamId,
                Fixture = _mapper.Map<FixtureDtoMsg, FixtureDto>(context.Message.Fixture)
            };

            await _mediator.Send(command);
        }
    }
}
