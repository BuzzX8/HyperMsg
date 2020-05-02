using System;

namespace HyperMsg
{
    public interface ITaskCompletionSource
    {
        void SetCompleted();

        void SetException(Exception exception);

        void SetCancelled();
    }

    public interface ITaskCompletionSource<T>
    {
        void SetResult(T result);

        void SetException(Exception exception);

        void SetCancelled();
    }
}
