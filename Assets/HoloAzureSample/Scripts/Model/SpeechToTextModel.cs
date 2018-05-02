using System.Collections.Generic;

namespace HoloAzureSample.SpeechToText
{
    /// <summary>
    /// SpeechToTextのモデルクラス（simple）
    /// </summary>
    public class STTSimpleResponse
    {
        public string RecognitionStatus { get; set; }
        public string DisplayText { get; set; }
        public int Offset { get; set; }
        public int Duration { get; set; }
    }

    /// <summary>
    /// SpeechToTextのモデルクラス（detail）
    /// </summary>
    public class STTDetailedResponse
    {
        public string RecognitionStatus { get; set; }
        public int Offset { get; set; }
        public int Duration { get; set; }
        public List<NBest> NBest { get; set; }
    }

    public class NBest
    {
        public double Confidence { get; set; }
        public string Lexical { get; set; }
        public string ITN { get; set; }
        public string MaskedITN { get; set; }
        public string Display { get; set; }
    }

} // namespace HoloAzureSample.SpeechToText