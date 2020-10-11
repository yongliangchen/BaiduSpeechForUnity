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
        public override void Init()
        {
            if (m_BaiduSpeechJavaObject != null)
            {
                if (m_AsrJavaObject != null) OnDispose();
                m_AsrJavaObject = m_BaiduSpeechJavaObject.Call<AndroidJavaObject>("NewAsr");
            }
            else Debug.LogWarning(GetType() + "/SpeechInit()/ m_BaiduSpeechJavaObject is null!");
        }

        /// <summary>
        /// 离线命令词，在线不需要调用
        /// </summary>
        /// <param name="json">详情请移步 https://ai.baidu.com/ai-doc/SPEECH/9k38lxfnk </param>
        public override void LoadOfflineEngine(string json)
        {
            if (m_AsrJavaObject != null)
            {
                m_AsrJavaObject.CallStatic("LoadOfflineEngine", json);
            }
            else Debug.LogWarning(GetType() + "/LoadOfflineEngine()/ m_AsrJavaObject is null!");
        }

        /// <summary>
        /// 开始录音
        /// </summary>
        /// <param name="json">详情请移步 https://ai.baidu.com/ai-doc/SPEECH/9k38lxfnk </param>
        public override void Begin(string json)
        {
            if (m_AsrJavaObject != null)
            {
                m_AsrJavaObject.CallStatic("Start", json);
            }
            else Debug.LogWarning(GetType() + "/Begin()/ m_AsrJavaObject is null!");
        }

        /// <summary>取消本次识别，取消后将立即停止不会返回识别结果</summary>
        public override void Cancel()
        {
            if (m_AsrJavaObject != null)
            {
                m_AsrJavaObject.CallStatic("Cancel");
            }
            else Debug.LogWarning(GetType() + "/Cancel()/ m_AsrJavaObject is null!");
        }

        /// <summary>停止录音</summary>
        public override void Stop()
        {
            if (m_AsrJavaObject != null)
            {
                m_AsrJavaObject.CallStatic("Stop");
            }
            else Debug.LogWarning(GetType() + "/Stop()/ m_AsrJavaObject is null!");
        }

        /// <summary>释放算法</summary>
        public override void Release()
        {
            if (m_AsrJavaObject != null)
            {
                m_AsrJavaObject.CallStatic("Release");
            }
            else Debug.LogWarning(GetType() + "/Release()/ m_AsrJavaObject is null!");
        }

        /// <summary>释放语音识别算法</summary>
        public override void OnDispose()
        {
            Release();

            if (m_AsrJavaObject != null) m_AsrJavaObject.Dispose();
            m_AsrJavaObject = null;
        }

#endif


    }
}