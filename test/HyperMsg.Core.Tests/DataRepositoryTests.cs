using System;
using Xunit;

namespace HyperMsg
{
    public class DataRepositoryTests : ServiceHostFixture
    {
        private readonly IDataRepository repository;

        public DataRepositoryTests() => repository = GetRequiredService<IDataRepository>();

        [Fact]
        public void Get_Returns_Value_Provided_With_Set()
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid();

            Assert.False(repository.Contains<Guid>(key));
            repository.AddOrUpdate(key, value);
            Assert.True(repository.Contains<Guid>(key));

            var actualValue = repository.Get<Guid>(key);

            Assert.Equal(value, actualValue);
        }

        [Fact]
        public void Get_Returns_Default_Value_If_Incorrect_Key_Provided()
        {
            var value = (int)Guid.NewGuid().ToByteArray()[0];

            repository.AddOrUpdate(Guid.NewGuid().ToString(), value);
            var actualValue = repository.Get<int>(Guid.NewGuid().ToString());

            Assert.Equal(default, actualValue);
        }

        [Fact]
        public void Get_Returns_Default_Value_If_Incorrect_Type_Provided()
        {
            var key = Guid.NewGuid().ToString();
            var value = (int)Guid.NewGuid().ToByteArray()[0];

            repository.AddOrUpdate(key, value);
            var actualValue = repository.Get<string>(key);

            Assert.Equal(default, actualValue);
        }

        [Fact]
        public void Remove_Removes_Value()
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid();

            repository.AddOrUpdate(key, value);
            Assert.True(repository.Contains<Guid>(key));

            repository.Remove<Guid>(key);
            Assert.False(repository.Contains<Guid>(key));
        }
    }    
}
