using System;
using System.Timers;

namespace WPF.Core
{
    /// <summary>Ensures a method is only fired one time in a given time span (interval).</summary>
    public class Debouncer : IDisposable
    {
        #region Fields

        protected Action action;
        protected Timer timer;
        protected bool disposedValue;

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="Debouncer"/> class.</summary>
        /// <param name="interval">The interval of time before the method can be fired if not called again.</param>
        /// <param name="action">The method to call after the interval elapses.</param>
        /// <exception cref="ArgumentNullException">action is null.</exception>
        public Debouncer(double interval, Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            this.action = action;

            timer = new Timer(interval);
            timer.Elapsed += Timer_Elapsed;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Throttles/Debounces the method invocation so it only happens once in the given interval. Calling this 
        /// method will reset the timer so it starts over (if it is running already).
        /// </summary>
        /// <exception cref="ObjectDisposedException">The object instance has been disposed.</exception>
        public virtual void Debounce()
        {
            if (disposedValue)
                throw new ObjectDisposedException("WPF.Core.Debouncer");

            // if the timer is running, stop it and start it again
            // this makes the interval reset
            if (timer.Enabled) timer.Stop();

            timer.Start();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                throw new ObjectDisposedException("WPF.Core.Debouncer");

            if (!disposedValue)
            {
                if (disposing)
                {
                    timer.Stop();
                    timer.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <summary>Free resources used by this object.</summary>
        /// <exception cref="ObjectDisposedException">The object instance has been disposed.</exception>
        public void Dispose()
        {
            if (disposedValue)
                throw new ObjectDisposedException("WPF.Core.Debouncer");

            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            action();
        }

        #endregion
    }
}
