using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TDR.Models
{
    public abstract class Car : MonoBehaviour
    {
        [SerializeField]
        private int currentLaneIndex;
        [SerializeField]
        private List<GameObject> listOfLights;
        [SerializeField]
        private Rigidbody _rigidbody;
        private RigidbodyConstraints defaultConstraints;

        public float carForwardSpeed;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            defaultConstraints = _rigidbody.constraints;
        }

        public void SetLaneIndex(int index)
        {
            currentLaneIndex = index;
        }

        public int GetLaneIndex()
        {
            return currentLaneIndex;
        }

        public void TurnOnTheLigths()
        {
            foreach (GameObject lightObject in listOfLights)
            {
                lightObject.gameObject.SetActive(true);
            }
        }

        public void TurnOffTheLigths()
        {
            foreach (GameObject lightObject in listOfLights)
            {
                lightObject.gameObject.SetActive(false);
            }
        }

        public void FreezeMovement()
        {
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        public void UnFreezeMovement()
        {
            _rigidbody.constraints = defaultConstraints;
        }

        public Rigidbody GetRigidbody()
        {
            return _rigidbody;
        }
    }

}
