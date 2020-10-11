using UnityEngine;
using UnityEngine.UI;

namespace BaiduSpeech.Examples
{
    /// <summary>唤醒功能测试</summary>
    public class WakeupDemo : MonoBehaviour
    {
        public Text content;
        public Text stateText;

        private BaiduSpeechManager m_BaiduSpeechManager;

        private void Start()
        {
            m_BaiduSpeechManager = FindObjectOfType<BaiduSpeechManager>();
            m_BaiduSpeechManager.WakeupInit();//初始化唤醒词
            m_BaiduSpeechManager.WakeupStart("WakeUp.bin");//加载唤醒词，词库
            m_BaiduSpeechManager.onSpeechEventListener += OnSpeechEventListener;

            stateText.text = "初始化唤醒词!";
        }

        private void OnDestroy()
        {
            m_BaiduSpeechManager.onSpeechEventListener -= OnSpeechEventListener;
        }

        /// <summary>百度语音识别事件</summary>
        private void OnSpeechEventListener(SpeechEventListenerInfo callbackMessage)
        {
            if (callbackMessage.state.Equals(SpeechConstant.CALLBACK_EVENT_WAKEUP_SUCCESS))
            {
                stateText.text = "唤醒成功!";
                ParamsData wakeupParams = Serializable.GetWakeupParams(callbackMessage.param);
                Debug.Log("errorCode:" + wakeupParams.errorCode);
                Debug.Log("errorDesc:" + wakeupParams.errorDesc);
                content.text = wakeupParams.word;
            }
        }

    }
}