using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TaskLib.Test
{
    [TestClass]
    public class TaskTest
    {
        //[TestMethod]
        //public void CheckTargetDate()
        //{
        //    DateTime now = DateTime.Now;
        //    Task task = new Task()
        //    {
        //        TargetDT = now.AddMinutes(15)
        //    };

        //    Assert.IsTrue(!task.IsTargetDate(now));
        //    Assert.IsTrue(task.IsTargetDate(now.AddSeconds(15 * 60)));
        //    Assert.IsTrue(task.IsTargetDate(now.AddMilliseconds(15 * 60000 + task.SensitivePeriodMs)));
        //}

        //[TestMethod]
        //public void CheckRemindPeriod()
        //{
        //    DateTime now = DateTime.Now;
        //    Task task = new Task()
        //    {
        //        TargetDT = now.AddMinutes(15),
        //        RemindPeriod = TimeSpan.FromDays(7)
        //    };

        //    Assert.IsTrue(!task.IsTargetDate(now));
        //    Assert.IsTrue(!task.IsTargetDate(now.AddDays(7)));
        //    Assert.IsTrue(task.IsTargetDate(now.AddDays(7).AddMinutes(15)));
        //    Assert.IsTrue(task.IsTargetDate(now.AddDays(7).AddMinutes(15).AddMilliseconds(task.SensitivePeriodMs)));
        //    Assert.IsTrue(task.IsTargetDate(now.AddDays(14).AddMinutes(15)));

        //}


        [TestMethod]
        public void TestOneTimeTimerDelayValue()
        {
            DateTime now = DateTime.Now;
            Task.DateTimeNowGetter = () => now;
            Task task = new Task()
            {
                TargetDT = now.AddHours(100)
            };
            var dif = (now.AddHours(100) - now).TotalMilliseconds;

            Assert.AreEqual(dif, task.TimerDelay);

            task.TargetDT = now.AddSeconds(-1);
            Assert.AreEqual(task.TimerDelay, Timeout.Infinite);
        }


        [TestMethod]
        public void TestPeriodicTimerDelayValue()
        {
            DateTime now = DateTime.Now;
            Task task = new Task()
            {
                TargetDT = now.AddHours(100),
                RemindPeriod = TimeSpan.FromHours(24)
            };

            Task.DateTimeNowGetter = () => now.AddHours(110);
            Assert.AreEqual(TimeSpan.FromHours(14).TotalMilliseconds, task.TimerDelay);

        }
               
    }
}
