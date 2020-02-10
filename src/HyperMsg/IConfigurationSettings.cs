using System.Collections.Generic;

namespace HyperMsg
{
    /// <summary>
    /// Represents dictionary for storing configuration values.
    /// </summary>
    public interface IConfigurationSettings : IReadOnlyDictionary<string, object>
    { }
}
