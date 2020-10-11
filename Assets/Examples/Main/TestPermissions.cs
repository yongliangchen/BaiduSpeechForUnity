using UnityEngine;

namespace BaiduSpeech.Examples
{
    /// <summary>测试动态申请权限</summary>
    public class TestPermissions : MonoBehaviour
    {
        private BaiduSpeechManager m_BaiduSpeechManager;

        private void Start()
        {
            m_BaiduSpeechManager = FindObjectOfType<BaiduSpeechManager>();
            m_BaiduSpeechManager.RequestPermissions(100,AndroidPermission.RECORD_AUDIO);
        }
    }
}