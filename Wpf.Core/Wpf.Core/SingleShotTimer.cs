using System;
using System.Timers;

namespace Wpf.Core
{
    public class SingleShotTimer
    {
        #region Fields

        private Action callback;
        private Timer timer;

        #endregion

        #region Properties

        public double Interval { get; set; }

        #endregion

        #region Methods

        public void Start(Action action)
        {
            if (timer == null)
            {
                timer = new Timer();
                timer.Interval = Interval;
                timer.Elapsed += Timer_Elapsed;
                timer.AutoReset = false;
            }

            callback = action;

            if (timer.Enabled)
            {
                // calling stop then start will reset the internal counter so it starts back at 0
                timer.Stop();
            }

            timer.Start();
        }

        public void Start(double interval, Action action)
        {
            Interval = interval;

            Start(action);
        }

        public void Stop()
        {
            timer.Stop();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            callback();
        }

        #endregion
    }
}
