using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EmployeeManagement.Api.Diagnostics
{
    public class EfQueryObserver : IObserver<DiagnosticListener>
    {
        private readonly Action<long> _onQueryExecuted;
        private readonly Action _onQueryCounted;

        public EfQueryObserver(Action<long> onQueryExecuted, Action onQueryCounted)
        {
            _onQueryExecuted = onQueryExecuted;
            _onQueryCounted = onQueryCounted;
        }

        public void OnNext(DiagnosticListener listener)
        {
            if (listener.Name == "Microsoft.EntityFrameworkCore")
            {
                listener.Subscribe(new EfCommandObserver(_onQueryExecuted, _onQueryCounted));
            }
        }

        public void OnError(Exception error) { }
        public void OnCompleted() { }
    }

    public class EfCommandObserver : IObserver<KeyValuePair<string, object>>
    {
        private readonly Action<long> _onQueryExecuted;
        private readonly Action _onQueryCounted;

        public EfCommandObserver(Action<long> onQueryExecuted, Action onQueryCounted)
        {
            _onQueryExecuted = onQueryExecuted;
            _onQueryCounted = onQueryCounted;
        }

        public void OnNext(KeyValuePair<string, object> value)
        {
            if (value.Key == "Microsoft.EntityFrameworkCore.Database.Command.CommandExecuted")
            {
                dynamic payload = value.Value;
                long duration = payload.Duration.Ticks;
                _onQueryExecuted(duration);
                _onQueryCounted();
            }
        }

        public void OnError(Exception error) { }
        public void OnCompleted() { }
    }
}
