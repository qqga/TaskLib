//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace TaskLib.Test
//{
//    [TestClass]
//    public class TaskReminderTest
//    {
//        [TestMethod]
//        public void TestTaskReminderTargetDate()
//        {
//            var tasks = new[]{
//                new Task(){TargetDT = DateTime.Now.AddMilliseconds(555)},
//                new Task(){TargetDT = DateTime.Now.AddMilliseconds(777)},
//            };
//            int i = 0;
//            var taskreminder = new TaskReminder(() => tasks);
//            taskreminder.OnActualTaskResived += (s, e) =>
//            {
//                i++;
//            };
//            taskreminder.Start(1);
//            System.Threading.Tasks.Task.Delay(1000);

//            Assert.AreEqual(2, i);
//        }

//        [TestMethod]
//        public void TestTaskReminderRemindPeriod()
//        {
//            var tasks = new[]{
//                new Task(){
//                    TargetDT = DateTime.Now.AddMilliseconds(2000),
//                    RemindPeriod = TimeSpan.FromMilliseconds(1000),
//                    SensitivePeriodMs = 50,
//                },
//            };
//            int i = 0;
//            var taskreminder = new TaskReminder(() => tasks);
//            taskreminder.OnActualTaskResived += (s, e) =>
//            {
//                i++;
//            };
//            taskreminder.Start(10);
//            System.Threading.Tasks.Task.Delay(20000);

//            Assert.AreEqual(4, i);
//        }
//    }
//}
