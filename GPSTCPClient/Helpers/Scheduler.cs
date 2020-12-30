using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace GPSTCPClient.Helpers
{
    public interface IAppendable
    {
        void Append(Action action);
    }

    public class TaskGroup : IAppendable
    {
        public int CurrentlyQueuedTasks { get { return _currentlyQueued; } }

        private readonly object _previousTaskMonitor;
        private Task _previousTask;
        private int _currentlyQueued;

        public TaskGroup()
        {
            _previousTaskMonitor = new object();
            _previousTask = Task.FromResult(false);
        }

        public void Append(Action action)
        {
            lock (_previousTaskMonitor)
            {
                Interlocked.Increment(ref _currentlyQueued);
                _previousTask = _previousTask.ContinueWith(task =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception)
                    {
                        //TODO
                    }
                    finally
                    {
                        Interlocked.Decrement(ref _currentlyQueued);
                    }
                });
            }
        }
    }

    public class QueueAppendable : IAppendable, IDisposable
    {
        public int CurrentlyQueuedTasks { get { return _Queue.Count; } }

        BlockingCollection<Action> _Queue = new BlockingCollection<Action>();

        public QueueAppendable()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        var action = _Queue.Take();
                        action();
                    }
                    catch (InvalidOperationException)
                    {
                        break;
                    }
                    catch
                    {
                        // TODO log me
                    }
                }
            });
        }

        public void Append(Action action)
        {
            _Queue.Add(action);
        }

        public void Dispose()
        {
            _Queue.CompleteAdding();
        }
    }

}
