using UnityEngine;

namespace BaiduSpeech
{
    /// <summary>百度语音基类</summary>
    public abstract class SpeechBase : MonoBehaviour
    {
        public string appId { get; set; }
        public string apiKey { get; set; }
        public string secretKey { get; set; }

        private void Awake()
        {
            OnAwake();
            OnInitPlatform();
        }

        private void Start()
        {
            OnStart();
        }

        private void Update()
        {
            OnUpdate();
        }

        private void OnDestroy()
        {
            OnDispose();
        }

        //----------------------------------------公共函数----------------------------------------
        /// <summary>初始化</summary>
        public virtual void OnAwake() { }
        public virtual void OnStart() { }
        public virtual void OnUpdate() { }
        /// <summary>初始化平台</summary>
        public virtual void OnInitPlatform() { }
        /// <summary>释放算法</summary>
        public virtual void OnDispose() { }

        //----------------------------------------语音识别----------------------------------------
        ///// <summary>初始化语音</summary>
        //public virtual void SpeechInit() { }
        ///// <summary>开始录音</summary>
        //public virtual void VoiceStart(string json) { }
        ///// <summary>取消本次识别，取消后将立即停止不会返回识别结果</summary>
        //public virtual void VoiceCancel() { }
        ///// <summary>停止录音</summary>
        //public virtual void VoiceStop() { }
        ///// <summary>释放语音识别算法</summary>
        //public virtual void SpeechDispose() { }

        //----------------------------------------唤醒词----------------------------------------
        ///// <summary>初始化唤醒词</summary>
        //public virtual void WakeupInit() { }
        ///// <summary>开始唤醒词功能</summary>
        //public virtual void WakeupStart(string wakeUpPath) { }
        ///// <summary>停止唤醒词</summary>
        //public virtual void WakeupStop() { }
        ///// <summary>释放唤醒词词算法</summary>
        //public virtual void WakeupDispose() { }

    }
}