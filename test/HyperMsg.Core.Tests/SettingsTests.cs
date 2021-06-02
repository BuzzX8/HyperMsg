using System;
using Xunit;

namespace HyperMsg
{
    public class SettingsTests : ServiceHostFixture
    {
        private readonly ISettings settings;

        public SettingsTests() => settings = GetRequiredService<ISettings>();

        [Fact]
        public void Get_Returns_Value_Provided_With_Set()
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid();

            settings.Set(key, value);
            var actualValue = settings.Get<Guid>(key);

            Assert.Equal(value, actualValue);
        }

        [Fact]
        public void Get_Returns_Default_Value_If_Incorrect_Key_Provided()
        {
            var value = (int)Guid.NewGuid().ToByteArray()[0];

            settings.Set(Guid.NewGuid().ToString(), value);
            var actualValue = settings.Get<int>(Guid.NewGuid().ToString());

            Assert.Equal(default, actualValue);
        }

        [Fact]
        public void Get_Returns_Default_Value_If_Incorrect_Type_Provided()
        {
            var key = Guid.NewGuid().ToString();
            var value = (int)Guid.NewGuid().ToByteArray()[0];

            settings.Set(key, value);
            var actualValue = settings.Get<string>(key);

            Assert.Equal(default, actualValue);
        }
    }    
}
