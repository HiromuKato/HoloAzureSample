using UnityEngine;

namespace HoloAzureSample.TextToSpeech
{
    /// <summary>
    /// TextToSpeechのビュークラス
    /// </summary>
    public class TextToSpeechView : MonoBehaviour
    {
        /// <summary>
        /// 音源
        /// </summary>
        [SerializeField]
        private AudioSource audioSource;

        // オーディオを再生する
        public void PlayAudio(AudioClip audioClip)
        {
            audioSource.clip = audioClip;
            audioSource.Play();

            /*
            // 音声データをファイルへ保存
            string filepath;
            byte[] tmpbytes = WavUtility.FromAudioClip(audioSource.clip, out filepath, true);
            Debug.Log(filepath);
            */
        }

    } // class TextToSpeechView
} // namespace HoloAzureSample.TextToSpeech