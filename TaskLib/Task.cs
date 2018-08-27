using System;
using System.Collections.Generic;

namespace TaskLib
{
    public interface ITask
    {
        int TaskId { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        DateTime CreatedDT { get; set; }
        DateTime? TargetDT { get; set; }
        TimeSpan? RemindPeriod { get; set; }
        bool? IsComplited { get; set; }
        IEnumerable<ITask> SubTasks { get; set; }
        int? SupTaskId { get; set; }
        Task SupTask { get; set; }

    }
    public class Task : IDisposable, ITask
    {
        public static Func<DateTime> DateTimeNowGetter = () => DateTime.Now;

        public int TaskId { get; set; }
        // Дата создания задания.
        public DateTime CreatedDT { get; set; }

        DateTime? _TargetDT;
        // Целевая дата.        
        public DateTime? TargetDT
        {
            get => _TargetDT;
            set
            {
                if(_TargetDT != value)
                {
                    _TargetDT = value;
                    OnTargetDTChanged();
                }
            }
        }

        TimeSpan? _RemindPeriod;
        // Период повторения. Если не задно - однократно.
        public TimeSpan? RemindPeriod
        {
            get => _RemindPeriod;
            set
            {
                if(value != _RemindPeriod)
                {
                    _RemindPeriod = value;
                    OnRemindPeriodChanged();
                }
            }
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? _IsComplited;
        public bool? IsComplited
        {
            get => _IsComplited;
            set
            {
                if(_IsComplited != value)
                {
                    _IsComplited = value;
                    OnIsComplitedChanged();
                }
            }
        }
        public int? SupTaskId { get; set; }
        // 
        public Task SupTask { get; set; }
        // Подзадачи.
        public IEnumerable<ITask> SubTasks { get; set; }

        //public bool IsActual => IsComplited == false;

        //public bool IsTargetDT(DateTime dateTime)
        //{

        //    var fromNowToTarget = TargetDT - dateTime;
        //    var fromNowToTargetMs = fromNowToTarget.TotalMilliseconds;
        //    var fromNowToTargetMsAbs = Math.Abs(fromNowToTargetMs);//fromNowToTarget.TotalMilliseconds;

        //    //первая цель
        //    if(fromNowToTargetMs + SensitivePeriodMs >= 0 && fromNowToTargetMs <= SensitivePeriodMs)
        //        return true;

        //    if(fromNowToTargetMs < 0 &&
        //        RemindPeriod.HasValue &&
        //        fromNowToTargetMsAbs >= RemindPeriod.Value.TotalMilliseconds &&
        //        fromNowToTargetMsAbs % RemindPeriod.Value.TotalMilliseconds <= SensitivePeriodMs)
        //    {
        //        return true;
        //    }

        //    return false;
        //}
        //public bool IsNowTargetDate => IsTargetDT(DateTime.Now);

        public event EventHandler OnTaskTime;
        System.Threading.Timer _Timer;

        public long TimerDelay
        {
            get
            {
                if(!TargetDT.HasValue)
                    return RemindPeriod.HasValue ? (long)RemindPeriod.Value.TotalMilliseconds : System.Threading.Timeout.Infinite;

                DateTime now = DateTimeNowGetter();

                TimeSpan nowToTargetTimeSpan = TargetDT.Value - now;
                long nowToTargetMs = (long)nowToTargetTimeSpan.TotalMilliseconds;
                if(RemindPeriod.HasValue)
                {
                    return nowToTargetMs > 0 ? nowToTargetMs :
                        (long)(RemindPeriod.Value.TotalMilliseconds - (-nowToTargetMs % RemindPeriod.Value.TotalMilliseconds));
                }
                else
                {
                    return nowToTargetMs > 0 ? nowToTargetMs : System.Threading.Timeout.Infinite;
                }
            }
        }

        public long TimerPeriod => RemindPeriod.HasValue ? (long)RemindPeriod.Value.TotalMilliseconds : System.Threading.Timeout.Infinite;

        public void StartObserve()
        {
            //SheckTimerCorrect();

            _Timer = _Timer ?? new System.Threading.Timer(TimerCallback);
            RefreshTimerParams();
        }
        public void RefreshTimerParams()
        {
            System.Diagnostics.Debug.WriteLine($"Refresh timer, delay:{TimerDelay}, period:{TimerPeriod}");
            _Timer?.Change(TimerDelay, TimerPeriod);
        }

        public void StopObserve()
        {
            _Timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }
        void TimerCallback(object state)
        {
            System.Diagnostics.Debug.WriteLine($"Task({Name}) TimerCallback at {DateTime.Now}");
            OnTaskTime?.Invoke(this, EventArgs.Empty);
            //RefreshTimerParams();
        }

        //void SheckTimerCorrect()
        //{
        //    if(IsComplited == true)
        //        throw new InvalidOperationException("Can't observe Complited task.");

        //    if(RemindPeriod == null && TargetDT < DateTimeNowGetter())
        //        throw new InvalidOperationException("Can't observe task. Target date is smaller then current and period is not set.");

        //}

        #region Property changed

        void OnTargetDTChanged()
        {
            if(_Timer != null)
                RefreshTimerParams();
        }
        void OnRemindPeriodChanged()
        {
            if(_Timer != null)
                RefreshTimerParams();
        }
        void OnIsComplitedChanged()
        {
            if(_Timer != null)
            {
                if(IsComplited == true)
                    _Timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                else if(IsComplited == false)
                    RefreshTimerParams();
            }
        }
        #endregion

        public override string ToString()
        {
            return $"Task:'{Name}', Target:'{TargetDT}', period:'{RemindPeriod}'";
        }

        public void Dispose()
        {
            _Timer?.Dispose();
        }
    }
}
