namespace AlasdairCooper.Reference.Components.State;

public delegate ValueTask<T> StateLoader<in TParameters, T>(TParameters parameters, IProgressReporter reporter, CancellationToken cancellationToken);