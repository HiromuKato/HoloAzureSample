using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace HoloAzureSample.Utility
{
    /// <summary>
    /// Webの画像をTextureとして取得するクラス
    /// </summary>
    public class WebTexture : MonoBehaviour
    {
        /// <summary>
        /// コールバック定義
        /// </summary>
        /// <param name="response">レスポンス</param>
        public delegate void WebTextureCallback(Texture tex);
        /// <summary>
        /// レスポンスを取得したときに呼ぶコールバック
        /// </summary>
        public WebTextureCallback Callback { get; set; }

        /// <summary>
        /// エラー時のコールバック定義
        /// </summary>
        /// <param name="response">レスポンス</param>
        public delegate void WebTextureErrorCallback(string response);
        /// <summary>
        /// エラーレスポンスを取得したときに呼ぶコールバック
        /// </summary>
        public WebTextureErrorCallback ErrorCallback { get; set; }

        /// <summary>
        /// リクエスト処理
        /// </summary>
        /// <param name="url">リクエストURL</param>
        /// <returns></returns>
        public IEnumerator SendRequest(string url)
        {
            Debug.Log("WebTexture Request.");

            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
                if (ErrorCallback != null)
                {
                    ErrorCallback(request.error);
                }
                else
                {
                    Debug.LogWarning("WebTextureErrorCallback is null.");
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
                    if (Callback != null)
                    {
                        Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                        Callback(texture);
                    }
                    else
                    {
                        Debug.LogWarning("WebTextureCallback is null.");
                    }
                }
            }
        }

    } // class WebTexture
} // namespace HoloAzureSample.Utility