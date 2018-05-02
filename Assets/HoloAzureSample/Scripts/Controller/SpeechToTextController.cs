using UnityEngine;
using Newtonsoft.Json;
using HoloToolkit.Unity.InputModule;

namespace HoloAzureSample.SpeechToText
{
    /// <summary>
    /// SpeechToTextのコントローラークラス
    /// </summary>
    public class SpeechToTextController : MonoBehaviour, IHoldHandler
    {
        /// <summary>
        /// SpeechToTextビュー
        /// </summary>
        [SerializeField]
        private SpeechToTextView view;

        /// <summary>
        /// SpeechToTextモデル
        /// </summary>
        [SerializeField]
        private STTSimpleResponse model;
        //private STTDetailResponse model;

        /// <summary>
        /// SpeechToText API
        /// </summary>
        [SerializeField]
        private SpeechToTextApi api;

        /// <summary>
        /// マイクが接続されているかどうかのフラグ
        /// </summary>
        private bool micConnected = false;

        /// <summary>
        /// 録音時の最小周波数
        /// </summary>
        private int minFreq;

        /// <summary>
        /// 録音時の最大周波数
        /// </summary>
        private int maxFreq;

        /// <summary>
        /// 音声データのバイト配列
        /// </summary>
        private byte[] bytes;

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Start()
        {
            // どこをタップしても反応するようにGlobalListenerに登録
            InputManager.Instance.AddGlobalListener(gameObject);

            // コールバックへの登録
            api.Callback = OnReceiveResponse;
            api.ErrorCallback = OnReceiveErrorResponse;

            // マイクが接続されているか確認
            if (Microphone.devices.Length <= 0)
            {
                view.SetDebugMessage("Microphone not connected!");
            }
            else
            {
                // マイクが接続されているフラグを設定
                micConnected = true;

                // デフォルトマイクの周波数の範囲を取得する
                Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);

                //minFreq や maxFreq 引数で 0 の値が返されるとデバイスが任意の周波数をサポートすることを示す
                if (minFreq == 0 && maxFreq == 0)
                {
                    // 録音サンプリングレートを設定する
                    maxFreq = 16000;
                }
            }
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
        /// タップのホールドを開始したときの処理
        /// </summary>
        /// <param name="eventData">ホールドのイベントデータ</param>
        public void OnHoldStarted(HoldEventData eventData)
        {
            if (!micConnected)
            {
                // マイクが接続されていない場合はなにもしない
                return;
            }

            view.OnHoldStarted();

            // 録音中でなければ処理を行う
            if (!Microphone.IsRecording(null))
            {
                //録音開始
                view.audioSrc.clip = Microphone.Start(null, true, 3, maxFreq);
            }
        }

        /// <summary>
        /// タップのホールドを終了したときの処理
        /// </summary>
        /// <param name="eventData">ホールドのイベントデータ</param>
        public void OnHoldCompleted(HoldEventData eventData)
        {
            view.OnHoldCompleted();

            if (Microphone.IsRecording(null))
            {
                //録音中であれば録音停止
                Microphone.End(null);

                // audioclipをbyte配列に変換
                string filepath;
                bytes = WavUtility.FromAudioClip(view.audioSrc.clip, out filepath, false);
                //Debug.Log(filepath);

                // 音声をテキストに変換するリクエスト送信
                StartCoroutine(api.SendRequest(bytes));
            }
        }

        /// <summary>
        /// タップのホールドをキャンセルしたときの処理
        /// </summary>
        /// <param name="eventData">ホールドのイベントデータ</param>
        public void OnHoldCanceled(HoldEventData eventData)
        {
            view.OnHoldCanceled();

            if (Microphone.IsRecording(null))
            {
                //録音中であれば録音停止
                Microphone.End(null);
            }
        }

        /// <summary>
        /// レスポンスを受信したときの処理
        /// </summary>
        /// <param name="response">レスポンス</param>
        public void OnReceiveResponse(string response)
        {
            if (api.Format.Equals("simple"))
            {
                model = JsonConvert.DeserializeObject<STTSimpleResponse>(response);
                view.ShowResponse(model.DisplayText);
            }
            else
            {
                // 詳細モードの場合
                //model = JsonConvert.DeserializeObject<STTDetailedResponse>(response);
                //view.ShowResponse(model.NBest[0].Display);
            }
        }

        /// <summary>
        /// エラーレスポンスを受信したときの処理
        /// </summary>
        /// <param name="response">レスポンス</param>
        public void OnReceiveErrorResponse(string response)
        {
            view.ShowErrorResponse(response);
        }

    } // class SpeechToTextController
} // namespace HoloAzureSample.SpeechToText
