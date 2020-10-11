using System;
using UnityEngine;
namespace BaiduSpeech
{
    [Serializable]
    public class PlatformMessageParams
    {
        /// <summary>消息ID</summary>
        public int msgCode;
        /// <summary>消息体</summary>
        public string Content;
        /// <summary>消息体集合</summary>
        //public string ContentArr;
    }

    /// <summary>和原生交互的回调消息</summary>
    [Serializable]
    public class SpeechEventListenerInfo
    {
        /// <summary>返回的状态</summary>
        public string state;
        /// <summary>参数数据</summary>
        public string param;
        /// <summary>缓存临时数据</summary>
        public byte[] data;
        /// <summary>缓存临时数据开始索引</summary>
        public int offset;
        /// <summary>缓存临时数据开始长度</summary>
        public int length;
    }


    [Serializable]
    public class AsrParams
    {
        /// <summary>解析后的识别结果。如无特殊情况，请取第一个结果</summary>
        public string[] results_recognition;
        /// <summary>识别结果类型</summary>
        public string result_type;
        /// <summary>最佳结果</summary>
        public string best_result;
        /// <summary>原始结果</summary>
        public OriginResult origin_result;
        /// <summary>错误码</summary>
        public int error;
    }

    [Serializable]
    public class WebAsrParams
    {
        public int err_no;
        public string err_msg;
        public long corpus_no;
        public string sn;
        public string[] result;
    }

    [Serializable]
    public enum ResultType
    {
        /// <summary>临时识别结果</summary>
        partial_result,
        /// <summary>最终结果，长语音每一句都有一个最终结果</summary>
        final_result,
        /// <summary>语义结果，在final_result后回调。语义结果的内容在(data，offset，length中）</summary>
        nlu_result,
    }

    [Serializable]
    public class OriginResult
    {
        public int err_no;
        /// <summary>错误信息</summary>
        public string err_msg;
        public long corpus_no;
        public int raf;
        /// <summary>返回结果</summary>
        public Result result=new Result();
        public string sn;
    }

    [Serializable]
    public class Result
    {
        /// <summary>文本</summary>
        public String[] word;
    }

    [Serializable]
    public class ParamsData
    {
        /// <summary>错误描述,此处固定为 success</summary>
        public string errorDesc;
        /// <summary>错误码,错误码为0表示唤醒成功，唤醒出错会在CALLBACK_EVENT_WAKEUP_ERROR 事件中</summary>
        public int errorCode;
        /// <summary>具体的唤醒词</summary>
        public string word;
    }


    [Serializable]
    public class AsrVolume
    {
        /// <summary>当前音量</summary>
        public float volume;
        /// <summary>当前音量的相对值（0-100）</summary>
        public int volume_percent;
    }

    [Serializable]
    public class PermissionsResultInfo
    {
        public int requestCode;
        public string[] permissions;
        public int[] grantResults;
    }

    public class Serializable
    {
        public static AsrParams GetAsrParams(string data)
        {
            AsrParams asrParams = JsonUtility.FromJson<AsrParams>(data);
            return asrParams;
        }

        public static AsrVolume GetAsrVolume(string data)
        {
            AsrVolume volume= JsonUtility.FromJson<AsrVolume>(data.Replace("-","_"));
            return volume;
        }

        public static ParamsData GetWakeupParams(string data)
        {
            ParamsData wakeupParams = JsonUtility.FromJson<ParamsData>(data);
            return wakeupParams;
        }
    }
}