using FakeItEasy;
using System;
using System.ComponentModel;
using System.Linq;
using Xunit;

namespace HyperMsg
{
    public class MessagingObjectTests
    {
        private readonly MessageBroker messageBroker;
        private readonly MessagingObjectImpl messagingObject;

        public MessagingObjectTests()
        {
            messageBroker = new();
            messagingObject = new(messageBroker);
        }

        [Fact]
        public void Dispose_Disposes_Registration_Added_By_AddRegistration()
        {
            var registration = A.Fake<IDisposable>();

            messagingObject.AddRegistration(registration);
            messagingObject.Dispose();

            A.CallTo(() => registration.Dispose()).MustHaveHappened();
        }

        [Fact]
        public void Dispose_Disposes_Registrations_Added_By_AddRegistrations()
        {
            var registrations = A.CollectionOfFake<IDisposable>(10).ToArray();

            messagingObject.AddRegistrations(registrations);
            messagingObject.Dispose();

            foreach (var registration in registrations)
            {
                A.CallTo(() => registration.Dispose()).MustHaveHappened();
            }
        }

        [Fact]
        public void SetProperty_Rises_PropertyChanged_Event()
        {
            var args = default(PropertyChangedEventArgs);

            messagingObject.PropertyChanged += (_, a) => args = a;
            messagingObject.Property = Guid.NewGuid().ToString();

            Assert.NotNull(args);
            Assert.Equal(nameof(MessagingObjectImpl.Property), args.PropertyName);
        }
    }

    internal class MessagingObjectImpl : MessagingObject
    {        
        private string stringValue;

        public MessagingObjectImpl(IMessagingContext messagingContext) : base(messagingContext)
        { }

        public new void AddRegistration(IDisposable registration) => base.AddRegistration(registration);

        public new void AddRegistrations(params IDisposable[] registrations) => base.AddRegistrations(registrations);

        public string Property
        {
            get => stringValue;
            set => SetProperty(ref stringValue, value);
        }
    }
}
