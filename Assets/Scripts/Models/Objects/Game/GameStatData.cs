using System;
using System.Collections;
using System.Collections.Generic;
using TDR.Helpers;
using UnityEngine;

namespace TDR.Models.Game
{
    [Serializable]
    public class GameStatData
    {
        public GameMode Mode;
        public float Score;
        public float Distance;
    }
}