using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Linq;
using System.Threading;
using Xunit;

namespace HyperMsg
{
	public class PipeReaderListenerTests
    {
		private readonly ManualResetEventSlim @event;
		private readonly Pipe pipe;
	    private readonly TimeSpan waitTimeout;

		public PipeReaderListenerTests()
		{
			@event = new ManualResetEventSlim();
			pipe = new Pipe();
			waitTimeout = TimeSpan.FromSeconds(1);
		}

        [Fact]
        public void Provides_Received_Buffer_To_Buffer_Reader()
        {            
            var expected = Guid.NewGuid().ToByteArray();
            var actual = (byte[])null;
            var listener = new PipeReaderListener(pipe.Reader, b =>
            {
                actual = b.First.ToArray();
                return 0;
            });
			listener.BufferReaded += (s, e) => @event.Set();
			listener.Start();

			WriteAndWaitEvent(expected);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Advances_Pipe_Reader_If_Byte_Reader_Returned_Value_Greater_Than_Zero()
        {
			var expected = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
			var actual = (byte[])null;
			var bytesToRead = 4;
	        var listener = new PipeReaderListener(pipe.Reader, b =>
	        {
				actual = b.First.Slice(0, bytesToRead).ToArray();		        
		        return bytesToRead;
	        });
			listener.BufferReaded += (s, e) => @event.Set();
			listener.Start();

			WriteAndWaitEvent(expected);
			@event.Reset();
			WriteAndWaitEvent(Array.Empty<byte>());

			Assert.Equal(expected.Skip(bytesToRead).Take(bytesToRead), actual);
		}

		[Fact]
		public void Stops_Listening_When_Writer_Completes()
		{
			var listener = new PipeReaderListener(pipe.Reader, b => 0);
			listener.Stopped += (s, e) => @event.Set();
			listener.Start();

			pipe.Writer.Complete();
			@event.Wait(waitTimeout);

			Assert.False(listener.IsListening);
		}

	    [Fact]
	    public void Stop_Stops_Invoking_Buffer_Reader()
	    {
		    var readerCalled = false;
		    var listener = new PipeReaderListener(pipe.Reader, b =>
		    {
			    readerCalled = true;
			    return 0;
		    });
		    listener.Stopped += (s, e) => @event.Set();
		    listener.Start();

			listener.Stop();
		    @event.Wait(waitTimeout);
		    
			Assert.False(readerCalled);
			Assert.False(listener.IsListening);
	    }

	    [Fact]
	    public void Rises_Error_When_Unhandled_Exception_Occurs()
	    {
			var listener = new PipeReaderListener(pipe.Reader, b => throw new ApplicationException());
		    var wasCalled = false;
		    listener.Error += (s, e) =>
		    {
			    wasCalled = true;
			    @event.Reset();
		    };
		    listener.Stopped += (s, e) => @event.Set();
			listener.Start();

			WriteAndWaitEvent(Guid.NewGuid().ToByteArray());

			Assert.True(wasCalled);
			Assert.False(listener.IsListening);
	    }

		private void WriteAndWaitEvent(byte[] data)
		{
			pipe.Writer.Write(data);
			pipe.Writer.FlushAsync().AsTask().Wait();
			@event.Wait(waitTimeout);
		}
    }
}
