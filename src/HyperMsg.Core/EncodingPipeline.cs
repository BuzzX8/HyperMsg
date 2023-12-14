namespace HyperMsg;

public static class EncodingPipeline
{
    public static Func<T, Result<Unit>> New<T>(Encoder<T> encoder, Func<Memory<byte>, Result<Unit>> bufferWriter, Func<Result<Memory<byte>>> bufferProvider)
    {
        return message => bufferProvider().Match(
            Succ: buffer => encoder(buffer, message).Match(
                Succ: bytesEncoded => bufferWriter(buffer[..bytesEncoded]),
                Fail: error => new Result<Unit>(error)),
            Fail: error => new Result<Unit>(error));
    }
}
