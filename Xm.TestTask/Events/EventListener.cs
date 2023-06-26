using Xm.TestTask.Interfaces;

namespace Xm.TestTask.Events
{
    public class EventListener
    {
        private readonly IEventBus _eventBus;
        private readonly IMediator _mediator;

        public EventListener(IEventBus eventBus, IMediator mediator)
        {
            _eventBus = eventBus;
            _mediator = mediator;
        }

        public void StartListening()
        {
            Task.Run(async () =>
            {
                await foreach (var @event in _eventBus.ListenAsync())
                {
                    await _mediator.Dispatch(@event.DataType, @event.Body);
                }
            });
        }
    }
}
