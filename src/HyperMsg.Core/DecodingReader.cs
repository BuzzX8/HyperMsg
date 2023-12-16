namespace HyperMsg;

using BufferProvider = Func<Result<Memory<byte>>>;
using ByteWriter = Func<Memory<byte>, Result<int>>;
using AsyncByteWriter = Func<Memory<byte>, CancellationToken, ValueTask<Result<int>>>;

public static class DecodingReader
{
    public static Func<Result<T>> New<T>(Func<ReadOnlyMemory<byte>, Result<(T message, int bytesDecoded)>> decoder, ByteWriter byteWriter, Memory<byte> buffer)
    {
        return New(decoder, byteWriter, () => new(buffer));
    }

    public static Func<Result<T>> New<T>(Func<ReadOnlyMemory<byte>, Result<(T message, int bytesDecoded)>> decoder, ByteWriter byteWriter, BufferProvider bufferProvider)
    {
        return () => bufferProvider().Match(
            Succ: buffer => byteWriter(buffer).Match(
                Succ: bytesWritten => decoder(buffer[..bytesWritten]).Match(
                    Succ: r => new(r.message),
                    Fail: error => new Result<T>(error)),
                Fail: error => new Result<T>(error)),
            Fail: error => new Result<T>(error));
    }

    public static Func<CancellationToken, ValueTask<Result<T>>> NewAsync<T>(Func<ReadOnlyMemory<byte>, Result<(T message, int bytesDecoded)>> decoder, AsyncByteWriter byteWriter, Memory<byte> buffer)
    {
        return NewAsync(decoder, byteWriter, () => new Result<Memory<byte>>(buffer));
    }

    public static Func<CancellationToken, ValueTask<Result<T>>> NewAsync<T>(Func<ReadOnlyMemory<byte>, Result<(T message, int bytesDecoded)>> decoder, AsyncByteWriter byteWriter, BufferProvider bufferProvider)
    {
        return token => bufferProvider().Match(
            Succ: (Func<Memory<byte>, ValueTask<Result<T>>>)(async (buffer) => await (await byteWriter(buffer, token)).Match(
                Succ: bytesWritten => decoder(buffer[..bytesWritten]).Match(
                    Succ: r => ValueTask.FromResult(new Result<T>(r.message)),
                    Fail: error => ValueTask.FromResult(new Result<T>(error))),
                Fail: error => ValueTask.FromResult(new Result<T>(error)))),
            Fail: error => ValueTask.FromResult(new Result<T>(error)));
    }
}
