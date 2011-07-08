using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Media;

namespace WurmTimer
{
    public class CountDownTimer : LabelProgressBar
    {
        private Timer timer = new Timer();
        private DateTime startTime;
        private DateTime endTime;
        private TimeSpan duration;
        private bool running = false;

        public string label;
        public string Label { get; set; }

        public event EventHandler Expired;

        public TimeSpan Duration
        {
            get
            {
                if (running)
                {
                    DateTime now = DateTime.Now;
                    if (endTime > now)
                        return endTime - DateTime.Now;
                    else
                        return new TimeSpan();

                }
                else
                {
                    return duration;
                }
            }
            set
            {
                if (running)
                {
                    endTime = DateTime.Now + value;
                    Tick();
                }
                else
                {
                    duration = value;
                }
            }
        }

        public CountDownTimer()
        {
            running = false;
            Style = ProgressBarStyle.Continuous;
        }

        protected override void Dispose(bool disposing)
        {
            timer.Stop();
            base.Dispose(disposing);
        }

        public void Start()
        {
            timer.Enabled = false;

            startTime = DateTime.Now;
            endTime = startTime + duration;

            this.Value = 0;
            this.Maximum = (int)duration.TotalSeconds;

            timer.Tick += new EventHandler(Tick);
            timer.Interval = 1000;
            timer.Enabled = true;
            running = true;
        }

        public void StartFlash()
        {
            timer.Enabled = false;

            timer.Tick -= new EventHandler(Tick);
            timer.Tick += new EventHandler(Flash);
            timer.Interval = 1000;
            timer.Enabled = true;
        }

        public void Stop()
        {
            timer.Enabled = false;
            running = false;
        }

        private void Tick(object sender, EventArgs e)
        {
            Tick();
        }

        private void Flash(object sender, EventArgs e)
        {
            if (LabelColor2 == SystemColors.ActiveCaptionText)
                LabelColor2 = SystemColors.ActiveCaption;
            else
                LabelColor2 = SystemColors.ActiveCaptionText;
        }

        private void Tick()
        {
            DateTime now = DateTime.Now;

            TimeSpan elapsed = now - startTime;
            TimeSpan remaining = endTime - now;

            base.Text = String.Format("{0}: {1:hh\\:mm\\:ss}", Label, remaining);
            this.Value = Math.Min((int)elapsed.TotalSeconds, this.Maximum);

            if (now >= endTime)
            {
                SystemSounds.Beep.Play();
                StartFlash();
                Expired.Invoke(this, new EventArgs());
            }
        }
    }
}
