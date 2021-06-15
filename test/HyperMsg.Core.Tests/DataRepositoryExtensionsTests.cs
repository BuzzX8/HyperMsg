using System;
using Xunit;

namespace HyperMsg
{
    public class DataRepositoryExtensionsTests : ServiceHostFixture
    {
        private readonly IDataRepository settings;

        public DataRepositoryExtensionsTests() => settings = GetRequiredService<IDataRepository>();

        [Fact]
        public void Get_Returns_Value_Provided_With_Set()
        {
            var value = Guid.NewGuid();

            settings.Set(value);

            var actualValue = settings.Get<Guid>();

            Assert.Equal(value, actualValue);
        }

        [Fact]
        public void TryGet_Returns_False_For_Not_Existing_Value()
        {
            Assert.False(settings.TryGet<string>(Guid.NewGuid().ToString(), out _));
        }
    }
}
