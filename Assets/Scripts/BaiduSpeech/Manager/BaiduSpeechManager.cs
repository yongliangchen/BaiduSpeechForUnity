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

        /// <summary>语音识别事件</summary>
        public event Action<SpeechEventListenerInfo> onSpeechEventListener = null;
        /// <summary>权限请求回调</summary>
        public event Action<PermissionsResultInfo> onRequestPermissionsResult = null;

        /// <summary>语音转文本</summary>
        private AsrBase m_Asr;
        /// <summary>唤醒词</summary>
        private WakeupBase m_Wakeup;

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
#else

#if UNITY_IPHONE && !UNITY_EDITOR

            m_Asr = gameObject.AddComponent<AsrForiOS>();
            m_Wakeup = gameObject.AddComponent<WakeupForiOS>(); 
#else

            m_Asr = gameObject.AddComponent<AsrForWeb>();
            m_Wakeup = gameObject.AddComponent<WakeupForWeb>();
#endif
#endif

            SetBaiduSpeechLicenese();
        }

        private void OnDestroy()
        {
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
        }

        /// <summary>检查权限</summary>
        public bool CheckPermission(string permisson)
        {
            int havePermission = 1;

            if (baiduSpeechJavaObject != null)
            {
                havePermission = baiduSpeechJavaObject.Call<int>("CheckPermissions", permisson);
            }

            return havePermission == 0;
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
            if (m_Asr != null) m_Asr.Init();
        }

        /// <summary>
        ///  离线命令词，在线不需要调用(只支持Android和iOS设备)
        /// </summary>
        /// <param name="json">详情请移步 https://ai.baidu.com/ai-doc/SPEECH/9k38lxfnk </param>
        public void VoiceLoadOfflineEngine(string json)
        {
            if (m_Asr != null) m_Asr.LoadOfflineEngine(json);
        }

        /// <summary>
        /// 开始录音
        /// </summary>
        /// <param name="json">详情请移步 https://ai.baidu.com/ai-doc/SPEECH/9k38lxfnk </param>
        public void VoiceStart(string json)
        {
            if (m_Asr != null) m_Asr.Begin(json);
        }

        /// <summary>取消本次识别，取消后将立即停止不会返回识别结果</summary>
        public void VoiceCancel()
        {
            if (m_Asr != null) m_Asr.Cancel();
        }

        /// <summary>停止录音</summary>
        public void VoiceStop()
        {
            if (m_Asr != null) m_Asr.Stop();
        }

        /// <summary>释放算法</summary>
        public void VoiceRelease()
        {
            if (m_Asr != null) m_Asr.Release();
        }

        //----------------------------------------唤醒词----------------------------------------

        /// <summary>初始化唤醒词</summary>
        public void WakeupInit()
        {
            if (m_Wakeup != null) m_Wakeup.Init();
        }

        /// <summary>开始唤醒词功能</summary>
        public void WakeupStart(string wakeUpPath)
        {
            if (m_Wakeup != null) m_Wakeup.Begin(wakeUpPath);
        }

        /// <summary>停止唤醒词</summary>
        public void WakeupStop()
        {
            if (m_Wakeup != null) m_Wakeup.Stop();
        }

        /// <summary>释放算法</summary>
        public void WakeupRelease()
        {
            if (m_Wakeup != null) m_Wakeup.Release();
        }

        //----------------------------------------事件回调----------------------------------------

        /// <summary>平台发送给Unity的消息</summary>
        public void OnMessage(string msg)
        {
            Debug.Log("Msg:" + msg);

            try
            {
                PlatformMessageParams platformMessageParams = JsonUtility.FromJson<PlatformMessageParams>(msg);
                PlatformMessage(platformMessageParams);
            }
            catch (Exception e)
            {
                Debug.LogError(GetType() + "/OnMessage()/接收到平台消息有误！msg:" + msg + "---Exception:" + e);
            }
        }

        //平台消息
        private void PlatformMessage(PlatformMessageParams platformMessageParams)
        {
            switch ((MessageCode)platformMessageParams.msgCode)
            {
                case MessageCode.None:
                case MessageCode.Log: Debug.Log(GetType() + "/PlatformMessage()/" + platformMessageParams.Content); break;
                case MessageCode.Warning: Debug.LogWarning(GetType() + "/PlatformMessage()/" + platformMessageParams.Content); break;
                case MessageCode.Error: Debug.LogError(GetType() + "/PlatformMessage()/" + platformMessageParams.Content); break;
                case MessageCode.OnAsrCallback: OnSpeechCallback(platformMessageParams.Content); break;
                case MessageCode.OnWakeupCallback: OnSpeechCallback(platformMessageParams.Content); break;
                case MessageCode.onRequestPermissionsResult:OnRequestPermissionsResult(platformMessageParams.Content);break;
                default:
                    break;
            }
        }

        /// <summary>语音识别回调</summary>
        private void OnSpeechCallback(string json)
        {
            try
            {
                SpeechEventListenerInfo callbackMessage = JsonUtility.FromJson<SpeechEventListenerInfo>(json);
                if (onSpeechEventListener != null) onSpeechEventListener(callbackMessage);
            }
            catch (Exception e)
            {
                Debug.LogWarning(GetType() + "/OnSpeechCallback()/解析数据失败！data:" + json + "---Exception:" + e);
            }
        }

        /// <summary>通过Web请求语音识别的回调</summary>
        public void WebSpeechCallback(SpeechEventListenerInfo callbackMessage)
        {
            if (onSpeechEventListener != null && callbackMessage != null) onSpeechEventListener(callbackMessage);
        }

        /// <summary>权限请求回调</summary>
        private void OnRequestPermissionsResult(string json)
        {
            try
            {
                PermissionsResultInfo permissionsResultInfo = JsonUtility.FromJson<PermissionsResultInfo>(json);
                if (onRequestPermissionsResult != null) onRequestPermissionsResult(permissionsResultInfo);
            }
            catch (Exception e)
            {
                Debug.LogWarning(GetType() + "/OnRequestPermissionsResult()/解析数据失败！data:" + json + "---Exception:" + e);
            }
        }
    }
}