using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HyperMsg.Messaging;

public class ServiceCollectionExtensionsTests
{
    private readonly ServiceCollection services;

    public ServiceCollectionExtensionsTests()
    {
        services = new ServiceCollection();
    }
}