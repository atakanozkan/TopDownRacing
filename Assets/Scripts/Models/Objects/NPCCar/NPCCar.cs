using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDR.Models.NPCCar
{
    public class NPCCar : Car
    {
        [SerializeField]
        private Transform lengthStart;
        [SerializeField]
        private Transform lengthEnd;
        [SerializeField]
        private Transform widthStart;
        [SerializeField]
        private Transform widthEnd;
        [SerializeField]
        private Renderer carRenderer;

        public BoxCollider simplifiedCollider;

        public bool isMovingBackward = false;

        public float GetLength()
        {
            return Vector3.Distance(lengthStart.position, lengthEnd.position);
        }

        public float GetWidth()
        {
            return Vector3.Distance(widthStart.position, widthEnd.position);
        }

        public Renderer GetRenderer()
        {
            return carRenderer;
        }
    }
}