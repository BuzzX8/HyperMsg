using System;
using Xunit;

namespace HyperMsg
{
    public class SettingsTests : ServiceHostFixture
    {
        private readonly ISettings settings;

        public SettingsTests()
        {
            settings = GetRequiredService<ISettings>();
        }

        [Fact]
        public void Get_Returns_Value_Provided_With_Set()
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid();

            settings.Set(key, value);
            var actualValue = settings.Get<Guid>(key);

            Assert.Equal(value, actualValue);
        }
    }    
}
