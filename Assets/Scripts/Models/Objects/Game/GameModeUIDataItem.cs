using TDR.Helpers;
using TMPro;
using System;

namespace TDR.Models.Game
{
    [Serializable]
    public class GameModeUIData
    {
        public GameMode Mode;
        public string Score;
        public string Distance;
        public TextMeshProUGUI TargetScoreTextMesh;
        public TextMeshProUGUI TargetDistanceTextMesh;
    }
}