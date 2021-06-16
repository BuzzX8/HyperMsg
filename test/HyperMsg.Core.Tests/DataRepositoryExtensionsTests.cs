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

        [Fact]
        public void SendDataRequest_Returns_Value_Stored_In_Repository()
        {
            var value = Guid.NewGuid();
            var key = value.ToString();

            repository.AddOrReplace(key, value);
            var actualValue = MessageSender.SendDataRequest<Guid>(key);

            Assert.Equal(value, actualValue);
        }

        [Fact]
        public void SendToDataRepository_Adds_Value_To_Data_Repository()
        {
            var value = Guid.NewGuid();
            var key = value.ToString();
                        
            MessageSender.SendToDataRepository(key, value);

            Assert.True(repository.Contains<Guid>(key));
            Assert.Equal(value, repository.Get<Guid>(key));
        }

        [Fact]
        public void SendDataExistenceRequest_Returns_True_Id_Data_Present_In_Repository()
        {
            var value = Guid.NewGuid();
            var key = value.ToString();

            repository.AddOrReplace(key, value);

            Assert.True(MessageSender.SendDataExistenceRequest<Guid>(key));
        }

        [Fact]
        public void SendDataDeletionRequest_Removes_Data_From_Repository()
        {
            var value = Guid.NewGuid();
            var key = value.ToString();
            repository.AddOrReplace(key, value);

            MessageSender.SendDataDeletionRequest<Guid>(key);

            Assert.False(repository.Contains<Guid>(key));
        }
    }
}
