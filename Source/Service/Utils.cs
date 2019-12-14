using System;
using System.Threading.Tasks;

namespace RAGEMP_TsVoice
{
    class Utils
    {
        public static System.Timers.Timer Delay(int ms, bool onlyOnce, Action action)
        {
            if (onlyOnce)
            {
                Task.Delay(ms).ContinueWith((t) => action());
                return null;
            }
            else
            {
                var t = new System.Timers.Timer(ms);
                t.Elapsed += (s, e) => action();
                t.Start();
                return t;
            }
        }

        public static void StopTimer(System.Timers.Timer timer) => timer.Stop();
    }
}
