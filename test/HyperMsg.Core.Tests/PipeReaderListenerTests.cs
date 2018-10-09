using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using Xunit;

namespace HyperMsg
{
    public class PipeReaderListenerTests
    {
        [Fact]
        public void Provides_Received_Bytes_To_Byte_Reader()
        {
            var pipe = new Pipe();
            var expected = Guid.NewGuid().ToByteArray();
            var actual = (byte[])null;
            var @event = new ManualResetEventSlim();
            var listener = new PipeReaderListener(pipe.Reader, b =>
            {
                actual = b.First.ToArray();
                @event.Set();
                return 0;                
            });
            listener.Start();

            pipe.Writer.Write(expected);
            pipe.Writer.FlushAsync().AsTask().Wait();
            @event.Wait();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Advances_Pipe_Reader_If_Byte_Reader_Returned_Value_Greater_Than_Zero()
        {

        }
    }
}
