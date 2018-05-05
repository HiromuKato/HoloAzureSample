using Newtonsoft.Json;
using UnityEngine;

namespace HoloAzureSample.LanguageUnderstanding
{
    /// <summary>
    /// LanguageUnderstandingのコントローラークラス
    /// </summary>
    public class LanguageUnderstandingController : MonoBehaviour
    {
        /// <summary>
        /// LanguageUnderstandingビュー
        /// </summary>
        [SerializeField]
        private LanguageUnderstandingView view;

        /// <summary>
        /// LanguageUnderstandingモデル
        /// </summary>
        [SerializeField]
        private LUISResponse model;

        /// <summary>
        /// LanguageUnderstanding API
        /// </summary>
        [SerializeField]
        private LanguageUnderstandingApi api;

        [SerializeField]
        private string searchSentence = "松屋の画像を検索";

        /// <summary>
        /// 初期化処理
        /// </summary>
        void Start()
        {
            // コールバックへの登録
            api.Callback = OnReceiveResponse;
            api.ErrorCallback = OnReceiveErrorResponse;

            view.SetSearchSentence(searchSentence);

            // 意図分類・キーワード抽出のリクエスト送信
            StartCoroutine(api.SendRequest(searchSentence));
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
        /// <param name="response">レスポンス</param>
        private void OnReceiveResponse(string response)
        {
            model = JsonConvert.DeserializeObject<LUISResponse>(response);
            if (model != null && model.entities != null && model.entities.Count > 0)
            {
                Debug.Log(model.entities[0].entity);
                view.SetKeyword(model.entities[0].entity);
            }
            else
            {
                view.SetKeyword("キーワードを検出できませんでした");
            }
        }

        /// <summary>
        /// エラーレスポンスを受信したときの処理
        /// </summary>
        /// <param name="response">レスポンス</param>
        private void OnReceiveErrorResponse(string response)
        {
            Debug.Log(response);
        }

    } // class LanguageUnderstandingController
} // namespace HoloAzureSample.LanguageUnderstanding