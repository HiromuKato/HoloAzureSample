using System.Collections;
using System.Text;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace HoloAzureSample.TextToSpeech
{
    /// <summary>
    /// Bing Speech APIを利用してテキスト→音声の変換処理を行うクラス
    /// 参考：https://docs.microsoft.com/ja-jp/azure/cognitive-services/speech/home
    /// </summary>
    public class TextToSpeechApi : MonoBehaviour
    {
        /// <summary>
        /// コールバック定義
        /// </summary>
        /// <param name="response">レスポンス</param>
        public delegate void TextToSpeechApiCallback(AudioClip response);
        /// <summary>
        /// レスポンスを取得したときに呼ぶコールバック
        /// </summary>
        public TextToSpeechApiCallback Callback { get; set; }

        /// <summary>
        /// エラー時のコールバック定義
        /// </summary>
        /// <param name="response">レスポンス</param>
        public delegate void TextToSpeechApiErrorCallback(string response);
        /// <summary>
        /// エラーレスポンスを取得したときに呼ぶコールバック
        /// </summary>
        public TextToSpeechApiErrorCallback ErrorCallback { get; set; }

        /// <summary>
        /// 取得したAzureのKey
        /// ★公開してはいけない、TextToSpeech.unityにも保持されているので注意すること
        /// </summary>
        [SerializeField]
        private string subscriptionKey = "★ここにAzureのキー情報を入力する★";

        /// <summary>
        /// 言語
        /// </summary>
        [SerializeField]
        private string lang = "ja-JP";

        /// <summary>
        /// 音声に変換する文章
        /// </summary>
        [SerializeField]
        private string sentence = "こんにちは";

        /// <summary>
        /// アクセストークン取得用のURL（トークンは10分間有効）
        /// </summary>
        private string tokenUrl = @"https://api.cognitive.microsoft.com/sts/v1.0/issueToken";

        /// <summary>
        /// テキスト→音声変換APIのエンドポイント
        /// </summary>
        private string endpoint = @"https://speech.platform.bing.com/synthesize";

        /// <summary>
        /// アクセストークン取得のリクエスト中かどうか
        /// </summary>
        private bool isTokenRequest = false;

        /// <summary>
        /// 音声→テキスト変換のリクエスト中かどうか
        /// </summary>
        private bool isRequest = false;

        /// <summary>
        /// リトライ最大回数
        /// </summary>
        private const int kMaxRetryCount = 5;

        /// <summary>
        /// 現在のリトライ回数
        /// </summary>
        private  int currentRetryCount = 0;

        /// <summary>
        /// アクセストークン取得のリクエストを送信する
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetTokenRequest()
        {
            Debug.Log("Get Token Request");

            // リクエスト中の新たなリクエストの防止
            if (isTokenRequest)
            {
                yield break;
            }
            isTokenRequest = true;

            UnityWebRequest request = UnityWebRequest.Post(tokenUrl, "");
            request.SetRequestHeader("Ocp-Apim-Subscription-Key", subscriptionKey);
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            else
            {
                Debug.Log("Response Code : " + request.responseCode);

                // レスポンスヘッダー表示
                foreach (var h in request.GetResponseHeaders())
                {
                    //Debug.Log(h);
                }

                if (request.responseCode == 200)
                {
                    string token = request.downloadHandler.text;
                    Debug.Log("token : " + token);
                    StartCoroutine(TextToSpeechRequest(token));
                }
                else
                {
                    // こちらのリトライ処理は省略
                }
            }
            isTokenRequest = false;
        }

        /// <summary>
        /// 音声→テキスト変換のリクエストを送信する
        /// </summary>
        /// <param name="token">アクセストークン</param>
        /// <returns></returns>
        public IEnumerator TextToSpeechRequest(string token)
        {
            Debug.Log("TextToSpeech Request");

            // リクエスト中の新たなリクエストの防止
            if (isRequest)
            {
                yield break;
            }
            isRequest = true;

            string ssml = "";
            // 日本語と英語をサポート(対応したい言語があればここを修正する)
            if(lang.Equals("ja-JP"))
            {
                ssml = GenerateSsml(lang, "Female", "Microsoft Server Speech Text to Speech Voice (ja-JP, Ayumi, Apollo)", sentence);
            }
            else if(lang.Equals("en-US"))
            {
                ssml = GenerateSsml(lang, "Female", "Microsoft Server Speech Text to Speech Voice (en-US, ZiraRUS)", sentence);
            }
            else
            {
                Debug.LogError(lang + " language is not supported.");
            }
            byte[] postData = Encoding.UTF8.GetBytes(ssml);

            UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(endpoint, AudioType.WAV);
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/ssml+xml");
            request.SetRequestHeader("X-Microsoft-OutputFormat", "riff-16khz-16bit-mono-pcm");
            request.SetRequestHeader("Authorization", "Bearer " + token);
            request.uploadHandler = new UploadHandlerRaw(postData);
            yield return request.SendWebRequest();

            if(request.isNetworkError)
            {
                Debug.Log(request.error);
                if (ErrorCallback != null)
                {
                    ErrorCallback(request.error);
                }
                else
                {
                    Debug.LogWarning("TextToSpeechApiErrorCallback is null.");
                }
            }
            else
            {
                Debug.Log("Response Code : " + request.responseCode);

                // レスポンスヘッダー表示
                foreach (var h in request.GetResponseHeaders())
                {
                    Debug.Log(h);
                }

                if (request.responseCode == 200)
                {
                    if(Callback != null)
                    {
                        Callback(DownloadHandlerAudioClip.GetContent(request));
                    }
                    else
                    {
                        Debug.LogWarning("TextToSpeechApiCallback is null.");
                    }
                }
                else
                {
                    // 指定された回数だけリトライ処理を行う
                    currentRetryCount++;

                    if (currentRetryCount > kMaxRetryCount)
                    {
                        // 既にリトライ済みであればコルーチンを終了する
                        Debug.Log("リトライ最大回数を超えました");
                        yield break;
                    }

                    // リトライ処理
                    Debug.LogFormat("Retry {0}回目", currentRetryCount);
                    isRequest = false;
                    StartCoroutine(GetTokenRequest());
                    yield break;
                }
            }
            currentRetryCount = 0;
            isRequest = false;
        }

        /// <summary>
        /// SSMLを生成する
        /// </summary>
        /// <param name="locale">言語</param>
        /// <param name="gender">性別</param>
        /// <param name="name">名前</param>
        /// <param name="text">文章</param>
        /// <returns></returns>
        private string GenerateSsml(string locale, string gender, string name, string text)
        {
            var ssmlDoc = new XDocument(
                new XElement("speak",
                    new XAttribute("version", "1.0"),
                    new XAttribute(XNamespace.Xml + "lang", locale),
                    new XElement("voice",
                        new XAttribute(XNamespace.Xml + "lang", locale),
                        new XAttribute(XNamespace.Xml + "gender", gender),
                        new XAttribute("name", name),
                        text
                    )
                )
            );
            return ssmlDoc.ToString();
        }

    } // class TextToSpeechApi
} // namespace HloAzureSample.TextToSpeech