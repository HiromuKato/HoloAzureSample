using UnityEngine;

namespace HoloAzureSample.SpeechToText
{
    /// <summary>
    ///SpeechToTextのViewクラス
    /// </summary>
    public class SpeechToTextView : MonoBehaviour
    {
        /// <summary>
        /// デバッグ用テキスト
        /// </summary>
        public TextMesh debug;

        /// <summary>
        /// AudioSource
        /// </summary>
        public AudioSource audioSrc;

        /// <summary>
        /// TextMeshを持ったプレハブ
        /// </summary>
        [SerializeField]
        private GameObject messagePrefab;

        /// <summary>
        /// カメラのキャッシュ
        /// </summary>
        private Transform cameraCache;

        /// <summary>
        /// 初期化処理
        /// </summary>
        void Start()
        {
            cameraCache = Camera.main.transform;
        }

        /// <summary>
        /// デバッグメッセージを設定する
        /// </summary>
        /// <param name="message"></param>
        public void SetDebugMessage(string message)
        {
            debug.text = message;
        }

        /// <summary>
        /// タップのホールドを開始したときの表示処理
        /// </summary>
        public void OnHoldStarted()
        {
            debug.color = new Color(1, 0, 0, 1);
            SetDebugMessage("Recording");
        }

        /// <summary>
        /// タップのホールドを終了したときの表示処理
        /// </summary>
        public void OnHoldCompleted()
        {
            debug.color = new Color(1, 1, 1, 1);
            SetDebugMessage("タップ&ホールドで録音開始");
        }

        /// <summary>
        /// タップのホールドをキャンセルしたときの表示処理
        /// </summary>
        public void OnHoldCanceled()
        {
            debug.color = new Color(1, 1, 1, 1);
            SetDebugMessage("タップ&ホールドで録音開始");
        }

        /// <summary>
        /// レスポンスを表示する
        /// </summary>
        /// <param name="response">レスポンス</param>
        public void ShowResponse(string response)
        {
            // カメラから距離10の位置にテキストを生成
            Ray ray = new Ray(cameraCache.position, cameraCache.rotation * Vector3.forward);
            GameObject message = (GameObject)Instantiate(messagePrefab, new Vector3(0, 0, 0), Quaternion.identity);
            message.transform.position = ray.GetPoint(10);
            message.transform.LookAt(cameraCache);
            message.transform.Rotate(new Vector3(0, 180, 0));
            if (response == null || response.Length == 0)
            {
                debug.color = new Color(1, 0, 0, 1);
                SetDebugMessage("音声を認識できませんでした");
            }
            else
            {
                message.GetComponent<TextMesh>().text = response;
            }
        }

        /// <summary>
        /// エラーレスポンスを表示する
        /// </summary>
        /// <param name="response"></param>
        public void ShowErrorResponse(string response)
        {
            debug.color = new Color(1, 0, 0, 1);
            SetDebugMessage(response);
        }

    } // class SpeechToTextView
} // namespace HoloAzureSample.SpeechToText