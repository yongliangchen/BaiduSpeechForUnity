using UnityEngine;

namespace BaiduSpeech
{
    /// <summary>安卓平台唤醒功能API管理</summary>
    public class WakeupForAndroid : WakeupBase
    {

#if UNITY_ANDROID && !UNITY_EDITOR

        private AndroidJavaObject m_BaiduSpeechJavaObject = null;
        private AndroidJavaObject m_WakeupJavaObject = null;

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

        /// <summary>初始化唤醒词</summary>
        public override void Init()
        {
            if (m_BaiduSpeechJavaObject != null)
            {
                if (m_WakeupJavaObject != null) OnDispose();
                m_WakeupJavaObject = m_BaiduSpeechJavaObject.Call<AndroidJavaObject>("NewWakeup");
            }
            else Debug.LogWarning(GetType() + "/Init()/ m_WakeupJavaObject is null!");
        }

        /// <summary>
        /// 开始唤醒词功能
        /// </summary>
        /// <param name="wakeUpPath">唤醒词库路径(Plugins/Android/assets路径下的.bin文件)需要加后缀名</param>
        public override void Begin(string wakeUpPath)
        {
            if (m_WakeupJavaObject != null)
            {
                string path = "assets://" + wakeUpPath;
                m_WakeupJavaObject.CallStatic("Start", path);
            }
            else Debug.LogWarning(GetType() + "/Start()/ m_WakeupJavaObject is null!");
        }


        /// <summary>停止唤醒词</summary>
        public override void Stop()
        {
            if (m_WakeupJavaObject != null)
            {
                m_WakeupJavaObject.CallStatic("Stop");
            }
            else Debug.LogWarning(GetType() + "/Stop()/ m_WakeupJavaObject is null!");
        }

        /// <summary>释放算法</summary>
        public override void Release()
        {
            if (m_WakeupJavaObject != null)
            {
                m_WakeupJavaObject.CallStatic("Release");
            }
            else Debug.LogWarning(GetType() + "/Release()/ m_WakeupJavaObject is null!");
        }

        /// <summary>释放唤醒词词算法</summary>
        public override void OnDispose()
        {
            Release();
            if (m_WakeupJavaObject != null) m_WakeupJavaObject.Dispose();
            m_WakeupJavaObject = null;
        }

#endif

    }
}