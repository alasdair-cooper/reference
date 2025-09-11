namespace AlasdairCooper.Reference.Api.Data.Entities.Ordering;

public enum DispatchState
{
    Pending,
    Picking,
    Packing,
    AwaitingCarrierPickup,
    AwaitingCarrierDelivery,
    Delivered
}