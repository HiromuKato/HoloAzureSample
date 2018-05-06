using Newtonsoft.Json;
using UnityEngine;

namespace HoloAzureSample.ImageSearch
{
    /// <summary>
    /// ImageSearchのコントローラークラス
    /// </summary>
    public class ImageSearchController : MonoBehaviour
    {
        /// <summary>
        /// ImageSearchビュー
        /// </summary>
        [SerializeField]
        private ImageSearchView view;

        /// <summary>
        /// ImageSearchモデル
        /// </summary>
        [SerializeField]
        private ImageSearchModel model;

        /// <summary>
        /// ImageSearch API
        /// </summary>
        [SerializeField]
        private ImageSearchApi api;

        /// <summary>
        /// Webの画像をTextureとして取得するユーティリティ
        /// </summary>
        [SerializeField]
        private Utility.WebTexture webTexture;

        /// <summary>
        /// 検索キーワード
        /// </summary>
        [SerializeField]
        private string searchKeyword = "猫";

        /// <summary>
        /// 画像をダウンロードした数
        /// </summary>
        private int downloadCount = 0;

        /// <summary>
        /// 初期化処理
        /// </summary>
        void Start()
        {
            // 画像検索APIで指定したcount分だけSpriteRendererを生成する
            view.CreateSpriteRenderer(api.ImageCount);

            // コールバックへの登録
            api.Callback = OnReceiveResponse;
            api.ErrorCallback = OnReceiveErrorResponse;

            webTexture.Callback = OnReceiveWebTextureResponse;
            webTexture.ErrorCallback = OnReceiveWebTextureErrorResponse;

            // 意図分類・キーワード抽出のリクエスト送信
            StartCoroutine(api.SendRequest(searchKeyword));
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        private void OnDestroy()
        {
            api.Callback = null;
            api.ErrorCallback = null;
            webTexture.Callback = null;
            webTexture.ErrorCallback = null;
        }

        /// <summary>
        /// ImageSearchApiのレスポンスを受信したときの処理
        /// </summary>
        /// <param name="response">レスポンス</param>
        private void OnReceiveResponse(string response)
        {
            model = JsonConvert.DeserializeObject<ImageSearchModel>(response);

            downloadCount = 0;
            for (int i = 0; i < model.value.Count; ++i)
            {
                // URLから画像を取得する
                StartCoroutine(webTexture.SendRequest(model.value[i].thumbnailUrl));
            }
        }

        /// <summary>
        /// ImageSearchApiのエラーレスポンスを受信したときの処理
        /// </summary>
        /// <param name="response">レスポンス</param>
        private void OnReceiveErrorResponse(string response)
        {
            Debug.Log(response);
        }

        /// <summary>
        /// WebTextureのレスポンスを受信したときの処理
        /// </summary>
        /// <param name="response">レスポンス</param>
        private void OnReceiveWebTextureResponse(Texture texture)
        {
            downloadCount++;
            view.ShowImages(downloadCount, model.value.Count, texture);
        }

        /// <summary>
        /// WebTextureのエラーレスポンスを受信したときの処理
        /// </summary>
        /// <param name="response">レスポンス</param>
        private void OnReceiveWebTextureErrorResponse(string response)
        {
            Debug.Log(response);
        }

    } // class ImageSearchController
} // namespace HoloAzureSample.ImageSearch
