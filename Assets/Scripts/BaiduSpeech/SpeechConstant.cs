namespace BaiduSpeech
{
    public class SpeechConstant
    {
        /// <summary>引擎就绪，可以说话</summary>
        public const string CALLBACK_EVENT_ASR_READY = "asr.ready";
        /// <summary>检测到第一句话说话开始。SDK只有第一句话说话开始的回调，没有长语音每句话说话结束的回调。</summary>
        public const string CALLBACK_EVENT_ASR_BEGIN = "asr.begin";
        /// <summary>检测到第一句话说话结束。SDK只有第一句话说话结束的回调，没有长语音每句话说话结束的回调</summary>
        public const string CALLBACK_EVENT_ASR_END = "asr.end";
        /// <summary>一句话识别结束（可能含有错误信息）最终识别的文字结果在ASR_PARTIAL事件中</summary>
        public const string CALLBACK_EVENT_ASR_FINISH = "asr.finish";
        /// <summary>识别结果</summary>
        public const string CALLBACK_EVENT_ASR_PARTIAL = "asr.partial";
        /// <summary>长语音额外的回调，表示长语音识别结束。使用infile参数无此回调，请用ASR_EXIT 代替</summary>
        public const string CALLBACK_EVENT_ASR_LONG_SPEECH = "asr.long-speech.finish";
        /// <summary>识别结束，资源释放</summary>
        public const string CALLBACK_EVENT_ASR_EXIT = "asr.exit";
        /// <summary>PCM音频片段 回调。必须输入ACCEPT_AUDIO_DATA 参数激活</summary>
        public const string CALLBACK_EVENT_ASR_AUDIO = "asr.audio";
        /// <summary>当前音量回调。必须输入ACCEPT_AUDIO_VOLUME参数激活</summary>
        public const string CALLBACK_EVENT_ASR_VOLUME = "asr.volume";
        /// <summary>离线模型加载成功回调</summary>
        public const string CALLBACK_EVENT_ASR_LOADED = "asr.loaded";
        /// <summary>离线模型卸载成功回调</summary>
        public const string CALLBACK_EVENT_ASR_UNLOADED = "asr.unloaded";
        /// <summary>取消</summary>
        public const string CALLBACK_EVENT_ASR_CANCEL = "asr.cancel";
        /// <summary>发生错误</summary>
        public const string CALLBACK_EVENT_ASR_ERROR = "asr.error";
        /// <summary>唤醒事件</summary>
        public const string CALLBACK_EVENT_WAKEUP_SUCCESS = "wp.data";
    }

    //public const string ASR_START = "asr.start";
    //public const string ASR_STOP = "asr.stop";
    //public const string ASR_CANCEL = "asr.cancel";
    //public const string ASR_KWS_LOAD_ENGINE = "asr.kws.load";
    //public const string ASR_KWS_UNLOAD_ENGINE = "asr.kws.unload";
    //public const string ASR_UPLOAD_WORDS = "asr.upload.words";
    //public const string ASR_UPLOAD_CANCEL = "asr.upload.cancel";
    //public const string WAKEUP_START = "wp.start";
    //public const string WAKEUP_STOP = "wp.stop";
    //public const string WAKEUP_LOAD_ENGINE = "wp.load";
    //public const string WAKEUP_UNLOAD_ENGINE = "wp.unload";
    //public const string ASR_UPLOAD_CONTRACT = "asr.upload.contract";
    //public const string UPLOADER_START = "uploader.start";
    //public const string UPLOADER_CANCEL = "uploader.cancel";
    //public const string CALLBACK_EVENT_ASR_SERIALNUMBER = "asr.sn";
    //public const string CALLBACK_EVENT_ASR_LOG = "asr.log";
    //public const string CALLBACK_EVENT_UPLOAD_COMPLETE = "asr.upload.complete";
    //public const string ASR_CALLBACk_NAME = "ASR.callback";
    //public const string WAKEUP_CALLBACK_NAME = "WAKEUP.callback";
    //public const string UPLOAD_CALLBACK_NAME = "UPLOAD.callback";
    //public const string CALLBACK_ASR_STATUS = "cb.asr.status.int";
    //public const string strCALLBACK_ASR_LEVEL = "cb.asr.level.int";
    //public const string CALLBACK_ASR_RESULT = "cb.asr.result.string";
    //public const string CALLBACK_WAK_STATUS = "cb.wak.status.int";
    //public const string CALLBACK_WAK_RESULT = "cb.wak.result.string";
    //public const string CALLBACK_ERROR_DOMAIN = "cb.error.domain.int16_t";
    //public const string CALLBACK_ERROR_CODE = "cb.error.code.int16_t";
    //public const string CALLBACK_ERROR_DESC = "cb.error.desc.string";
    //public const string CALLBACK_EVENT_UNIT_FINISH = "unit.finish";
    //public const string SOUND_START = "sound_start";
    //public const string SOUND_END = "sound_end";
    //public const string SOUND_SUCCESS = "sound_success";
    //public const string SOUND_ERROR = "sound_error";
    //public const string SOUND_CANCEL = "sound_cancel";
    //public const string CALLBACK_EVENT_WAKEUP_STARTED = "wp.enter";
    //public const string CALLBACK_EVENT_WAKEUP_READY = "wp.ready";
    //public const string CALLBACK_EVENT_WAKEUP_STOPED = "wp.exit";
    //public const string CALLBACK_EVENT_WAKEUP_LOADED = "wp.loaded";
    //public const string CALLBACK_EVENT_WAKEUP_UNLOADED = "wp.unloaded";
    //public const string CALLBACK_EVENT_WAKEUP_ERROR = "wp.error";

    //public const string CALLBACK_EVENT_WAKEUP_AUDIO = "wp.audio";
    //public const string CALLBACK_EVENT_UPLOAD_FINISH = "uploader.finish";
    //public const string LOG_LEVEL = "log_level";
    //public const string LANGUAGE = "language";
    //public const string CONTACT = "contact";
    //public const string VAD = "vad";
    //public const string VAD_MFE = "mfe";
    //public const string VAD_MODEL = "model-vad";
    //public const string VAD_DNN = "dnn";
    //public const string VAD_TOUCH = "touch";
    //public const string SAMPLE_RATE = "sample";
    //public const string PAM = "decoder-server.pam";
    //public const string NLU = "nlu";
    //public const string PROP = "prop";
    //public const bool PUBLIC_DECODER = false;
    //public const string IN_FILE = "infile";
    //public const string AUDIO_MILLS = "audio.mills";
    //public const string AUDIO_SOURCE = "audio.source";
    //public const string OUT_FILE = "outfile";
    //public const string ACCEPT_AUDIO_DATA = "accept-audio-data";
    //public const string ACCEPT_AUDIO_VOLUME = "accept-audio-volume";
    //public const string APP_KEY = "key";
    //public const string SECRET = "secret";
    //public const string URL = "decoder-server.url";
    //public const string PID = "pid";
    //public const string LMID = "lm_id";
    //public const string APP_NAME = "decoder-server.app";
    //public const string URL_NEW = "https://vse.baidu.com/v2";
    //public const string URL_OLD = "https://vse.baidu.com/echo.fcgi";
    //public const string DEC_TYPE = "dec-type";
    //public const string DECODER = "decoder";
    //public const string ASR_VAD_RES_FILE_PATH = "vad.res-file";
    //public const string VAD_ENDPOINT_TIMEOUT = "vad.endpoint-timeout";
    //public const string ASR_OFFLINE_ENGINE_GRAMMER_FILE_PATH = "grammar";
    //public const string ASR_OFFLINE_ENGINE_DAT_FILE_PATH = "asr-base-file-path";
    //public const string ASR_OFFLINE_ENGINE_LICENSE_FILE_PATH = "license-file-path";
    //public const string SLOT_DATA = "slot-data";
    //public const string DISABLE_PUNCTUATION = "disable-punctuation";
    //public const string KWS_TYPE = "kws-type";
    //public const string APP_ID = "appid";
    //public const string DEV = "dev";
    //public const string WP_DAT_FILEPATH = "wakeup_dat_filepath";
    //public const string WP_WORDS_FILE = "kws-file";
    //public const string WP_WORDS = "words";
    //public const string ENABLE_HTTPDNS = "enable-httpdns";
    //public const string WP_VAD_ENABLE = "wp.vad_enable";
    //public const string WP_ENGINE_LICENSE_FILE_PATH = "license-file-path";
    //public const string BOT_SESSION_LIST = "bot_session_list";
    //public const string BOT_ID = "bot_id";
    //public const string BOT_SESSION = "bot_session";
    //public const string BOT_SESSION_ID = "bot_session_id";
    //public const string ASR_ENABLE_NUMBERFORMAT = "enable.numberformat";
    //public const string ASR_PUNCTUATION_MODE = "punctuation-mode";

}