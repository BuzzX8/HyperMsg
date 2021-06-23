using System;
using Xunit;

namespace HyperMsg
{
    public class DataRepositoryExtensionsTests : ServiceHostFixture
    {
        private readonly IDataRepository repository;

        public DataRepositoryExtensionsTests() => repository = GetRequiredService<IDataRepository>();

        [Fact]
        public void Get_Returns_Value_Provided_With_Set()
        {
            var value = Guid.NewGuid();

            repository.AddOrReplace(value);

            var actualValue = repository.Get<Guid>();

            Assert.Equal(value, actualValue);
        }

        [Fact]
        public void TryGet_Returns_False_For_Not_Existing_Value()
        {
            Assert.False(repository.TryGet<string>(Guid.NewGuid().ToString(), out _));
        }
    }
}
