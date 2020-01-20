using FakeItEasy;
using System;
using Xunit;

namespace HyperMsg
{
    public class ConfigurationSettingsExtensionsTests
    {
        private readonly IConfigurationSettings settings = A.Fake<IConfigurationSettings>();

        [Fact]
        public void Get_Returns_Value_From_Settings()
        {
            var expected = Guid.NewGuid();
            var key = expected.ToString();
            A.CallTo(() => settings[key]).Returns(expected);

            var actual = settings.Get<Guid>(key);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Get_Throws_Exception_If_Invalid_Value_Type_Provided()
        {
            var key = Guid.NewGuid().ToString();
            A.CallTo(() => settings[key]).Returns(Guid.NewGuid());

            Assert.Throws<InvalidOperationException>(() => settings.Get<int>(key));
        }
    }
}
