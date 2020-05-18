using Library.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryServices
{
   public class DataHelpers
    {
        public static IEnumerable<string> HumanizeBizHours(IEnumerable<BranchHours> branchHour)
        {

            var hours = new List<string>();

            foreach(var time in branchHour)
            {
                var day = HumanizeDay(time.DayOfWeek);
                var opentime = HumanizeTime(time.OpenTime);
                var closetime = HumanizeTime(time.CloseTime);

                var timeEntry = $"{day} {opentime} to {closetime}";
                hours.Add(timeEntry); 
            }

            return hours;
        }

        public static string HumanizeDay(int number)
        {
            return Enum.GetName(typeof(DayOfWeek), number);
        }

        public static string HumanizeTime(int time)
        {
            return TimeSpan.FromHours(time).ToString("hh':'mm"); 
        }
    }
}
