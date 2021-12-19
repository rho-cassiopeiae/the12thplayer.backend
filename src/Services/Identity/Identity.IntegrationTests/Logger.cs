using System;

using Microsoft.Extensions.Logging;

using Xunit.Sdk;
using Xunit.Abstractions;

namespace Identity.IntegrationTests {
    public class XunitLoggerProvider : ILoggerProvider {
        private readonly IMessageSink _sink;

        public XunitLoggerProvider(IMessageSink sink) {
            _sink = sink;
        }

        public ILogger CreateLogger(string categoryName)
            => new XunitLogger(_sink, categoryName);

        public void Dispose() { }
    }

    public class XunitLogger : ILogger {
        private readonly IMessageSink _sink;
        private readonly string _categoryName;

        public XunitLogger(IMessageSink sink, string categoryName) {
            _sink = sink;
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
            => _NoopDisposable.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter
        ) {
            _sink.OnMessage(new DiagnosticMessage($"{_categoryName} [{eventId}] {formatter(state, exception)}"));
            if (exception != null) {
                _sink.OnMessage(new DiagnosticMessage(exception.ToString()));
            }
        }

        private class _NoopDisposable : IDisposable {
            public static readonly _NoopDisposable Instance = new _NoopDisposable();

            public void Dispose() { }
        }
    }
}
