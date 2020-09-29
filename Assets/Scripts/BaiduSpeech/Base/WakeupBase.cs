namespace BaiduSpeech
{
    /// <summary>唤醒功能基类</summary>
    public abstract class WakeupBase : SpeechBase
    {
        /// <summary>初始化唤醒功能</summary>
        public virtual void WakeupInit() { }
        /// <summary>开始唤醒功能</summary>
        public virtual void WakeupStart(string wakeUpPath) { }
        /// <summary>停止唤醒功能</summary>
        public virtual void WakeupStop() { }
    }
}