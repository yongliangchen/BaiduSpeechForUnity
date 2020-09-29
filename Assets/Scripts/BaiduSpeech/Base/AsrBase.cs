namespace BaiduSpeech
{
    /// <summary>语音转文字基类</summary>
    public abstract class AsrBase : SpeechBase
    {
        /// <summary>初始化语音</summary>
        public virtual void AsrInit() { }
        /// <summary>开始录音</summary>
        public virtual void VoiceStart(string json) { }
        /// <summary>取消本次识别，取消后将立即停止不会返回识别结果</summary>
        public virtual void VoiceCancel() { }
        /// <summary>停止录音</summary>
        public virtual void VoiceStop() { }
    }
}