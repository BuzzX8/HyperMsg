using System;
using System.Collections.Generic;

namespace HyperMsg
{
    internal class DataRepositoryService : MessagingService
    {
        private IDataRepository dataRepository;

        public DataRepositoryService(IMessagingContext messagingContext, IDataRepository dataRepository) : base(messagingContext) =>
            this.dataRepository = dataRepository;

        protected override IEnumerable<IDisposable> GetAutoDisposables()
        {
            yield return this.RegisterRequestHandler(() => dataRepository);
        }
    }
}
