﻿using System.Threading;

namespace DotNext.Net.Cluster.Consensus.Raft
{
    internal sealed class FollowerState : RaftState
    {
        private readonly ManualResetEvent refreshEvent;
        private readonly RegisteredWaitHandle timerHandle;

        internal FollowerState(IRaftStateMachine stateMachine, int timeout)
            : base(stateMachine)
        {
            timerHandle = ThreadPool.RegisterWaitForSingleObject(refreshEvent = new ManualResetEvent(false), TimerEvent,
                null, timeout, false);
        }

        private void TimerEvent(object state, bool timedOut)
        {
            if (timedOut)
            {
                timerHandle.Unregister(refreshEvent);
                stateMachine.MoveToCandidateState();
            }
            else
                refreshEvent.Reset();
        }

        internal void Refresh() => refreshEvent.Set();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                timerHandle.Unregister(refreshEvent);
                refreshEvent.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
