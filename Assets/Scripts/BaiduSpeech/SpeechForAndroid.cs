using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace BaiduSpeech
{

    /// <summary>安卓平台百度语音管理</summary>
    public class SpeechForAndroid : SpeechBase
    {

#if UNITY_ANDROID && !UNITY_EDITOR

    private AndroidJavaObject m_BaiduSpeechManager = null;
    private AndroidJavaObject m_SpeechJavaObject = null;
    private AndroidJavaObject m_WakeupJavaObject = null;

    //初始化平台
    public override void InitPlatform()
    {
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            m_BaiduSpeechManager = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            m_BaiduSpeechManager.Call("Init", m_BaiduSpeechManager, gameObject.name);
        }
        catch (Exception e)
        {
            Debug.LogWarning(GetType() + "/InitPlatform()/ Exception:" + e);
        }
    }

    /// <summary>释放全部算法</summary>
    public override void OnDisposeAll()
    {
        if (m_BaiduSpeechManager != null) m_BaiduSpeechManager.Dispose();
        m_BaiduSpeechManager = null;

        SpeechDispose();//释放语音识别算法
        WakeupDispose();//释放离线命令词算法
    }

    //----------------------------------------语音识别----------------------------------------

    /// <summary>初始化语音识别</summary>
    public override void SpeechInit()
    {
        if (m_BaiduSpeechManager != null)
        {
            if (m_SpeechJavaObject != null) SpeechDispose();
            m_SpeechJavaObject = m_BaiduSpeechManager.Call<AndroidJavaObject>("NewSpeech");
        }
        else Debug.LogWarning(GetType() + "/SpeechInit()/ m_BaiduSpeechManager is null!");
    }

    /// <summary>
    /// 开始录音
    /// </summary>
    /// <param name="json"></param>
    public override void VoiceStart(string json)
    {
        if (m_SpeechJavaObject != null)
        {
            m_SpeechJavaObject.CallStatic("StartVoice", json);
        }
        else Debug.LogWarning(GetType() + "/VoiceStart()/ m_SpeechJavaObject is null!");
    }

    /// <summary>取消本次识别，取消后将立即停止不会返回识别结果</summary>
    public override void VoiceCancel()
    {
        if (m_SpeechJavaObject != null)
        {
            m_SpeechJavaObject.CallStatic("CancelVoice");
        }
        else Debug.LogWarning(GetType() + "/VoiceCancel()/ m_SpeechJavaObject is null!");
    }

    /// <summary>停止录音</summary>
    public override void VoiceStop()
    {
        if (m_SpeechJavaObject != null)
        {
            m_SpeechJavaObject.CallStatic("StopVoice");
        }
        else Debug.LogWarning(GetType() + "/VoiceStop()/ m_SpeechJavaObject is null!");
    }

    /// <summary>释放语音识别算法</summary>
    public override void SpeechDispose()
    {
        if (m_SpeechJavaObject != null) m_SpeechJavaObject.Dispose();
        m_SpeechJavaObject = null;
    }

    //----------------------------------------唤醒词----------------------------------------

    /// <summary>初始化唤醒词</summary>
    public override void WakeupInit()
    {
        if (m_BaiduSpeechManager != null)
        {
            if (m_WakeupJavaObject != null) WakeupDispose();
            m_WakeupJavaObject = m_BaiduSpeechManager.Call<AndroidJavaObject>("NewWakeup");
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
    public override void WakeupDispose()
    {
        if (m_WakeupJavaObject != null) m_WakeupJavaObject.Dispose();
        m_WakeupJavaObject = null;
    }

#endif


    }
}