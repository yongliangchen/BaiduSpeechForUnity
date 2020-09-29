using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

//RequireComponent的这两个组件主要用于播放自己录制的声音,不需要刻意删除,同时注意删除使用组件的代码
[RequireComponent(typeof(AudioListener)), RequireComponent(typeof(AudioSource))]
public class BaiduASR : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    //百度语音识别相关key
    string appId = "21554025";
    string apiKey = "oihuTZ4kIwWctUmnnTIj39YA";              //填写自己的apiKey
    string secretKey = "wMCLOlTWSrzPbHz9cyWEZnpTGfoV78Yd";         //填写自己的secretKey

    //记录accesstoken令牌
    string accessToken = string.Empty;

    //语音识别的结果
    string asrResult = string.Empty;

    //标记是否有麦克风
    private bool isHaveMic = false;

    //当前录音设备名称
    string currentDeviceName = string.Empty;

    //录音频率,控制录音质量(8000,16000)
    int recordFrequency = 8000;

    //上次按下时间戳
    double lastPressTimestamp = 0;

    //表示录音的最大时长
    int recordMaxLength = 10;

    //实际录音长度(由于unity的录音需先指定长度,导致识别上传时候会上传多余的无效字节)
    //通过该字段,获取有效录音长度,上传时候剪切到无效的字节数据即可
    int trueLength = 0;

    //存储录音的片段
    [HideInInspector]
    public AudioClip saveAudioClip;

    //当前按钮下的文本
    public Text textBtn;

    //显示结果的文本
    public Text textResult;

    //音源
    AudioSource audioSource;

    void Start()
    {
        //获取麦克风设备，判断是否有麦克风设备
        if (Microphone.devices.Length > 0)
        {
            isHaveMic = true;
            currentDeviceName = Microphone.devices[0];
        }

        //获取相关组件
        //textBtn = this.transform.GetChild(0).GetComponent<Text>();
        audioSource = this.GetComponent<AudioSource>();
        //textResult = this.transform.parent.GetChild(1).GetComponent<Text>();
    }

    /// <summary>
    /// 开始录音
    /// </summary>
    /// <param name="isLoop"></param>
    /// <param name="lengthSec"></param>
    /// <param name="frequency"></param>
    /// <returns></returns>
    public bool StartRecording(bool isLoop = false) //8000,16000
    {
        if (isHaveMic == false || Microphone.IsRecording(currentDeviceName))
        {
            return false;
        }

        //开始录音
        /*
         * public static AudioClip Start(string deviceName, bool loop, int lengthSec, int frequency);
         * deviceName   录音设备名称.
         * loop         如果达到长度,是否继续记录
         * lengthSec    指定录音的长度.
         * frequency    音频采样率   
         */

        lastPressTimestamp = GetTimestampOfNowWithMillisecond();

        saveAudioClip = Microphone.Start(currentDeviceName, isLoop, recordMaxLength, recordFrequency);

        return true;
    }

    /// <summary>
    /// 录音结束,返回实际的录音时长
    /// </summary>
    /// <returns></returns>
    public int EndRecording()
    {
        if (isHaveMic == false || !Microphone.IsRecording(currentDeviceName))
        {
            return 0;
        }

        //结束录音
        Microphone.End(currentDeviceName);

        //向上取整,避免遗漏录音末尾
        return Mathf.CeilToInt((float)(GetTimestampOfNowWithMillisecond() - lastPressTimestamp) / 1000f);
    }

    /// <summary>
    /// 获取毫秒级别的时间戳,用于计算按下录音时长
    /// </summary>
    /// <returns></returns>
    public double GetTimestampOfNowWithMillisecond()
    {
        return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
    }

    /// <summary>
    /// 按下录音按钮
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        textBtn.text = "松开识别";
        StartRecording();
    }

    /// <summary>
    /// 放开录音按钮
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        textBtn.text = "按住说话";
        trueLength = EndRecording();
        if (trueLength > 1)
        {
            audioSource.PlayOneShot(saveAudioClip);
            StartCoroutine(_StartBaiduYuYin());
        }
        else
        {
            textResult.text = "录音时长过短";
        }
    }

    /// <summary>
    /// 获取accessToken请求令牌
    /// </summary>
    /// <returns></returns>
    IEnumerator _GetAccessToken()
    {
        var uri =
            string.Format(
                "https://openapi.baidu.com/oauth/2.0/token?grant_type=client_credentials&client_id={0}&client_secret={1}",
                apiKey, secretKey);
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(uri);
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.isDone)
        {
            //这里可以考虑用Json,本人比较懒所以用正则匹配出accessToken
            Match match = Regex.Match(unityWebRequest.downloadHandler.text, @"access_token.:.(.*?).,");
            if (match.Success)
            {
                //表示正则匹配到了accessToken
                accessToken = match.Groups[1].ToString();
            }
            else
            {
                textResult.text = "验证错误,获取AccessToken失败!!!";
            }
        }
    }

    /// <summary>
    /// 发起语音识别请求
    /// </summary>
    /// <returns></returns>
    IEnumerator _StartBaiduYuYin()
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            yield return _GetAccessToken();
        }

        asrResult = string.Empty;

        //处理当前录音数据为PCM16
        float[] samples = new float[recordFrequency * trueLength * saveAudioClip.channels];
        saveAudioClip.GetData(samples, 0);
        var samplesShort = new short[samples.Length];
        for (var index = 0; index < samples.Length; index++)
        {
            samplesShort[index] = (short)(samples[index] * short.MaxValue);
        }
        byte[] datas = new byte[samplesShort.Length * 2];
        Buffer.BlockCopy(samplesShort, 0, datas, 0, datas.Length);

        string url = string.Format("{0}?cuid={1}&token={2}", "https://vop.baidu.com/server_api", SystemInfo.deviceUniqueIdentifier, accessToken);

        WWWForm wwwForm = new WWWForm();
        wwwForm.AddBinaryData("audio", datas);

        UnityWebRequest unityWebRequest = UnityWebRequest.Post(url, wwwForm);

        unityWebRequest.SetRequestHeader("Content-Type", "audio/pcm;rate=" + recordFrequency);

        yield return unityWebRequest.SendWebRequest();

        if (string.IsNullOrEmpty(unityWebRequest.error))
        {
            asrResult = unityWebRequest.downloadHandler.text;
            if (Regex.IsMatch(asrResult, @"err_msg.:.success"))
            {
                Match match = Regex.Match(asrResult, "result.:..(.*?)..]");
                if (match.Success)
                {
                    asrResult = match.Groups[1].ToString();
                }
            }
            else
            {
                asrResult = "识别结果为空";
            }
            textResult.text = asrResult;
        }
    }
}

