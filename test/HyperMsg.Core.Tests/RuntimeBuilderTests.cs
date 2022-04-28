using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HyperMsg
{
    public class RuntimeBuilderTests
    {
        public readonly RuntimeBuilder builder = new ();

        [Fact]
        public void AddService_Adds_Service_To_Runtime_ServiceProvider()
        {
            var service = A.Fake<Action>();
            builder.Services.AddSingleton(service);

            var runtime = builder.Build();

            Assert.Same(service, runtime.ServiceProvider.GetService(typeof(Action)));
        }

        [Fact]
        public void Build_Invokes_Initializers_Added_By_AddInitializer()
        {
            var initializer = A.Fake<Action<IRuntime>>();
            builder.AddInitializer(initializer);

            var runtime = builder.Build();

            A.CallTo(() => initializer.Invoke(runtime)).MustHaveHappened();
        }
    }
}
