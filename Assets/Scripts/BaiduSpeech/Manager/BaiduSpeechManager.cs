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
        /// <summary>语音转文本</summary>
        private AsrBase m_Asr;
        /// <summary>唤醒词</summary>
        private WakeupBase m_Wakeup;
        /// <summary>文本转语音</summary>
        private TtsBase m_Tts;

        /// <summary>百度语音管理Java类</summary>
        public AndroidJavaObject baiduSpeechJavaObject { get; set; }

        private void Awake()
        {

#if UNITY_ANDROID && !UNITY_EDITOR

            try
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                baiduSpeechJavaObject = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                baiduSpeechJavaObject.Call("Init", baiduSpeechJavaObject, gameObject.name);
            }
            catch (Exception e)
            {
                Debug.LogWarning(GetType() + "/Awake()/ Exception:" + e);
            }

            m_Asr = gameObject.AddComponent<AsrForAndroid>();
            m_Wakeup = gameObject.AddComponent<WakeupForAndroid>();
            m_Tts = gameObject.AddComponent<TtsForAndroid>();

#else

#if UNITY_IPHONE && !UNITY_EDITOR

            m_Asr = gameObject.AddComponent<AsrForiOS>();
            m_Wakeup = gameObject.AddComponent<WakeupForiOS>();
            m_Tts = gameObject.AddComponent<TtsForiOS>();

#else

            m_Asr = gameObject.AddComponent<AsrForWeb>();
            m_Wakeup = gameObject.AddComponent<WakeupForWeb>();
            m_Tts = gameObject.AddComponent<TtsForWeb>();

#endif
#endif

            SetBaiduSpeechLicenese();
        }

        private void OnDestroy()
        {
            if (m_Asr != null) m_Asr.OnDispose();
            if (m_Wakeup != null) m_Wakeup.OnDispose();
            if (m_Tts != null) m_Tts.OnDispose();

            if (baiduSpeechJavaObject != null)
            {
                baiduSpeechJavaObject.Dispose();
                baiduSpeechJavaObject = null;
            }
        }

        /// <summary>设置百度语音的许可证</summary>
        private void SetBaiduSpeechLicenese()
        {
            if (m_Asr != null)
            {
                m_Asr.appId = APP_ID;
                m_Asr.apiKey = API_KEY;
                m_Asr.secretKey = SECRET_KEY;
            }

            if (m_Wakeup != null)
            {
                m_Wakeup.appId = APP_ID;
                m_Wakeup.apiKey = API_KEY;
                m_Wakeup.secretKey = SECRET_KEY;
            }

            if (m_Tts != null)
            {
                m_Tts.appId = APP_ID;
                m_Tts.apiKey = API_KEY;
                m_Tts.secretKey = SECRET_KEY;
            }
        }

        /// <summary>检查权限</summary>
        public bool CheckPermissions(params string[] permissons)
        {
            bool havePermission = false;

            if(baiduSpeechJavaObject!=null)
            {
                havePermission = baiduSpeechJavaObject.Call<bool>("CheckPermissions", permissons);
            }

            return havePermission;
        }

        /// <summary>
        /// 请求权限
        /// </summary>
        /// <param name="requestCode">请求权限ID</param>
        /// <param name="permissons">权限列表</param>
        public void RequestPermissions(int requestCode, params string[] permissons)
        {
            if (baiduSpeechJavaObject != null)
            {
                baiduSpeechJavaObject.Call("RequestPermissions", requestCode, permissons);
            }
        }

        //----------------------------------------语音转文本----------------------------------------

        /// <summary>初始化语音转文本功能</summary>
        public void AsrInit()
        {
            if (m_Asr != null) m_Asr.AsrInit();
        }

        /// <summary>开始录音</summary>
        public void VoiceStart(string json)
        {
            if (m_Asr != null) m_Asr.VoiceStart(json);
        }

        /// <summary>取消本次识别，取消后将立即停止不会返回识别结果</summary>
        public void VoiceCancel()
        {
            if (m_Asr != null) m_Asr.VoiceCancel();
        }

        /// <summary>停止录音</summary>
        public void VoiceStop()
        {
            if (m_Asr != null) m_Asr.VoiceStop();
        }

        /// <summary>释放语音识别算法</summary>
        public void AsrDispose()
        {
            if (m_Asr != null) m_Asr.OnDispose();
        }

        //----------------------------------------唤醒词----------------------------------------

        /// <summary>初始化唤醒词</summary>
        public void WakeupInit()
        {
            if (m_Wakeup != null) m_Wakeup.WakeupInit();
        }

        /// <summary>开始唤醒词功能</summary>
        public void WakeupStart(string wakeUpPath)
        {
            if (m_Wakeup != null) m_Wakeup.WakeupStart(wakeUpPath);
        }

        /// <summary>停止唤醒词</summary>
        public void WakeupStop()
        {
            if (m_Wakeup != null) m_Wakeup.WakeupStop();
        }

        /// <summary>释放离线命令词算法</summary>
        public void WakeupDispose()
        {
            if (m_Wakeup != null) m_Wakeup.OnDispose();
        }

        //----------------------------------------事件回调----------------------------------------

        /// <summary>平台发送给Unity的消息</summary>
        public void OnMessage(string msg)
        {
            Debug.Log("Msg:" + msg);

            try
            {
                PlatformMessageInfo platformMessageInfo = JsonUtility.FromJson<PlatformMessageInfo>(msg);
                PlatformMessage(platformMessageInfo);
            }
            catch (Exception e)
            {
                Debug.LogError(GetType() + "/OnMessage()/接收到平台消息有误！msg:" + msg + "---Exception:" + e);
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
            catch (Exception e)
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