using UnityEngine;

namespace BaiduSpeech
{
    /// <summary>Android平台语音转文本功能API管理</summary>
    public class AsrForAndroid : AsrBase
    {

#if UNITY_ANDROID && !UNITY_EDITOR

        private AndroidJavaObject m_BaiduSpeechJavaObject = null;
        private AndroidJavaObject m_AsrJavaObject = null;

        //初始化平台
        public override void OnInitPlatform()
        {
            if (FindObjectOfType<BaiduSpeechManager>() != null)
            {
                m_BaiduSpeechJavaObject = FindObjectOfType<BaiduSpeechManager>().baiduSpeechJavaObject;
            }
            else
            {
                Debug.LogError(GetType() + "/OnInitPlatform()/初始化失败！ BaiduSpeechManager is null");
            }
        }

        /// <summary>初始化语音识别</summary>
        public override void AsrInit()
        {
            if (m_BaiduSpeechJavaObject != null)
            {
                if (m_AsrJavaObject != null) OnDispose();
                m_AsrJavaObject = m_BaiduSpeechJavaObject.Call<AndroidJavaObject>("NewAsr");
            }
            else Debug.LogWarning(GetType() + "/SpeechInit()/ m_BaiduSpeechJavaObject is null!");
        }

        /// <summary>
        /// 开始录音
        /// </summary>
        /// <param name="json"></param>
        public override void VoiceStart(string json)
        {
            if (m_AsrJavaObject != null)
            {
                m_AsrJavaObject.CallStatic("VoiceStart", json);
            }
            else Debug.LogWarning(GetType() + "/VoiceStart()/ m_AsrJavaObject is null!");
        }

        /// <summary>取消本次识别，取消后将立即停止不会返回识别结果</summary>
        public override void VoiceCancel()
        {
            if (m_AsrJavaObject != null)
            {
                m_AsrJavaObject.CallStatic("VoiceCancel");
            }
            else Debug.LogWarning(GetType() + "/VoiceCancel()/ m_AsrJavaObject is null!");
        }

        /// <summary>停止录音</summary>
        public override void VoiceStop()
        {
            if (m_AsrJavaObject != null)
            {
                m_AsrJavaObject.CallStatic("VoiceStop");
            }
            else Debug.LogWarning(GetType() + "/VoiceStop()/ m_AsrJavaObject is null!");
        }

        /// <summary>释放语音识别算法</summary>
        public override void OnDispose()
        {
            if (m_AsrJavaObject != null) m_AsrJavaObject.Dispose();
            m_AsrJavaObject = null;
        }

#endif


    }
}