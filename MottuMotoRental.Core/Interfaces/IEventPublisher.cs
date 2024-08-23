namespace MottuMotoRental.Core.Interfaces
{
    public interface IEventPublisher
    {
        void Publish<T>(T @event);
    }
}