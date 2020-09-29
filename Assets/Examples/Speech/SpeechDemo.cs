using UnityEngine;
using UnityEngine.UI;

namespace BaiduSpeech.Examples
{
    /// <summary>测试语音功能</summary>
    public class SpeechDemo : MonoBehaviour
    {
        public Text content;
        public Slider volumeSlider;
        public Text volumeText;
        public Text volumeText2;
        public Text stateText;

        private BaiduSpeechManager m_BaiduSpeechManager;

        private void Start()
        {
            stateText.text = "初始化语音识别!";

            m_BaiduSpeechManager = FindObjectOfType<BaiduSpeechManager>();
            m_BaiduSpeechManager.SpeechInit();//初始化语音识别
            m_BaiduSpeechManager.onSpeechEvent += OnSpeechEvent;
        }

        private void OnDestroy()
        {
            m_BaiduSpeechManager.onSpeechEvent -= OnSpeechEvent;
        }

        /// <summary>开始说话</summary>
        public void VoiceStart()
        {
            //string data = "{\"accept-audio-data\":false,\"disable-punctuation\":false,\"accept-audio-volume\":true,\"vad.endpoint-timeout\":0,\"pid\":1537}";
            string data = "{\"accept-audio-data\":false,\"disable-punctuation\":false,\"accept-audio-volume\":true,\"pid\":1537}";
            m_BaiduSpeechManager.VoiceStart(data);

            content.text = null;
        }

        /// <summary>结束说话</summary>
        public void VoiceEnd()
        {
            m_BaiduSpeechManager.VoiceStop();
        }

        /// <summary>百度语音识别事件</summary>
        private void OnSpeechEvent(CallbackMessageInfo callbackMessage)
        {
            string state = callbackMessage.state;
            string paramsData = callbackMessage.paramsData;

            // 引擎就绪，可以说话，一般在收到此事件后通过UI通知用户可以说话了
            if (state.Equals(SpeechConstant.CALLBACK_EVENT_ASR_READY))
            {
                stateText.text = "引擎就绪，可以说话!";
                Debug.Log("引擎就绪，可以说话");
            }

            // 识别结果
            if (state.Equals(SpeechConstant.CALLBACK_EVENT_ASR_PARTIAL))
            {
                stateText.text = "识别结果！";

                Debug.Log("state:" + state + "---" + "params:" + paramsData);

                SpeechParams speechParams = Serializable.GetSpeechParams(paramsData);

                if (speechParams.results_recognition.Length > 0) Debug.Log("results_recognition:" + speechParams.results_recognition[0]);
                Debug.Log("result_type:" + speechParams.result_type);
                Debug.Log("best_result:" + speechParams.best_result);
                Debug.Log("corpus_no:" + speechParams.origin_result.corpus_no);
                Debug.Log("err_no:" + speechParams.origin_result.err_no);
                Debug.Log("raf:" + speechParams.origin_result.raf);
                Debug.Log("sn:" + speechParams.origin_result.sn);
                if(speechParams.origin_result.result.word.Length>0) Debug.Log("result:" + speechParams.origin_result.result.word[0]);
                Debug.Log("error:" + speechParams.error);

                if (speechParams.results_recognition.Length > 0) content.text = speechParams.results_recognition[0];
            }

            // 当前音量
            if (state.Equals(SpeechConstant.CALLBACK_EVENT_ASR_VOLUME))
            {
                AsrVolume asrVolume = Serializable.GetAsrVolume(paramsData);
                volumeSlider.value = asrVolume.volume_percent;
                volumeText.text = asrVolume.volume_percent.ToString();
                volumeText2.text = asrVolume.volume.ToString();

                Debug.Log("volume:" + asrVolume.volume);
                Debug.Log("volume_percent:" + asrVolume.volume_percent);
            }

            //一句话识别结束
            if(state.Equals(SpeechConstant.CALLBACK_EVENT_ASR_FINISH))
            {
                stateText.text = "一句话识别结束！";
            }

            //一句话识别结束
            if (state.Equals(SpeechConstant.CALLBACK_EVENT_ASR_END))
            {
                stateText.text = "第一句说话结束！";
            }

            //识别结束释放资源
            if (state.Equals(SpeechConstant.CALLBACK_EVENT_ASR_EXIT))
            {
                stateText.text = "识别结束释放资源！";
                volumeSlider.value =0;
                volumeText.text ="0";
                volumeText2.text ="0";
            }

            //发生错误
            if (state.Equals(SpeechConstant.CALLBACK_EVENT_ASR_ERROR))
            {
                stateText.text = "发生错误！";
                volumeSlider.value = 0;
                volumeText.text = "0";
                volumeText2.text = "0";
            }

            //Debug.Log("state:"+ state+"---"+ "params:"+ paramsData);

        }
    }
}