using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace HoloAzureSample.SpeechToText
{
    /// <summary>
    /// Bing Speech APIを利用して音声→テキストの変換処理を行うクラス
    /// 参考：https://docs.microsoft.com/ja-jp/azure/cognitive-services/speech/home
    /// </summary>
    public class SpeechToTextApi : MonoBehaviour
    {
        /// <summary>
        /// コールバック定義
        /// </summary>
        /// <param name="response">レスポンス</param>
        public delegate void SpeechToTextApiCallback(string response);
        /// <summary>
        /// レスポンスを取得したときに呼ぶコールバック
        /// </summary>
        public SpeechToTextApiCallback Callback { get; set; }

        /// <summary>
        /// エラー時のコールバック定義
        /// </summary>
        /// <param name="response">レスポンス</param>
        public delegate void SpeechToTextApiErrorCallback(string response);
        /// <summary>
        /// エラーレスポンスを取得したときに呼ぶコールバック
        /// </summary>
        public SpeechToTextApiErrorCallback ErrorCallback { get; set; }

        /// <summary>
        /// 取得したAzureのKey(★公開してはいけない)
        /// </summary>
        [SerializeField]
        private string subscriptionKey = "★ここにAzureのキー情報を入力する★";

        /// <summary>
        /// 認識モード : interactive or conversation or dictation
        /// </summary>
        [SerializeField]
        private string recognitionMode = "interactive";

        /// <summary>
        /// 言語 : ja-JP, en-USなど
        /// </summary>
        [SerializeField]
        private string lang = "ja-JP";

        /// <summary>
        /// 出力フォーマット : simple or detailed
        /// </summary>
        [SerializeField]
        private string format = "simple";
        public string Format { get { return format; } }

        /// <summary>
        /// リクエスト送信先のエンドポイント
        /// </summary>
        private string endpoint = @"https://speech.platform.bing.com/speech/recognition/";


        /// <summary>
        /// リクエスト中かどうか
        /// </summary>
        private bool isRequest = false;

        /// <summary>
        /// リクエスト処理
        /// </summary>
        /// <param name="bytes">音声データ</param>
        /// <returns></returns>
        public IEnumerator SendRequest(byte[] bytes)
        {
            // リクエスト中の新たなリクエストの防止
            if(isRequest)
            {
                yield break;
            }
            isRequest = true;

            string requestUrl = endpoint
                + recognitionMode
                + @"/cognitiveservices/v1?language=" + lang
                + @"&format=" + format;
            Debug.Log(requestUrl);

            // formにバイナリデータを追加する
            WWWForm form = new WWWForm();
            form.AddBinaryData("file", bytes, "voice.wav", @"application/octet-stream");

            // HTTPリクエストを送る
            UnityWebRequest request = UnityWebRequest.Post(requestUrl, form);
            request.chunkedTransfer = true;
            request.SetRequestHeader("Ocp-Apim-Subscription-Key", subscriptionKey);
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                Debug.Log(request.error);
                if(ErrorCallback != null)
                {
                    ErrorCallback(request.error);
                }
                else
                {
                    Debug.LogWarning("SpeechToTextApiErrorCallback is null.");
                }
            }
            else
            {
                Debug.Log("Response Code : " + request.responseCode);
                foreach (var h in request.GetResponseHeaders())
                {
                    // レスポンスヘッダー表示
                    Debug.Log(h);
                }

                if (request.responseCode == 200)
                {
                    // UTF8文字列として取得する
                    string text= request.downloadHandler.text;
                    Debug.Log(text);
                    if(Callback != null)
                    {
                        Callback(text);
                    }
                    else
                    {
                        Debug.LogWarning("SpeechToTextApiCallback is null.");
                    }
                }
            }
            isRequest = false;
        }

    } // class SpeechToTextApi
} // namespace HoloAzureSample.SpeechToText