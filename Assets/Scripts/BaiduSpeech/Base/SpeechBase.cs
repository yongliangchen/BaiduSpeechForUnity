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
        /// <summary>删除脚本</summary>
        public virtual void OnDispose() { }

    }
}