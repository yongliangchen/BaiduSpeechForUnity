namespace BaiduSpeech
{
    /// <summary>唤醒功能基类(唤醒词功能只支持Android和iOS设备)</summary>
    public abstract class WakeupBase : SpeechBase
    {
        /// <summary>初始化唤醒功能</summary>
        public virtual void Init() { }
        /// <summary>开始唤醒功能</summary>
        public virtual void Begin(string wakeUpPath) { }
        /// <summary>停止唤醒功能</summary>
        public virtual void Stop() { }
        /// <summary>释放算法</summary>
        public virtual void Release() { }
    }
}