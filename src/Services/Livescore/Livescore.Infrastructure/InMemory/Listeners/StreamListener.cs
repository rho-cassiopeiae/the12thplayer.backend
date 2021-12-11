using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;

using ServiceStack.Redis;

namespace Livescore.Infrastructure.InMemory.Listeners {
    public abstract class StreamListener {
        protected class _Stream {
            public string Name { get; init; }
            public List<_StreamEntry> Entries { get; init; }
        }

        protected class _StreamEntry {
            public string Id { get; init; }
            public List<KeyValuePair<string, string>> NamedValues { get; init; }
        }

        protected readonly ILogger _logger;
        protected readonly IRedisClientsManager _redis;

        protected StreamListener(ILogger logger, IRedisClientsManager redis) {
            _logger = logger;
            _redis = redis;
        }

        protected object[] _populateCommandArgs(
            List<object> commandArgs, List<KeyValuePair<string, string>> streamPositions
        ) {
            if (commandArgs.Count > 4) {
                commandArgs.RemoveRange(4, commandArgs.Count - 4);
            }

            foreach (var position in streamPositions) {
                commandArgs.Add(position.Key);
            }
            foreach (var position in streamPositions) {
                commandArgs.Add(position.Value);
            }

            return commandArgs.ToArray();
        }

        protected IEnumerable<_Stream> _parseResult(RedisText result) {
            if (result == null) {
                return null;
            }

            /* Result format:

                1) 1) "mystream"
                   2) 1) 1) 1526984818136-0
                         2) 1) "duration"
                            2) "1532"
                            3) "event-id"
                            4) "5"
                            5) "user-id"
                            6) "7782813"
                      2) 1) 1526999352406-0
                         2) 1) "duration"
                            2) "812"
                            3) "event-id"
                            4) "9"
                            5) "user-id"
                            6) "388234"
                2) 1) "writers"
                   2) 1) 1) 1526985676425-0
                         2) 1) "name"
                            2) "Virginia"
                            3) "surname"
                            4) "Woolf"
                      2) 1) 1526985685298-0
                         2) 1) "name"
                            2) "Jane"
                            3) "surname"
                            4) "Austen"

            */

            var streams = new List<_Stream>(result.Children.Count);
            foreach (var streamResult in result.Children) {
                var streamEntryResults = streamResult.Children.Last().Children;

                var stream = new _Stream {
                    Name = streamResult.Children.First().GetResult(),
                    Entries = new List<_StreamEntry>(streamEntryResults.Count)
                };

                foreach (var streamEntryResult in streamEntryResults) {
                    var streamEntryRecords = streamEntryResult.Children.Last().Children;

                    var streamEntry = new _StreamEntry {
                        Id = streamEntryResult.Children.First().GetResult(),
                        NamedValues = new List<KeyValuePair<string, string>>(streamEntryRecords.Count)
                    };

                    for (int i = 0; i < streamEntryRecords.Count; i += 2) {
                        streamEntry.NamedValues.Add(
                            new KeyValuePair<string, string>(
                                streamEntryRecords[i].GetResult(), streamEntryRecords[i + 1].GetResult()
                            )
                        );
                    }

                    stream.Entries.Add(streamEntry);
                }

                streams.Add(stream);
            }

            return streams;
        }
    }
}
