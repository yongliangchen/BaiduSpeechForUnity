using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace BaiduSpeech
{
    /// <summary>Web接口语音转文本功能API管理</summary>
    public class AsrForWeb : AsrBase
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
        public override void OnInitPlatform()
        {
            //获取麦克风设备，判断是否有麦克风设备
            if (Microphone.devices.Length > 0)
            {
                isHaveMic = true;
                currentDeviceName = Microphone.devices[0];
            }

            m_BaiduSpeechManager = FindObjectOfType<BaiduSpeechManager>();
        }

        /// <summary>初始化语音</summary>
        public override void Init()
        {
            if (isHaveMic == false || Microphone.IsRecording(currentDeviceName))
            {
                SpeechEventListenerInfo callbackMessage = new SpeechEventListenerInfo();
                callbackMessage.state = SpeechConstant.CALLBACK_EVENT_ASR_ERROR;
                AsrParams asrParams = new AsrParams();
                asrParams.error = 1;
                callbackMessage.param = JsonUtility.ToJson(asrParams);
                OnWebSpeechCallback(callbackMessage);
                Debug.LogWarning(GetType() + "/SpeechInit()/当前设备没有麦克风！");
            }
            else
            {
                SpeechEventListenerInfo callbackMessage = new SpeechEventListenerInfo();
                callbackMessage.state = SpeechConstant.CALLBACK_EVENT_ASR_READY;
                AsrParams asrParams = new AsrParams();
                callbackMessage.param = JsonUtility.ToJson(asrParams);
                OnWebSpeechCallback(callbackMessage);
            }
        }

        /// <summary>加载离线命令词功能（只支持Android和iOS设备）</summary>
        public override void LoadOfflineEngine(string json)
        {
            Debug.LogWarning(GetType() + "/LoadOfflineEngine()/该设备不支持离线命令词功能！ 离线命令词功能只支持Android和iOS设备");
        }

        /// <summary>
        /// 开始录音
        /// </summary>
        /// <param name="json">详情请移步 https://ai.baidu.com/ai-doc/SPEECH/9k38lxfnk </param>
        public override void Begin(string json)
        {
            if (isHaveMic == false || Microphone.IsRecording(currentDeviceName))
            {
                return;
            }

            SpeechEventListenerInfo callbackMessage = new SpeechEventListenerInfo();
            callbackMessage.state = SpeechConstant.CALLBACK_EVENT_ASR_PARTIAL;
            AsrParams asrParams = new AsrParams();
            callbackMessage.param = JsonUtility.ToJson(asrParams);
            OnWebSpeechCallback(callbackMessage);

            lastPressTimestamp = GetTimestampOfNowWithMillisecond();
            saveAudioClip = Microphone.Start(currentDeviceName, isLoop, recordMaxLength, recordFrequency);
        }

        /// <summary>取消本次识别，取消后将立即停止不会返回识别结果</summary>
        public override void Cancel()
        {
            if (isHaveMic == false || !Microphone.IsRecording(currentDeviceName))
            {
                return;
            }

            SpeechEventListenerInfo callbackMessage = new SpeechEventListenerInfo();
            callbackMessage.state = SpeechConstant.CALLBACK_EVENT_ASR_CANCEL;
            AsrParams asrParams = new AsrParams();
            callbackMessage.param = JsonUtility.ToJson(asrParams);
            OnWebSpeechCallback(callbackMessage);

            Microphone.End(currentDeviceName);
        }

        /// <summary>停止录音</summary>
        public override void Stop()
        {
            if (isHaveMic == false || !Microphone.IsRecording(currentDeviceName)) { return; }

            Microphone.End(currentDeviceName);
            trueLength = Mathf.CeilToInt((float)(GetTimestampOfNowWithMillisecond() - lastPressTimestamp) / 1000f);

            if (trueLength > 1)
            {
                SpeechEventListenerInfo callbackMessage = new SpeechEventListenerInfo();
                callbackMessage.state = SpeechConstant.CALLBACK_EVENT_ASR_EXIT;
                AsrParams asrParams = new AsrParams();
                callbackMessage.param = JsonUtility.ToJson(asrParams);
                OnWebSpeechCallback(callbackMessage);

                StartCoroutine(StartAsr());
            }
            else
            {
                SpeechEventListenerInfo callbackMessage = new SpeechEventListenerInfo();
                callbackMessage.state = SpeechConstant.CALLBACK_EVENT_ASR_ERROR;
                AsrParams asrParams = new AsrParams();
                asrParams.error = 1;
                callbackMessage.param = JsonUtility.ToJson(asrParams);
                OnWebSpeechCallback(callbackMessage);
                Debug.LogWarning(GetType() + "/VoiceStop()/录音时长过短！");
            }
        }

        /// <summary>释放算法</summary>
        public override void Release()
        {
            Cancel();
        }

        /// <summary>脚本删除的时候调用</summary>
        public override void OnDispose()
        {
            Release();
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
                    SpeechEventListenerInfo callbackMessage = new SpeechEventListenerInfo();
                    callbackMessage.state = SpeechConstant.CALLBACK_EVENT_ASR_ERROR;
                    AsrParams asrParams = new AsrParams();
                    asrParams.error = 1;
                    callbackMessage.param = JsonUtility.ToJson(asrParams);
                    OnWebSpeechCallback(callbackMessage);
                    Debug.LogWarning(GetType() + "/GetAccessToken()/验证错误,获取AccessToken失败！");
                }
            }
        }

        /// <summary>发起语音识别请求</summary>
        private IEnumerator StartAsr()
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

                Debug.Log(asrResult);

                SpeechEventListenerInfo callbackMessage = new SpeechEventListenerInfo();

                AsrParams asrParams = new AsrParams();

                WebAsrParams webAsrParams = JsonUtility.FromJson<WebAsrParams>(asrResult);

                OriginResult originResult = new OriginResult();
                originResult.err_no = webAsrParams.err_no;
                originResult.err_msg = webAsrParams.err_msg;
                originResult.corpus_no = webAsrParams.corpus_no;
                originResult.sn = webAsrParams.sn;
                originResult.result.word = webAsrParams.result;

                asrParams.origin_result = originResult;
                asrParams.results_recognition = webAsrParams.result;

                callbackMessage.state = SpeechConstant.CALLBACK_EVENT_ASR_PARTIAL;
                callbackMessage.param = JsonUtility.ToJson(asrParams);

                OnWebSpeechCallback(callbackMessage);
            }
            else
            {
                Debug.LogWarning(GetType() + "/StartAsr()/语音识别识别！");
            }
        }

        /// <summary>语音识别回调</summary>
        private void OnWebSpeechCallback(SpeechEventListenerInfo callbackMessage)
        {
            if (m_BaiduSpeechManager != null) m_BaiduSpeechManager.WebSpeechCallback(callbackMessage);
            else
            {
                Debug.LogWarning(GetType() + "/OnWebSpeechCallback()/ m_BaiduSpeechManager is null!");
            }
        }

    }
}