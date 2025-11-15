namespace AlasdairCooper.Reference.Components.State;

public delegate ValueTask<T> StateLoader<in TParameters, T>(TParameters parameters, LoadingState state, CancellationToken cancellationToken);