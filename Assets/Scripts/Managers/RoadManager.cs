using System;
using System.Collections;
using System.Collections.Generic;
using TDR.Builder;
using TDR.Controllers;
using TDR.Helpers;
using TDR.Models.Game;
using TDR.Models.Road;
using UnityEngine;

namespace TDR.Managers
{
    public class RoadManager : MonoBehaviour
    {
        [SerializeField]
        private RoadBuilder builder;
        [SerializeField]
        private PlayerController playerController;
        [SerializeField]
        private List<Road> roadList;
        [SerializeField]
        private Vector3[] currentRoadEnterPositions;
        [SerializeField]
        private Vector3[] currentRoadExitPositions;

        public List<float> laneList;
        public int maxCurrentRoadLimit = 3;

        private bool canUpdateRoads = false;

        private void LateUpdate()
        {
            if (canUpdateRoads)
            {
                ControlPassCurrentExit();
            }
        }

        public void BuildInitialRoadItems()
        {
            builder.CreateItems();
        }

        public void GenerateStartRoad()
        {
            roadList = builder.BuildInitialRoad();
            currentRoadEnterPositions = new Vector3[maxCurrentRoadLimit];
            currentRoadExitPositions = new Vector3[maxCurrentRoadLimit];
            UpdateCurrentEnterPositions(roadList);
            UpdateCurrentExitPositions(roadList);
        }


        public void ControlPassCurrentExit()
        {
            Vector3 playerCurrentPos = playerController.GetPlayerCurrentPosition();
            int halfwayIndex = maxCurrentRoadLimit / 2;

            if (playerCurrentPos.z >= currentRoadExitPositions[halfwayIndex].z)
            {
                UpdateRoads();
            }
        }

        public void UpdateCurrentEnterPositions(List<Road> roads)
        {
            if (roads.Count != maxCurrentRoadLimit)
            {
                Debug.LogError("The current size must be equal to road limit.");
                return;
            }
            int index = 0;
            foreach(Road road in roads)
            {
                currentRoadEnterPositions[index] = roads[index].roadEnterPoint.transform.position;
                index++;
            }
        }

        public void UpdateCurrentExitPositions(List<Road> roads)
        {
            if(roads.Count != maxCurrentRoadLimit)
            {
                Debug.LogError("The current size must be equal to road limit.");
                return;
            }
            int index = 0;
            foreach (Road road in roads)
            {
                currentRoadExitPositions[index] = roads[index].roadExitPoint.transform.position;
                index++;
            }
        }

        public void UpdateRoads()
        {
            Road newRoad = builder.BuildNextRoad(roadList[maxCurrentRoadLimit-1]);

            Road firstRoadOnList = roadList[0];
            roadList.RemoveAt(0);

            builder.DestroyRoad(firstRoadOnList);

            roadList.Add(newRoad);
            UpdateCurrentEnterPositions(roadList);
            UpdateCurrentExitPositions(roadList);
        }

        public void ResetAllRoads()
        {
            foreach(Road road in roadList)
            {
                builder.DestroyRoad(road);
            }
        }

        private void CheckCanUpdateRoads(GameState state)
        {
            if(state == GameState.GameOver)
            {
                ResetAllRoads();
                canUpdateRoads = false;
            }
            if(state == GameState.Playing)
            {
                canUpdateRoads = true;
            }
            else
            {
                canUpdateRoads = false;
            }
        }

        public float GetRoadLanePosX(int index)
        {
            return laneList[index];
        }

        private void OnEnable()
        {
            GameManager.Instance.OnGameStateChanged += CheckCanUpdateRoads;
        }

        private void OnDisable()
        {
            if(GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= CheckCanUpdateRoads;
            }
        }
    }
}

