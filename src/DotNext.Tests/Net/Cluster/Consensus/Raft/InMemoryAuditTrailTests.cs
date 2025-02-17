﻿using DotNext.Net.Cluster.Messaging;
using DotNext.Net.Cluster.Replication;
using DotNext.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DotNext.Net.Cluster.Consensus.Raft
{
    public sealed class InMemoryAuditTrailTests : Assert
    {
        private sealed class LogEntry : TextMessage, ILogEntry
        {
            public LogEntry(string command)
                : base(command, "Entry")
            {
            }

            public long Term { get; set; }
        }

        [Fact]
        public static async Task RaftPersistentState()
        {
            IPersistentState auditTrail = new InMemoryAuditTrail();
            NotNull(auditTrail.First);
            Equal(0, auditTrail.First.Term);
            await auditTrail.UpdateTermAsync(10);
            Equal(10, auditTrail.Term);
            await auditTrail.IncrementTermAsync();
            Equal(11, auditTrail.Term);
        }

        private sealed class CommitDetector : AsyncManualResetEvent
        {
            internal long Count;

            internal CommitDetector()
                : base(false)
            {
            }

            internal void OnCommitted(IAuditTrail<ILogEntry> sender, long startIndex, long count)
            {
                Count = count;
                Set();
            }
        }

        [Fact]
        public static async Task Appending()
        {
            IPersistentState auditTrail = new InMemoryAuditTrail();
            Equal(0, auditTrail.GetLastIndex(false));
            Equal(0, auditTrail.GetLastIndex(true));
            var entry1 = new LogEntry("SET X=0") { Term = 1 };
            var entry2 = new LogEntry("SET Y=0") { Term = 2 };
            Equal(1, await auditTrail.AppendAsync(new[] { entry1, entry2 }));
            Equal(0, auditTrail.GetLastIndex(true));
            Equal(2, auditTrail.GetLastIndex(false));
            var entries = await auditTrail.GetEntriesAsync(1, 2);
            Equal(2, entries.Count);
            entry1 = (LogEntry)entries[0];
            entry2 = (LogEntry)entries[1];
            Equal("SET X=0", entry1.Content);
            Equal("SET Y=0", entry2.Content);
            //now replace entry at index 2 with new entry
            entry2 = new LogEntry("ADD") { Term = 3 };
            Equal(2, await auditTrail.AppendAsync(new[] { entry2 }, 2));
            entries = await auditTrail.GetEntriesAsync(1, 2);
            Equal(2, entries.Count);
            entry1 = (LogEntry)entries[0];
            entry2 = (LogEntry)entries[1];
            Equal("SET X=0", entry1.Content);
            Equal("ADD", entry2.Content);
            Equal(2, auditTrail.GetLastIndex(false));
            Equal(0, auditTrail.GetLastIndex(true));
            //commit all entries
            using (var detector = new CommitDetector())
            {
                auditTrail.Committed += detector.OnCommitted;
                Equal(2, await auditTrail.CommitAsync(1));
                await detector.Wait();
                Equal(2, auditTrail.GetLastIndex(true));
                Equal(2, detector.Count);
            }
        }


    }
}
