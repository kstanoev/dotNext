﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static System.Threading.Timeout;

namespace DotNext.Threading
{
    public sealed class AsyncExclusiveLockTests : Assert
    {
        [Fact]
        public static async Task TrivialLock()
        {
            using (var @lock = new AsyncExclusiveLock())
            {
                True(await @lock.TryAcquire(TimeSpan.FromMilliseconds(10)));
                False(await @lock.TryAcquire(TimeSpan.FromMilliseconds(100)));
                await ThrowsAsync<TimeoutException>(() => @lock.Acquire(TimeSpan.FromMilliseconds(100)));
                @lock.Release();
                True(await @lock.TryAcquire(TimeSpan.FromMilliseconds(100)));
            }
        }

        [Fact]
        public static async Task ConcurrentLock()
        {
            using (var are = new AutoResetEvent(false))
            using (var @lock = new AsyncExclusiveLock())
            {
                await @lock.Acquire(TimeSpan.Zero);
                var task = new TaskCompletionSource<bool>();
                ThreadPool.QueueUserWorkItem(async state =>
                {
                    False(await @lock.TryAcquire(TimeSpan.FromMilliseconds(10)));
                    True(ThreadPool.QueueUserWorkItem(ev => ev.Set(), are, false));
                    await @lock.Acquire(InfiniteTimeSpan);
                    @lock.Release();
                    task.SetResult(true);
                });
                are.WaitOne();
                @lock.Release();
                await task.Task;
            }
        }
    }
}
