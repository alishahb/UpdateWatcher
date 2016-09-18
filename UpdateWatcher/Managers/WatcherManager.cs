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
        private Timer _cycleTimer { get; set; }
        private Timer _tickTimer { get; set; }
        public Stopwatch _elapsedTime { get; private set; }
        private bool IsWorking { get; set; }
        public uint CycleNumber { get; protected set; }

        public WatcherManager(int minutesDelay)
        {
            MinutesDelay = minutesDelay;

            _elapsedTime = new Stopwatch();
            _tickTimer = new Timer(1000);
            _cycleTimer = new Timer(TimeSpan.FromMinutes(minutesDelay).TotalMilliseconds);

            _cycleTimer.Elapsed += OnCycleElapsed;
            _cycleTimer.AutoReset = true;

            _tickTimer.Elapsed += OnTickElapsed;
            _tickTimer.AutoReset = true;

        }

        public void Execute()
        {
            if (IsWorking) return;

            IsWorking = true;

            OnWatchStarted?.Invoke(this, null);

            _cycleTimer.Start();
            _elapsedTime.Restart();
            _tickTimer.Start();

        }

        private void OnTickElapsed(object sender, ElapsedEventArgs e) => OnTick?.Invoke(_elapsedTime.Elapsed, null);
        private void OnCycleElapsed(object sender, ElapsedEventArgs e)
        {
            CycleNumber++;
            OnWatchCycleCompleted?.Invoke(this, e);
        }

        public void Cancel()
        {
            _elapsedTime.Stop();
            _tickTimer?.Stop();
            _cycleTimer?.Stop();

            OnWatchStopped?.Invoke(this, null);

            IsWorking = false;
            CycleNumber = 0;
        }

        public void Dispose()
        {
            _elapsedTime.Stop();
            _elapsedTime = null;

            _tickTimer?.Stop();
            _tickTimer?.Dispose();

            _cycleTimer?.Stop();
            _cycleTimer?.Dispose();

            OnWatchStopped?.Invoke(this, null);

        }
    }
}
