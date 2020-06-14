using System;
using System.Diagnostics;

namespace RCSHTools
{
    /// <summary>
    /// Records the duration of an action
    /// </summary>
    public class RecordAction
    {
        private long time;

        /// <summary>
        /// Runs an action and records its time
        /// </summary>
        public RecordAction(Action action)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            action();
            timer.Stop();
            time = timer.ElapsedMilliseconds;
        }

        /// <summary>
        /// Logs to the console a message so %s is replaces with the time in seconds and %t is replaces with in milliseconds
        /// </summary>
        /// <param name="message"></param>
        public void Log(string message)
        {
            Console.WriteLine(message.Replace("%s", (time / 1000f).ToString()).Replace("%t", time.ToString()));
        }
    }
}
