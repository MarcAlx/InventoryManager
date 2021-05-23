using System;
namespace Engine
{
    /// <summary>
    /// Reusable timer that updates with gametime
    /// </summary>
    public class Timer : IDisposable
    {
        private Action   _action = null;
        private TimeSpan _interval = TimeSpan.FromSeconds(0);
        private bool     _repeat = false;

        private bool _started;
        private bool _paused;
        private bool _finished;

        private TimeSpan _elapsedTime = TimeSpan.FromSeconds(0);

        /// <summary>
        /// True if started
        /// </summary>
        public bool IsStarted
        {
            get
            {
                return this._started;
            }
        }

        /// <summary>
        /// True if paused
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return this._paused;
            }
        }

        /// <summary>
        /// True if finished
        /// </summary>
        public bool IsFinished
        {
            get
            {
                return this._finished;
            }
        }

        /// <summary>
        /// Init a timer with an action to perform after a given interval,
        /// this timer could repeat itself after being done.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="interval"></param>
        /// <param name="repeat"></param>
        public Timer(Action action, TimeSpan interval, bool repeat = false)
        {
            this._action = action;
            this._interval = interval;
            this._repeat = repeat;
        }

        /// <summary>
        /// Start this timer
        /// </summary>
        public void Start()
        {
            lock (this)
            {
                this._started = true;
            }
        }

        /// <summary>
        /// Update this timer with elpapsed time
        /// </summary>
        /// <param name="elapsedTime"></param>
        public void Update(TimeSpan elapsedTime)
        {
            lock (this)
            {
                if (this._started
                && !this._paused
                && !this._finished
                &&  this._action != null)
                {
                    this._elapsedTime += elapsedTime;
                    if (this._elapsedTime >= this._interval)
                    {
                        this._action();
                        if (this._repeat)
                        {
                            this._elapsedTime = TimeSpan.FromSeconds(0);
                        }
                        else
                        {
                            this.Stop();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Pause this timer
        /// </summary>
        public void Pause()
        {
            lock (this)
            {
                this._paused = true;
            }
        }

        /// <summary>
        /// Stop this timer
        /// </summary>
        public void Stop()
        {
            lock (this)
            {
                this._finished = true;
            }
        }

        /// <summary>
        /// Dispose this timer by stopping it
        /// </summary>
        public void Dispose()
        {
            this.Stop();
        }
    }
}
