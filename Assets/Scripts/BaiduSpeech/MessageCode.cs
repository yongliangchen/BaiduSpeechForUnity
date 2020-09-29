namespace BaiduSpeech
{
    /// <summary>消息码</summary>
    public enum MessageCode
    {
        None,//未知消息
        Log,//日记
        Warning,//警告
        Error,//发生错误
        OnHavePermissions,//用户已经有了权限
        OnHaveAllPermissions,//用户已经有了全部权限
        OnNoPermissions,//用户没有权限
        OnPermissionSuccess,//授权成功回调
        OnPermissionFail,//授权失败回调
        OnWakeupCallback,//唤醒词回调
        OnSpeechCallback,//语音识别回调
    }
}