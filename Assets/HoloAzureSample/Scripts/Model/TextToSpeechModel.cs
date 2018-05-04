using UnityEngine;

namespace HoloAzureSample.TextToSpeech
{
    /// <summary>
    /// TextToSpeechのモデルクラス
    /// </summary>
    public class TextToSpeechModel : MonoBehaviour
    {
        /// <summary>
        /// テキスト→音声変換したオーディオデータ
        /// </summary>
        public AudioClip audioClip { get; set; }

    } // class TextToSpeechModel
} // namespace HoloAzureSample.TextToSpeech
