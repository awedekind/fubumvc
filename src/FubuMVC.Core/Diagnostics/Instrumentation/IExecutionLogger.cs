﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FubuMVC.Core.Diagnostics.Instrumentation
{
    public interface IExecutionLogger
    {
        void Record(ChainExecutionLog log, IDictionary<string, object> http);
    }


    public class NulloExecutionLogger : IExecutionLogger
    {
        public void Record(ChainExecutionLog log, IDictionary<string, object> http)
        {
            // no-op
        }
    }

    public class VerboseExecutionLogger : IExecutionLogger
    {
        private readonly IExecutionLogStorage _storage;

        public VerboseExecutionLogger(IExecutionLogStorage storage)
        {
            _storage = storage;
        }

        public void Record(ChainExecutionLog log, IDictionary<string, object> http)
        {
            throw new NotImplementedException();
        }
    }

    public class ProductionExecutionLogger : IExecutionLogger
    {
        private readonly IExecutionLogStorage _storage;

        public ProductionExecutionLogger(IExecutionLogStorage storage)
        {
            _storage = storage;
        }

        public void Record(ChainExecutionLog log, IDictionary<string, object> http)
        {
            throw new NotImplementedException();
        }
    }

    public interface IExecutionLogStorage
    {
        void Store(ChainExecutionLog log);
        void Start();
    }


    public class PerformanceHistoryQueue : IExecutionLogStorage, IDisposable
    {
        private readonly BlockingCollection<ChainExecutionLog> _collection =
            new BlockingCollection<ChainExecutionLog>(new ConcurrentBag<ChainExecutionLog>());

        private Task _readingTask;



        public void Dispose()
        {
            _collection.CompleteAdding();
            _collection.Dispose();
        }


        public void Enqueue(ChainExecutionLog log)
        {
            _collection.Add(log);
        }

        private void record()
        {
            foreach (var request in _collection.GetConsumingEnumerable())
            {
                throw new Exception("Do something here.");
            }
        }

        public void Start()
        {
            _readingTask = Task.Factory.StartNew(record);
        }

        void IExecutionLogStorage.Store(ChainExecutionLog log)
        {
            Enqueue(log);
        }
    }
}