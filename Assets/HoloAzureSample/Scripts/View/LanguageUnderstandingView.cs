using UnityEngine;

namespace HoloAzureSample.LanguageUnderstanding
{
    /// <summary>
    /// LanguageUnderstandingのビュークラス
    /// </summary>
    public class LanguageUnderstandingView : MonoBehaviour
    {
        /// <summary>
        /// LUISに入力する文章
        /// </summary>
        [SerializeField]
        private TextMesh searchSentence;

        /// <summary>
        /// LUISにより検出されたキーワード
        /// </summary>
        [SerializeField]
        private TextMesh keyword;

        /// <summary>
        /// 入力文章に文字列を設定する
        /// </summary>
        /// <param name="value"></param>
        public void SetSearchSentence(string value)
        {
            searchSentence.text = value;
        }

        /// <summary>
        /// キーワードに文字列を設定する
        /// </summary>
        public void SetKeyword(string value)
        {
            keyword.text = value;
        }

    } // class LanguageUnderstandingView
} // namespace HoloAzureSample.LanguageUnderstanding