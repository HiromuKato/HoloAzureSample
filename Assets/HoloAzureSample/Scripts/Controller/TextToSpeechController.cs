using UnityEngine;

namespace HoloAzureSample.TextToSpeech
{
    /// <summary>
    /// TextToSpeechのコントローラークラス
    /// </summary>
    public class TextToSpeechController : MonoBehaviour
    {
        /// <summary>
        /// TextToSpeechビュー
        /// </summary>
        [SerializeField]
        private TextToSpeechView view;

        /// <summary>
        /// TextToSpeechモデル
        /// </summary>
        [SerializeField]
        private TextToSpeechModel model;

        /// <summary>
        /// TextToSpeech API
        /// </summary>
        [SerializeField]
        private TextToSpeechApi api;

        /// <summary>
        /// 初期化処理
        /// </summary>
        void Start()
        {
            // コールバックへの登録
            api.Callback = OnReceiveResponse;
            api.ErrorCallback = OnReceiveErrorResponse;

            // テキスト→音声変換のリクエスト送信
            StartCoroutine(api.GetTokenRequest());
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        private void OnDestroy()
        {
            api.Callback = null;
            api.ErrorCallback = null;
        }

        /// <summary>
        /// レスポンスを受信したときの処理
        /// </summary>
        /// <param name="response">レスポンス(オーディオデータ)</param>
        private void OnReceiveResponse(AudioClip response)
        {
            model.audioClip = response;
            view.PlayAudio(model.audioClip);
        }

        /// <summary>
        /// エラーレスポンスを受信したときの処理
        /// </summary>
        /// <param name="response">レスポンス</param>
        private void OnReceiveErrorResponse(string response)
        {
            Debug.Log(response);
        }

    } // class TextToSpeechController
} // namespace HoloAzureSample.TextToSpeech