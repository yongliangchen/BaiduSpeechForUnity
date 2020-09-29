using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace BaiduSpeech
{
    /// <summary>其他平台百度语音管理</summary>
    public class SpeechForOther : SpeechBase
    {
        private BaiduSpeechManager m_BaiduSpeechManager;

        /// <summary>记录accesstoken令牌</summary>
        private string accessToken = string.Empty;
        /// <summary>百度请求令牌API地址</summary>
        private const string ACCESS_TOKEN_API_URL = "https://openapi.baidu.com/oauth/2.0/token?grant_type=client_credentials&client";
        /// <summary>标记是否有麦克风</summary>
        private bool isHaveMic = false;
        /// <summary>当前录音设备名称</summary>
        private string currentDeviceName = string.Empty;
        /// <summary>录音频率,控制录音质量(8000,16000)</summary>
        private int recordFrequency = 8000;
        /// <summary>上次按下时间戳</summary>
        private double lastPressTimestamp = 0;
        /// <summary>表示录音的最大时长</summary>
        private int recordMaxLength = 10;
        /// <summary>实际录音长度</summary>
        private int trueLength = 0;
        /// <summary>是否循环</summary>
        private bool isLoop = false;
        private AudioClip saveAudioClip;

        //初始化平台
        public override void InitPlatform()
        {
            //获取麦克风设备，判断是否有麦克风设备
            if (Microphone.devices.Length > 0)
            {
                isHaveMic = true;
                currentDeviceName = Microphone.devices[0];
            }

            m_BaiduSpeechManager = FindObjectOfType<BaiduSpeechManager>();
        }

        //----------------------------------------语音识别----------------------------------------

        /// <summary>初始化语音</summary>
        public override void SpeechInit()
        {
            if (isHaveMic == false || Microphone.IsRecording(currentDeviceName))
            {
                CallbackMessageInfo callbackMessage = new CallbackMessageInfo();
                callbackMessage.state = SpeechConstant.CALLBACK_EVENT_ASR_ERROR;
                SpeechParams speechParams = new SpeechParams();
                speechParams.error = 1;
                callbackMessage.paramsData = JsonUtility.ToJson(speechParams);
                OnWebSpeechCallback(callbackMessage);
                Debug.LogWarning(GetType() + "/SpeechInit()/当前设备没有麦克风！");
            }
            else
            {
                CallbackMessageInfo callbackMessage = new CallbackMessageInfo();
                callbackMessage.state = SpeechConstant.CALLBACK_EVENT_ASR_READY;
                SpeechParams speechParams = new SpeechParams();
                callbackMessage.paramsData = JsonUtility.ToJson(speechParams);
                OnWebSpeechCallback(callbackMessage);
            }
        }

        /// <summary>
        /// 开始录音
        /// </summary>
        /// <param name="json"></param>
        public override void VoiceStart(string json)
        {
            if (isHaveMic == false || Microphone.IsRecording(currentDeviceName))
            {
                return;
            }

            CallbackMessageInfo callbackMessage = new CallbackMessageInfo();
            callbackMessage.state = SpeechConstant.CALLBACK_EVENT_ASR_PARTIAL;
            SpeechParams speechParams = new SpeechParams();
            callbackMessage.paramsData = JsonUtility.ToJson(speechParams);
            OnWebSpeechCallback(callbackMessage);

            lastPressTimestamp = GetTimestampOfNowWithMillisecond();
            saveAudioClip = Microphone.Start(currentDeviceName, isLoop, recordMaxLength, recordFrequency);
        }

        /// <summary>取消本次识别，取消后将立即停止不会返回识别结果</summary>
        public override void VoiceCancel()
        {
            if (isHaveMic == false || !Microphone.IsRecording(currentDeviceName))
            {
                return;
            }

            CallbackMessageInfo callbackMessage = new CallbackMessageInfo();
            callbackMessage.state = SpeechConstant.CALLBACK_EVENT_ASR_CANCEL;
            SpeechParams speechParams = new SpeechParams();
            callbackMessage.paramsData = JsonUtility.ToJson(speechParams);
            OnWebSpeechCallback(callbackMessage);

            Microphone.End(currentDeviceName);
        }

        /// <summary>停止录音</summary>
        public override void VoiceStop()
        {
            if (isHaveMic == false || !Microphone.IsRecording(currentDeviceName))
            {
                return;
            }

            Microphone.End(currentDeviceName);
            trueLength = Mathf.CeilToInt((float)(GetTimestampOfNowWithMillisecond() - lastPressTimestamp) / 1000f);

            if (trueLength > 1)
            {
                CallbackMessageInfo callbackMessage = new CallbackMessageInfo();
                callbackMessage.state = SpeechConstant.CALLBACK_EVENT_ASR_EXIT;
                SpeechParams speechParams = new SpeechParams();
                callbackMessage.paramsData = JsonUtility.ToJson(speechParams);
                OnWebSpeechCallback(callbackMessage);

                StartCoroutine(StartBaiduYuYin());
            }
            else
            {
                CallbackMessageInfo callbackMessage = new CallbackMessageInfo();
                callbackMessage.state = SpeechConstant.CALLBACK_EVENT_ASR_ERROR;
                SpeechParams speechParams = new SpeechParams();
                speechParams.error = 1;
                callbackMessage.paramsData = JsonUtility.ToJson(speechParams);
                OnWebSpeechCallback(callbackMessage);
                Debug.LogWarning(GetType() + "/VoiceStop()/录音时长过短！");
            }
        }

        /// <summary>获取毫秒级别的时间戳,用于计算按下录音时长</summary>
        private double GetTimestampOfNowWithMillisecond()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        /// <summary>获取accessToken请求令牌</summary>
        private IEnumerator GetAccessToken()
        {
            var uri = string.Format(ACCESS_TOKEN_API_URL + "_id={0}&client_secret={1}", apiKey, secretKey);
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(uri);
            yield return unityWebRequest.SendWebRequest();
            if (unityWebRequest.isDone)
            {
                Match match = Regex.Match(unityWebRequest.downloadHandler.text, @"access_token.:.(.*?).,");
                if (match.Success)
                {
                    accessToken = match.Groups[1].ToString();
                }
                else
                {
                    CallbackMessageInfo callbackMessage = new CallbackMessageInfo();
                    callbackMessage.state = SpeechConstant.CALLBACK_EVENT_ASR_ERROR;
                    SpeechParams speechParams = new SpeechParams();
                    speechParams.error = 1;
                    callbackMessage.paramsData = JsonUtility.ToJson(speechParams);
                    OnWebSpeechCallback(callbackMessage);
                    Debug.LogWarning(GetType() + "/GetAccessToken()/验证错误,获取AccessToken失败！");
                }
            }
        }

        /// <summary>发起语音识别请求</summary>
        private IEnumerator StartBaiduYuYin()
        {
            if (string.IsNullOrEmpty(accessToken)) { yield return GetAccessToken(); }

            string asrResult = string.Empty;

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

                //OriginResult originResult = JsonUtility.FromJson<OriginResult>(asrResult);
                //CallbackMessageInfo callbackMessage = new CallbackMessageInfo();
                //callbackMessage.state = SpeechConstant.CALLBACK_EVENT_ASR_PARTIAL;
                //SpeechParams speechParams = new SpeechParams();
                //speechParams.results_recognition = originResult.result.word;
                //if(originResult.result.word!=null&&originResult.result.word.Length>0) speechParams.best_result = originResult.result.word[0];
                //speechParams.error = originResult.err_no;
                //speechParams.origin_result = originResult;
                //callbackMessage.paramsData = JsonUtility.ToJson(speechParams);
                //OnWebSpeechCallback(callbackMessage);

                Debug.Log(asrResult);
            }
        }

        //----------------------------------------唤醒词----------------------------------------

        /// <summary>初始化唤醒词</summary>
        public override void WakeupInit()
        {

        }

        /// <summary>
        /// 开始唤醒词功能
        /// </summary>
        /// <param name="wakeUpPath">唤醒词库路径</param>
        public override void WakeupStart(string wakeUpPath)
        {

        }

        /// <summary>语音识别回调</summary>
        private void OnWebSpeechCallback(CallbackMessageInfo callbackMessage)
        {
            if (m_BaiduSpeechManager != null) m_BaiduSpeechManager.WebSpeechCallback(callbackMessage);
            else
            {
                Debug.LogWarning(GetType() + "/OnWebSpeechCallback()/ m_BaiduSpeechManager is null!");
            }
        }

    }
}