using HDJ.Framework.Tools;
namespace HDJ.Framework.Game.LogicSystem
{
    [ComponentName(LogicComponentType.Event, "时间/倒计时")]
    [System.Serializable]
    public class Event_Timer_Countdown : EventComponentBase
    {
        public string timerName = "";
        public float timeCount = 0;
        public bool isIgnoreTimeScale = false;

        public bool isSetTimeValue2InternalValue = false;
        public string internalVariableName = "";

        private Timer ts;
        protected override void Init()
        {
            ts = TimerManager.SetTimerRunOnce(timeCount, timerName);
            ts.OnComplete = (name) =>
            {
                EventComplete();
            };
            ts.isRealTime = isIgnoreTimeScale;
            ts.autoDestroyOnStop = false;
            if (isSetTimeValue2InternalValue)
            {
                ts.OnUpdate = (leftTime) =>
                {
                    logicObject.logicManager.SetInternalValue(internalVariableName, (int)leftTime);

                };
            }
        }

        public override void OnPause(bool isPause)
        {
            if (ts != null)
            {
                if (isPause)
                    ts.Pause();
                else
                {
                    if (ts.TimerState == TimerPlayState.Pause)
                        ts.Start();
                }
            }
        }
        public override void OnClose()
        {
            ts.Stop();
            ts.autoDestroyOnStop = true;
        }

        public override string ToExplain()
        {
            return "倒计时：" + timeCount + " 时间缩放：" + isIgnoreTimeScale;
        }
    }
}