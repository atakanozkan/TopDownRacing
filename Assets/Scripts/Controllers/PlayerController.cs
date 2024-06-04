using System;
using System.Collections;
using System.Collections.Generic;
using TDR.Managers;
using TDR.Helpers;
using UnityEngine;
using Unity.VisualScripting;
using TDR.Models.Player;

namespace TDR.Controllers
{

    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Player player;
        [SerializeField] private float laneChangeDuration = 0.5f;
        [SerializeField] private WheelController wheelController;
        [SerializeField] private float acceleration = 5f;
        [SerializeField] private float brakingPower = 10f;
        [SerializeField] private float maxSpeed = 20f;
        [SerializeField] private float normalSpeed = 10f;

        private float targetSpeed;
        private float distanceTravel;
        private float totalLaneNumberForPlayerMove;
        private Material materialChosenForPlayer;

        public Action<MoveState> onPlayerMovementChange;
        public Action<bool> onPlayerGoOppositeLane;
        public bool isChangingLane = false;
        public bool canMove = false;
        public int playerStartLaneIndex = 2;

        public void BuildPlayer()
        {
            player.gameObject.SetActive(true);
            ResetPlayerAttributes();
            canMove = GameManager.Instance.currentGameState == GameState.Playing;
            totalLaneNumberForPlayerMove = GameManager.Instance.GetRoadManager().laneList.Count;
        }

        private void Update()
        {
            if(canMove){
                MoveForward();
                UpdateSpeed();
            }
        }

        void UpdateSpeed()
        {
            if (Mathf.Abs(targetSpeed - player.carForwardSpeed) > Mathf.Epsilon)
            {
                player.carForwardSpeed = Mathf.MoveTowards(player.carForwardSpeed, targetSpeed,
                    canMove == true ? Time.deltaTime * (targetSpeed > player.carForwardSpeed ? acceleration : brakingPower) : 0);
            }
        }

        public void Accelerate()
        {
            targetSpeed = maxSpeed;
        }

        public void Brake()
        {
            targetSpeed = 0f;
        }

        public void ReleaseGas()
        {
            targetSpeed = normalSpeed;
        }


        private void MoveForward()
        {
            Vector3 currentPosition = player.transform.position;
            float distance = player.carForwardSpeed * Time.deltaTime;
            currentPosition.z += distance;
            player.transform.position = currentPosition;
            distanceTravel += distance;

        }

        public void TriggerPlayerMovement(MoveState state)
        {
            onPlayerMovementChange?.Invoke(state);
        }

        public void SwitchLane(MoveState state)
        {
            if (state == MoveState.StraightMove)
            {
                return;
            }

            int currentLane = player.GetLaneIndex();
            if (state == MoveState.LeftMove)
            {
                MovePlayer(currentLane - 1, -15f);
            }
            else if (state == MoveState.RightMove)
            {
                MovePlayer(currentLane + 1, 15f);
            }
        }

        public void MovePlayer(int laneIndex, float steerAngle)
        {
            if(player.carForwardSpeed == 0) { return; }
            if (laneIndex >= totalLaneNumberForPlayerMove || laneIndex < 0)
            {
                return;
            }
            float destinationX = GameManager.Instance.GetRoadManager().laneList[laneIndex];
            StartCoroutine(SmoothLaneChange(destinationX,laneIndex,steerAngle));
        }

        private IEnumerator SmoothLaneChange(float destinationX, int laneIndex, float steerAngle)
        {
            isChangingLane = true;
            float elapsedTime = 0f;
            Vector3 startingPos = player.transform.position;
            Vector3 targetPos = new Vector3(destinationX, startingPos.y, startingPos.z);

            if (wheelController != null)
            {
                wheelController.SteerWheels(steerAngle);
            }


            while (elapsedTime < laneChangeDuration)
            {
                float t = elapsedTime / laneChangeDuration;
                Vector3 newPosition = Vector3.Lerp(startingPos, targetPos, t);

                newPosition.z += player.carForwardSpeed * elapsedTime;

                player.transform.position = newPosition;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (wheelController != null)
            {
                wheelController.SteerWheels(0f);
            }


            targetPos.z += player.carForwardSpeed * laneChangeDuration;
            player.transform.position = targetPos;
            player.SetLaneIndex(laneIndex);
            isChangingLane = false;
            CheckPlayerIfOppositeLine();
        }


        public Vector3 GetPlayerCurrentPosition()
        {
            return player.transform.position;
        }


        private void ControlPlayerLights(DayTimeType dayTimeType)
        {
            if(dayTimeType == DayTimeType.Night)
            {
                player.TurnOnTheLigths();
            }
            else
            {
                player.TurnOffTheLigths();
            }
        }

        private void CheckIfPlayerMove(GameState state)
        {
            if(state == GameState.GameOver)
            {
                canMove = false;
                player.gameObject.SetActive(false);
            }
            if(player != null)
            {
                if (state == GameState.Playing)
                {
                    canMove = true;
                    player.UnFreezeMovement();
                }
                else
                {
                    canMove = false;
                    player.FreezeMovement();
                }
            }
        }

        public void ApplySettingsToPlayer(Models.Game.GameSettingsData data)
        {
            normalSpeed = data.PlayerNormalSpeed / 10.0f;
            maxSpeed = data.PlayerMaxSpeed / 10.0f;
            acceleration = data.AccelerationOffset;
            brakingPower = data.BrakePowerOffset;
            materialChosenForPlayer = player.GetMaterialBySettingIndex(data.PlayerColorIndex);
        }

        public void ResetPlayerAttributes()
        {
            player.carForwardSpeed = normalSpeed;
            targetSpeed = normalSpeed;
            player.SetLaneIndex(playerStartLaneIndex);
            player.transform.position =
                new Vector3(
                    GameManager.Instance.GetRoadManager().laneList[playerStartLaneIndex],
                    player.transform.position.y,
                    0
                    );
            player.SetMaterial(materialChosenForPlayer);
            onPlayerGoOppositeLane?.Invoke(false);
            isChangingLane = false;
            distanceTravel = 0;
        }

        public void CheckPlayerIfOppositeLine()
        {
            bool check = player.GetLaneIndex() < totalLaneNumberForPlayerMove / 2;

            if (check)
            {
                onPlayerGoOppositeLane?.Invoke(true);
            }
            else
            {
                onPlayerGoOppositeLane?.Invoke(false);
            }
        }

        public float GetCurrentSpeed()
        {
            return player.carForwardSpeed;
        }

        public float GetDistanceTravel()
        {
            return distanceTravel;
        }

        private void OnEnable()
        {
            onPlayerMovementChange += SwitchLane;
            GameManager.Instance.GetDayTimeManager().onDayTimeChanged += ControlPlayerLights;
            GameManager.Instance.OnGameStateChanged += CheckIfPlayerMove;
            GameManager.Instance.OnGameSettingsChanged += ApplySettingsToPlayer;
            GameManager.Instance.SetPlayerController(this);
        }

        private void OnDisable()
        {
            onPlayerMovementChange -= SwitchLane;

            if(GameManager.Instance != null)
            {
                GameManager.Instance.GetDayTimeManager().onDayTimeChanged -= ControlPlayerLights;
                GameManager.Instance.OnGameStateChanged -= CheckIfPlayerMove;
                GameManager.Instance.OnGameSettingsChanged -= ApplySettingsToPlayer;
                GameManager.Instance.SetPlayerController(null);
            }
        }
    }

}
