using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace HyperMsg
{
    public class StreamObservableTests
    {
        [Fact]
        public void Provides_Received_Bytes_To_Subscriber()
        {
            var bytes = Guid.NewGuid().ToByteArray();
            var stream = new MemoryStream();
            var buffer = new Memory<byte>(new byte[100]);
            var observable = new StreamObservable(stream, () => buffer);
            var actual = (byte[])null;
            observable.Subscribe(b =>
            {
                actual = b.ToArray();
            });
            observable.Start();

            stream.Write(bytes, 0, bytes.Length);

            Assert.Equal(bytes, actual);
        }
    }
}
