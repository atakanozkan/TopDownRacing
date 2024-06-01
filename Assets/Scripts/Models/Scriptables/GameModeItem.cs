using System.Collections;
using System.Collections.Generic;
using TDR.Helpers;
using UnityEngine;

namespace TDR.Models
{
    [CreateAssetMenu (fileName = "New Game Mode Item", menuName = "Game Mode")]
    public class GameModeItem : ScriptableObject
    {
        public string name;
        public GameMode gameMode;
    }

}
