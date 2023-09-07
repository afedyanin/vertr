namespace Vertr.Exchange.Infrastructure;

public interface IRequestAwaitingService
{
    Task<AwaitingResponse> Register(long orderId, CancellationToken cancellationToken = default);

    Task<long[]> GetAwatingRequests();

    void Complete(AwaitingResponse response);
}
