using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDR.Models.Player
{
    public class Player : Car
    {
        [SerializeField]
        private GameObject playerCar;

        [SerializeField]
        private Material lightMaterial;

        [SerializeField]
        private List<Material> playerColorRange;

        [SerializeField]
        private Renderer playerRenderer;


        public void SetPlayerCar(GameObject obj)
        {
            playerCar = obj;
        }

        public GameObject GetPlayerCar()
        {
            return playerCar;
        }

        public Material GetMaterialBySettingIndex(int index)
        {
            if (index < playerColorRange.Count)
            {
                return playerColorRange[index];
            }
            return null;
        }

        public void SetMaterial(Material material)
        {
            if(material == null)
            {
                return;
            }
             playerRenderer.materials = new Material[] { material, lightMaterial };
        }
    }
}

