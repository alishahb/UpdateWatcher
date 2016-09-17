using System;
using System.Diagnostics;
using System.Timers;

namespace Alisha.UpdateWatcher.Managers
{
    public class WatcherManager : IDisposable
    {
        public event EventHandler OnWatchStarted;
        public event EventHandler OnWatchStopped;
        public event EventHandler OnTick;
        public event EventHandler OnWatchCycleCompleted;

        public int MinutesDelay { get; }
        private Timer _checkTimer { get; set; }
        private Timer _ticker { get; set; }
        public Stopwatch _elapsedTime { get; private set; }

        public WatcherManager(int minutesDelay)
        {
            MinutesDelay = minutesDelay;

            _elapsedTime = new Stopwatch();
            _ticker = new Timer(1000);
            _checkTimer = new Timer(TimeSpan.FromSeconds(10).TotalMilliseconds);

            _checkTimer.Elapsed += OnElapsed;
            _checkTimer.AutoReset = false;

            _ticker.Elapsed += OnTickElapsed;
            _ticker.AutoReset = true;

        }

        public void Execute()
        {
            OnWatchStarted?.Invoke(this, null);

            _checkTimer.Start();
            _elapsedTime.Restart();
            _ticker.Start();

        }

        private void OnTickElapsed(object sender, ElapsedEventArgs e) => OnTick?.Invoke(_elapsedTime.Elapsed, null);
        private void OnElapsed(object sender, ElapsedEventArgs e) => OnWatchCycleCompleted?.Invoke(this, e);

        public void Cancel()
        {
            _elapsedTime.Stop();
            _ticker?.Stop();
            _checkTimer?.Stop();

            OnWatchStopped?.Invoke(this, null);
        }

        public void Dispose()
        {
            _elapsedTime.Stop();
            _elapsedTime = null;

            _ticker?.Stop();
            _ticker?.Dispose();

            _checkTimer?.Stop();
            _checkTimer?.Dispose();

            OnWatchStopped?.Invoke(this, null);

        }
    }
}
