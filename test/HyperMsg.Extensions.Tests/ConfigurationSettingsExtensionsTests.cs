using FakeItEasy;
using System;
using System.Net;
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
            A.CallTo(() => settings.ContainsKey(key)).Returns(true);
            A.CallTo(() => settings[key]).Returns(expected);

            var actual = settings.Get<Guid>(key);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Get_Throws_Exception_If_Invalid_Value_Type_Provided()
        {
            var key = Guid.NewGuid().ToString();
            A.CallTo(() => settings.ContainsKey(key)).Returns(true);
            A.CallTo(() => settings[key]).Returns(Guid.NewGuid());

            Assert.Throws<InvalidOperationException>(() => settings.Get<string>(key));
        }

        [Fact]
        public void Get_Converts_Value_Into_Assignable_Type()
        {
            var settingName = nameof(IPEndPoint);
            var expectedSetting = new IPEndPoint(0, 0);
            A.CallTo(() => settings.ContainsKey(settingName)).Returns(true);
            A.CallTo(() => settings[settingName]).Returns(expectedSetting);

            var actualSetting = settings.Get<EndPoint>(settingName);

            Assert.Same(expectedSetting, actualSetting);
        }
    }
}
