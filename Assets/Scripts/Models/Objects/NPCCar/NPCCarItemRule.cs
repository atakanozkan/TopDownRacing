using System;
using System.Collections;
using System.Collections.Generic;
using TDR.Models.Pool;
using UnityEngine;

namespace TDR.Models.NPCCar
{
    [Serializable]
    public class NPCCarItemRule
    {
        public PoolItemType Key;
        public int Value;
        public int MaxValueRule;
        public List<Material> Materials;
    }
}
