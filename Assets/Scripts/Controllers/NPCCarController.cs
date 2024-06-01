using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using TDR.Helpers;
using TDR.Managers;
using TDR.Models.NPCCar;
using TDR.Models.Player;
using Unity.VisualScripting;
using UnityEngine;

namespace TDR.Controllers
{
    public class NPCCarController : MonoBehaviour
    {
        [SerializeField]
        private Transform playerTransform;
        [SerializeField]
        private float despawnDistance = -5f;

        public List<NPCCar> listNpcCars = new List<NPCCar>();

        public Action<NPCCar> onCarDisappers;

        public bool canNPCCarsMove = false;

        private void Start()
        {
            canNPCCarsMove = GameManager.Instance.currentGameState == GameState.Playing;
        }

        private void Update()
        {
            if (canNPCCarsMove)
            {
                MoveCars();
                CleanupCars();
            }
        }

        private void MoveCars()
        {
            foreach (NPCCar car in listNpcCars)
            {
                Vector3 currentPosition = car.transform.position;
                float movement = car.carForwardSpeed * Time.deltaTime;
                currentPosition.z += car.isMovingBackward ? -movement : movement;
                car.transform.position = currentPosition;
            }
        }

        private void CleanupCars()
        {
            List<NPCCar> carsToRemove = new List<NPCCar>();
            foreach (NPCCar car in listNpcCars)
            {
                float distanceToPlayer = car.transform.position.z - playerTransform.position.z;
                if (distanceToPlayer < despawnDistance)
                {
                    carsToRemove.Add(car);
                }
            }

            foreach (NPCCar car in carsToRemove)
            {
                listNpcCars.Remove(car);
                onCarDisappers?.Invoke(car);
            }
        }

        public void ResetAllCars()
        {
            List<NPCCar> carsToRemove = new List<NPCCar>();
            foreach (NPCCar car in listNpcCars)
            {
                carsToRemove.Add(car);
            }

            foreach (NPCCar car in carsToRemove)
            {
                listNpcCars.Remove(car);
                onCarDisappers?.Invoke(car);
            }
        }

        private void ControlPlayerLights(DayTimeType dayTimeType)
        {
            if (dayTimeType == DayTimeType.Night)
            {
                foreach (NPCCar nPCCar in listNpcCars)
                {
                    nPCCar.TurnOnTheLigths();
                }
            }
            else
            {
                foreach (NPCCar nPCCar in listNpcCars)
                {
                    nPCCar.TurnOffTheLigths();
                }
            }
        }

        public void AddNewCarToList(NPCCar car)
        {
            listNpcCars.Add(car);
        }

        public List<NPCCar> GetCurrentCars()
        {
            return listNpcCars;
        }


        private void CheckIfNPCCarsMove(GameState state)
        {
            if (state == GameState.Playing)
            {
                canNPCCarsMove = true;
            }
            else
            {
                canNPCCarsMove = false;
            }
            ControlFreezeAllNpcs(!canNPCCarsMove);
        }

        public void ControlFreezeAllNpcs(bool doFreeze)
        {
            foreach (NPCCar car in listNpcCars)
            {
                if (doFreeze)
                {
                    car.FreezeMovement();
                }
                else
                {
                    car.UnFreezeMovement();
                }

            }
        }
        private void OnEnable()
        {
            GameManager.Instance.GetDayTimeManager().onDayTimeChanged += ControlPlayerLights;
            GameManager.Instance.OnGameStateChanged += CheckIfNPCCarsMove;
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.GetDayTimeManager().onDayTimeChanged -= ControlPlayerLights;
                GameManager.Instance.OnGameStateChanged -= CheckIfNPCCarsMove;
            }
        }
    }

}