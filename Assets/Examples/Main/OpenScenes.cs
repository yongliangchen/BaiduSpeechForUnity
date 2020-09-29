using UnityEngine;
using UnityEngine.SceneManagement;

namespace BaiduSpeech.Examples
{
    /// <summary>打开场景</summary>
    public class OpenScenes : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}