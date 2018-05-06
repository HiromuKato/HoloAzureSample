using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace HoloAzureSample.ImageSearch
{
    /// <summary>
    /// Bing Image Search Apiを利用して画像検索を行うクラス
    /// 参考：https://docs.microsoft.com/en-us/azure/cognitive-services/bing-image-search/
    /// </summary>
    public class ImageSearchApi : MonoBehaviour
    {
        /// <summary>
        /// コールバック定義
        /// </summary>
        /// <param name="response">レスポンス</param>
        public delegate void ImageSearchApiCallback(string response);
        /// <summary>
        /// レスポンスを取得したときに呼ぶコールバック
        /// </summary>
        public ImageSearchApiCallback Callback { get; set; }

        /// <summary>
        /// エラー時のコールバック定義
        /// </summary>
        /// <param name="response">レスポンス</param>
        public delegate void ImageSearchApiErrorCallback(string response);
        /// <summary>
        /// エラーレスポンスを取得したときに呼ぶコールバック
        /// </summary>
        public ImageSearchApiErrorCallback ErrorCallback { get; set; }

        /// <summary>
        /// 取得したAzureのKey
        /// ★公開してはいけない、ImageSearch.unityにも保持されているので注意すること
        /// </summary>
        [SerializeField]
        private string subscriptionKey = "★ここにAzureのキー情報を入力する★";

        /// <summary>
        /// 言語
        /// </summary>
        [SerializeField]
        private string lang = "ja-JP";

        /// <summary>
        /// 検索結果数
        /// </summary>
        [SerializeField]
        private int imageCount = 20;
        public int ImageCount { get { return imageCount; } }

        /// <summary>
        /// 検索画像のオフセット
        /// </summary>
        [SerializeField]
        private int offset = 0;

        /// <summary>
        /// エンドポイント
        /// </summary>
        private string endpoint = @"https://api.cognitive.microsoft.com/bing/v7.0/images/search?";

        /// <summary>
        /// リクエスト中かどうか
        /// </summary>
        private bool isRequest = false;

        /// <summary>
        /// リクエスト処理
        /// </summary>
        /// <param name="keyword">検索キーワード</param>
        /// <returns></returns>
        public IEnumerator SendRequest(string keyword)
        {
            // リクエスト中の新たなリクエストの防止
            if (isRequest || keyword == null)
            {
                yield break;
            }
            isRequest = true;
            Debug.Log("Send Image Search Request.");

            // 文字列のエスケープ処理
            string escapedKeyword = WWW.EscapeURL(keyword);

            string param =
                "q=" + escapedKeyword +
                "&count=" + imageCount.ToString() +
                "&offset=" + offset.ToString() +
                "&mkt=" + lang;
            string requestUrl = endpoint + param;
            Debug.Log(requestUrl);

            // HTTPリクエストを送る
            UnityWebRequest request = UnityWebRequest.Get(requestUrl);
            request.chunkedTransfer = true;
            request.SetRequestHeader("Ocp-Apim-Subscription-Key", subscriptionKey);
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
                    Debug.LogWarning("ImageSearchApiErrorCallback is null.");
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
                        Debug.LogWarning("ImageSearchApiCallback is null.");
                    }
                }
            }
            isRequest = false;
        }

    } // class ImageSearchApi
} // namespace HoloAzureSample.ImageSearch
