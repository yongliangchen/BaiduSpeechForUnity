namespace BaiduSpeech
{
    /// <summary>语音转文字基类</summary>
    public abstract class AsrBase : SpeechBase
    {
        /// <summary>初始化语音</summary>
        public virtual void Init() { }
        /// <summary>离线命令词(只支持Andrioid和iOS设备)</summary>
        public virtual void LoadOfflineEngine(string json) { }
        /// <summary>开始录音</summary>
        public virtual void Begin(string json) { }
        /// <summary>取消本次识别，取消后将立即停止不会返回识别结果</summary>
        public virtual void Cancel() { }
        /// <summary>停止录音</summary>
        public virtual void Stop() { }
        /// <summary>释放算法</summary>
        public virtual void Release() { }
    }
}