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
        public override void WakeupInit()
        {
            if (m_BaiduSpeechJavaObject != null)
            {
                if (m_WakeupJavaObject != null) OnDispose();
                m_WakeupJavaObject = m_BaiduSpeechJavaObject.Call<AndroidJavaObject>("NewWakeup");
            }
            else Debug.LogWarning(GetType() + "/WakeupInit()/ m_WakeupJavaObject is null!");
        }

        /// <summary>
        /// 开始唤醒词功能
        /// </summary>
        /// <param name="wakeUpPath">唤醒词库路径(Plugins/Android/assets路径下的.bin文件)需要加后缀名</param>
        public override void WakeupStart(string wakeUpPath)
        {
            if (m_WakeupJavaObject != null)
            {
                string path = "assets://" + wakeUpPath;
                m_WakeupJavaObject.CallStatic("StartWakeup", path);
            }
            else Debug.LogWarning(GetType() + "/WakeupStart()/ m_WakeupJavaObject is null!");
        }


        /// <summary>停止唤醒词</summary>
        public override void WakeupStop()
        {
            if (m_WakeupJavaObject != null)
            {
                m_WakeupJavaObject.CallStatic("StopWakeup");
            }
            else Debug.LogWarning(GetType() + "/WakeupStop()/ m_WakeupJavaObject is null!");
        }

        /// <summary>释放唤醒词词算法</summary>
        public override void OnDispose()
        {
            if (m_WakeupJavaObject != null) m_WakeupJavaObject.Dispose();
            m_WakeupJavaObject = null;
        }

#endif

    }
}