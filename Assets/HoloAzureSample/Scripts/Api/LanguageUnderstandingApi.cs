using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace HoloAzureSample.LanguageUnderstanding
{
    /// <summary>
    /// LUIS APIを利用し自然言語から意図分類、キーワード抽出するクラス
    /// 参考：https://docs.microsoft.com/en-us/azure/cognitive-services/luis/
    /// </summary>
    public class LanguageUnderstandingApi : MonoBehaviour
    {
        /// <summary>
        /// コールバック定義
        /// </summary>
        /// <param name="response">レスポンス</param>
        public delegate void LUISApiCallback(string response);
        /// <summary>
        /// レスポンスを取得したときに呼ぶコールバック
        /// </summary>
        public LUISApiCallback Callback { get; set; }

        /// <summary>
        /// エラー時のコールバック定義
        /// </summary>
        /// <param name="response">レスポンス</param>
        public delegate void LUISApiErrorCallback(string response);
        /// <summary>
        /// エラーレスポンスを取得したときに呼ぶコールバック
        /// </summary>
        public LUISApiErrorCallback ErrorCallback { get; set; }

        /// <summary>
        /// 言語
        /// </summary>
        [SerializeField]
        private string lang = "ja-JP";

        /// <summary>
        /// LUISのエンドポイント（日本語）
        /// ★subscription-keyは公開してはいけない、LanguageUnderstanding.unityにも保持されているので注意すること
        /// </summary>
        [SerializeField]
        private string endpoint_ja = "★ここにエンドポイントの情報を入力する★";

        /// <summary>
        /// LUISのエンドポイント（英語）
        /// ★subscription-keyは公開してはいけない、LanguageUnderstanding.unityにも保持されているので注意すること
        /// </summary>
        [SerializeField]
        private string endpoint_en = "★ここにエンドポイントの情報を入力する★";

        /// <summary>
        /// リクエスト中かどうか
        /// </summary>
        private bool isRequest = false;

        /// <summary>
        /// リクエスト処理
        /// </summary>
        /// <returns></returns>
        public IEnumerator SendRequest(string requestText)
        {
            // リクエスト中の新たなリクエストの防止
            if (isRequest || requestText == null)
            {
                yield break;
            }
            isRequest = true;
            Debug.Log("Send LUIS Request.");

            string url = endpoint_ja;
            // 言語選択
            if (lang.Equals("ja-JP"))
            {
                url = endpoint_ja;
            }
            else if (lang.Equals("en-US"))
            {
                url = endpoint_en;
            }
            else
            {
                Debug.LogAssertion("不明な言語");
            }

            // HTTPリクエストを送る
            UnityWebRequest request = UnityWebRequest.Get(url + requestText);
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                Debug.Log(request.error);
                if (ErrorCallback != null)
                {
                    ErrorCallback(request.error);
                }
                else
                {
                    Debug.LogWarning("LUISApiErrorCallback is null.");
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
                    string text = request.downloadHandler.text;
                    Debug.Log(text);
                    if (Callback != null)
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

    } // class LanguageUnderstandingApi
} // namespace HoloAzureSample.LanguageUnderstanding