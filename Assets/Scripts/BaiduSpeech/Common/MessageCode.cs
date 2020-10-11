namespace BaiduSpeech
{
    /// <summary>消息码</summary>
    public enum MessageCode
    {
        None,//未知消息
        Log,//日记
        Warning,//警告
        Error,//发生错误
        onRequestPermissionsResult,//请求权限回调结果
        OnWakeupCallback,//唤醒词回调
        OnAsrCallback,//语音识别回调
    }
}