﻿using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Core.ServiceBus.TestSupport
{
    public class TransportCleanupActivator : IActivator
    {
        private readonly TransportCleanup _cleanup;

        public TransportCleanupActivator(TransportCleanup cleanup)
        {
            _cleanup = cleanup;
        }

        public void Activate(IActivationLog log)
        {
            log.Trace("Adding TransportCleanup to the Bottles EventAggregator");
            FubuMVC.Core.Services.Messaging.EventAggregator.Messaging.AddListener(_cleanup);
        }
    }
}