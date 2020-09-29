using System;
using UnityEngine;

namespace BaiduSpeech
{
    /// <summary>百度语音管理类</summary>
    public class BaiduSpeechManager : MonoBehaviour
    {
        private const string APP_ID = "21554025";
        private const string API_KEY = "oihuTZ4kIwWctUmnnTIj39YA";
        private const string SECRET_KEY = "wMCLOlTWSrzPbHz9cyWEZnpTGfoV78Yd";

        /// <summary>语音识别事件，参数1表示状态，参数2表示识别结果</summary>
        public event Action<CallbackMessageInfo> onSpeechEvent = null;
        /// <summary>唤醒词事件，参数1表示状态，参数2表示识别结果</summary>
        //public event Action<CallbackMessageInfo> onWakeupEvent = null;
        private SpeechBase m_SpeechBase;

        private void Awake()
        {

#if UNITY_ANDROID && !UNITY_EDITOR
         m_SpeechBase = gameObject.AddComponent<SpeechForAndroid>();
#else

#if UNITY_IPHONE && !UNITY_EDITOR
         m_SpeechBase = gameObject.AddComponent<SpeechForiOS>();
#else
         m_SpeechBase = gameObject.AddComponent<SpeechForOther>();
#endif
#endif
            InitBaiduSpeechKey();
        }

        private void InitBaiduSpeechKey()
        {
            if (m_SpeechBase != null)
            {
                m_SpeechBase.appId = APP_ID;
                m_SpeechBase.apiKey = API_KEY;
                m_SpeechBase.secretKey = SECRET_KEY;
            }
        }

        //----------------------------------------语音识别----------------------------------------

        /// <summary>初始化语音识别</summary>
        public void SpeechInit()
        {
            if (m_SpeechBase != null) m_SpeechBase.SpeechInit();
        }

        /// <summary>开始录音</summary>
        public void VoiceStart(string json)
        {
            if (m_SpeechBase != null) m_SpeechBase.VoiceStart(json);
        }

        /// <summary>取消本次识别，取消后将立即停止不会返回识别结果</summary>
        public void VoiceCancel()
        {
            if (m_SpeechBase != null) m_SpeechBase.VoiceCancel();
        }

        /// <summary>停止录音</summary>
        public void VoiceStop()
        {
            if (m_SpeechBase != null) m_SpeechBase.VoiceStop();
        }

        /// <summary>释放语音识别算法</summary>
        public void SpeechDispose()
        {
            if (m_SpeechBase != null) m_SpeechBase.SpeechDispose();
        }

        //----------------------------------------唤醒词----------------------------------------

        /// <summary>初始化唤醒词</summary>
        public void WakeupInit()
        {
            if(m_SpeechBase != null) m_SpeechBase.WakeupInit();
        }

        /// <summary>开始唤醒词功能</summary>
        public void WakeupStart(string wakeUpPath)
        {
            if (m_SpeechBase != null) m_SpeechBase.WakeupStart(wakeUpPath);
        }

        /// <summary>停止唤醒词</summary>
        public void WakeupStop()
        {
            if (m_SpeechBase != null) m_SpeechBase.WakeupStop();
        }

        /// <summary>释放离线命令词算法</summary>
        public void WakeupDispose()
        {
            if (m_SpeechBase != null) m_SpeechBase.WakeupDispose();
        }

        //----------------------------------------事件回调----------------------------------------

        /// <summary>平台发送给Unity的消息</summary>
        public void OnMessage(string msg)
        {
            Debug.Log("MSG:" + msg);
            try
            {
                PlatformMessageInfo platformMessageInfo = JsonUtility.FromJson<PlatformMessageInfo>(msg);
                PlatformMessage(platformMessageInfo);
            }
            catch (Exception e)
            {
                Debug.LogError(GetType() + "/OnMessage()/接收到平台消息有误！msg:" + msg+ "---Exception:"+e);
            }
        }

        //平台消息
        private void PlatformMessage(PlatformMessageInfo platformMessageInfo)
        {
            switch ((MessageCode)platformMessageInfo.msgCode)
            {
                case MessageCode.None:
                case MessageCode.Log: Debug.Log(GetType() + "/PlatformMessage()/" + platformMessageInfo.Content); break;
                case MessageCode.Warning: Debug.LogWarning(GetType() + "/PlatformMessage()/" + platformMessageInfo.Content); break;
                case MessageCode.Error: Debug.LogError(GetType() + "/PlatformMessage()/" + platformMessageInfo.Content); break;
                case MessageCode.OnSpeechCallback: OnSpeechCallback(platformMessageInfo.Content); break;
                case MessageCode.OnWakeupCallback: OnSpeechCallback(platformMessageInfo.Content); break;
                default:
                    break;
            }
        }

        /// <summary>语音识别回调</summary>
        private void OnSpeechCallback(string data)
        {
            try
            {
                CallbackMessageInfo callbackMessage = JsonUtility.FromJson<CallbackMessageInfo>(data);
                if (onSpeechEvent != null) onSpeechEvent(callbackMessage);
            }
            catch(Exception e)
            {
                Debug.LogWarning(GetType() + "/OnSpeechCallback()/解析数据失败！data:" + data + "---Exception:" + e);
            }
        }

        /// <summary>通过Web请求语音识别的回调</summary>
        public void WebSpeechCallback(CallbackMessageInfo callbackMessage)
        {
            if (onSpeechEvent != null && callbackMessage != null) onSpeechEvent(callbackMessage);
        }

    }
}