using UnityEngine;

namespace BaiduSpeech
{
    /// <summary>Web接口唤醒功能API管理</summary>
    public class WakeupForWeb : WakeupBase
    {
        //初始化平台
        public override void OnInitPlatform()
        {
            Debug.LogWarning(GetType() + "/OnInitPlatform()/该设备不支持唤醒词功能! 唤醒词功能只支持Android和iOS设备");
        }
    }
}