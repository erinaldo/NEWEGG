using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TWNewEgg.Logistics.HiLife.Model
{
    public class SyncTimes
    {
        public enum SyncWeek
        {
            Mon = 1,
            Tue = 2,
            Wed = 3,
            Thu = 4,
            Fri = 5,
            Sat = 6,
            Sun = 7
        }

        public SyncFile SyncFileInfo { get; set; }
        public List<PerSyncTime> SyncTimeList { get; set; }

        public class SyncFile
        {
            public SyncFile()
            {
            }

            public string From { get; set; }
            public string To { get; set; }
        }

        public class PerSyncTime
        {
            public PerSyncTime()
            {
            }
            public int Year { get; set; }
            public int Month { get; set; }
            public int Date { get; set; }
            public SyncWeek Week { get; set; }
            public int Hour { get; set; }
            public int Minutes { get; set; }
            public int Seconds { get; set; }
        }
    }
}
