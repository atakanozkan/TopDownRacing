using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDR.Models.Road
{
    public class Road : MonoBehaviour
    {
        public GameObject roadEnterPoint;
        public GameObject roadExitPoint;

        public float GetRoadWidth()
        {
            return roadEnterPoint.transform.position.z - roadExitPoint.transform.position.z;
        }

    }

}
