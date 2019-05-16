using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SystemClockSetter
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEMTIME
    {
        public short wYear;
        public short wMonth;
        public short wDayOfWeek;
        public short wDay;
        public short wHour;
        public short wMinute;
        public short wSecond;
        public short wMilliseconds;
    }


    public static class ClockHelper
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint SetSystemTime(ref SYSTEMTIME st);

        public static uint SetClock(DateTime dateTime)
        {
            var st = new SYSTEMTIME();
            st.wYear = (short)dateTime.Year;
            st.wMonth = (short)dateTime.Month;
            st.wDay = (short)dateTime.Day;
            st.wHour = (short)dateTime.Hour;
            st.wMinute = (short)dateTime.Minute;
            st.wSecond = (short)dateTime.Second;

            return SetSystemTime(ref st); // invoke this method.
        }

    }
}
