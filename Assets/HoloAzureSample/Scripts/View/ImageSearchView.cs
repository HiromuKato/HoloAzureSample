using UnityEngine;

namespace HoloAzureSample.ImageSearch
{
    /// <summary>
    /// ImageSearchのビュークラス
    /// </summary>
    public class ImageSearchView : MonoBehaviour
    {
        /// <summary>
        /// 画像表示するためのプレハブ
        /// </summary>
        [SerializeField]
        private SpriteRenderer imageUnit;

        /// <summary>
        /// 画像の親オブジェクト（Hierarchyを整理するために利用）
        /// </summary>
        [SerializeField]
        private Transform parent;

        /// <summary>
        /// 画像検索した結果を表示するためのSpriteRenderer
        /// </summary>
        private SpriteRenderer[] sr;

        /// <summary>
        /// 画像を円形表示するさいの半径
        /// </summary>
        private float radius = 10;

        /// <summary>
        /// サムネイル画像の横幅
        /// </summary>
        private int imageWitdh = 160;

        /// <summary>
        /// /// サムネイル画像の高さ
        /// </summary>
        private int imageHeight = 120;

        /// <summary>
        /// SpriteRendererを生成する
        /// </summary>
        /// <param name="imageCount">生成する数</param>
        public void CreateSpriteRenderer(int imageCount)
        {
            sr = new SpriteRenderer[imageCount];
        }

        /// <summary>
        /// ダウンロードが終了した
        /// </summary>
        /// <param name="downloadCount">画像のダウンロードが終了した数</param>
        /// <param name="maxCount">画像検索APIで指定した検索数</param>
        /// <param name="texture">ダウンロードしたテクスチャ</param>
        public void ShowImages(int downloadCount, int maxCount, Texture texture)
        {
            // テクスチャのリサイズ
            float w = texture.width;
            float h = texture.height;
            float ratio = w / h;
            if (ratio >= (float)imageWitdh / (float)imageHeight)
            {
                TextureScale.Scale((Texture2D)texture, imageWitdh, (int)(imageWitdh / ratio));
            }
            else
            {
                TextureScale.Scale((Texture2D)texture, (int)(imageHeight * ratio), imageHeight);
            }

            //各オブジェクトを円状に配置する
            Vector3 imagePosition = parent.transform.position;
            float angleDiff = 360f / maxCount;
            float angle = (90 - angleDiff * downloadCount) * Mathf.Deg2Rad;
            imagePosition.x += radius * Mathf.Cos(angle);
            imagePosition.y = parent.transform.position.y;
            imagePosition.z += radius * Mathf.Sin(angle);

            int index = downloadCount - 1;
            // 以前のインスタンスを廃棄
            if (sr[index] != null)
            {
                Destroy(sr[index].gameObject);
            }
            sr[index] = (SpriteRenderer)Instantiate(imageUnit, imagePosition, Quaternion.identity, parent);
            sr[index].sprite = Sprite.Create((Texture2D)texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            sr[index].gameObject.name = "ImageUnit" + index.ToString();
        }

    } // class ImageSearchView
} // namespace HoloAzureSample.ImageSearch