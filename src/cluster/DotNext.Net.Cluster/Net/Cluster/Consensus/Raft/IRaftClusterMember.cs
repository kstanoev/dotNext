﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DotNext.Net.Cluster.Consensus.Raft
{
    /// <summary>
    /// Represents cluster member accessible through Raft protocol.
    /// </summary>
    public interface IRaftClusterMember : IClusterMember
    {
        /// <summary>
        /// Requests vote from the member.
        /// </summary>
        /// <param name="term">Term value maintained by local cluster member.</param>
        /// <param name="lastLogIndex">Index of candidate's last log entry.</param>
        /// <param name="lastLogTerm">Term of candidate's last log entry.</param>
        /// <param name="token">The token that can be used to cancel asynchronous operation.</param>
        /// <returns>Vote received from member; <see langword="true"/> if node accepts new leader, <see langword="false"/> if node doesn't accept new leader, <see langword="null"/> if node is not available.</returns>
        Task<Result<bool>> VoteAsync(long term, long lastLogIndex, long lastLogTerm, CancellationToken token);

        /// <summary>
        /// Transfers transaction log entry to the member.
        /// </summary>
        /// <param name="term">Term value maintained by local cluster member.</param>
        /// <param name="entries">A set of entries to be replicated with this node.</param>
        /// <param name="prevLogIndex">Index of log entry immediately preceding new ones.</param>
        /// <param name="prevLogTerm">Term of <paramref name="prevLogIndex"/> entry.</param>
        /// <param name="commitIndex">Last entry known to be committed by the local node.</param>
        /// <param name="token">The token that can be used to cancel asynchronous operation.</param>
        /// <returns><see langword="true"/> if message is handled successfully by this member; <see langword="false"/> if message is rejected due to invalid Term/Index number.</returns>
        /// <exception cref="MemberUnavailableException">The member is unreachable through network.</exception>
        Task<Result<bool>> AppendEntriesAsync(long term, IReadOnlyList<ILogEntry> entries, long prevLogIndex, long prevLogTerm, long commitIndex, CancellationToken token);

        /// <summary>
        /// Index of next log entry to send to this node.
        /// </summary>
        /// <value></value>
        ref long NextIndex { get; }

        /// <summary>
        /// Aborts all active outbound requests.
        /// </summary>
        void CancelPendingRequests();
    }
}
