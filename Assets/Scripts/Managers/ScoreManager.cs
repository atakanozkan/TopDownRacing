using System.Collections;
using System.Collections.Generic;
using TDR.Controllers;
using UnityEngine;
using System;
using TDR.Helpers;
using UnityEngine.XR;
using Unity.VisualScripting;

namespace TDR.Managers
{
    [Serializable]
    public class GameModeBonus
    {
        public GameMode GameMode;
        public bool IsNightBonusAvailable;
        public bool IsOppositeLaneBonusAvailable;
        public float OppositeLaneBonusMultiplier;
        public float NightBonusMultpilier;
    }

    public class ScoreManager : MonoBehaviour
    {
        private float currentScore;

        [SerializeField]
        private List<GameModeBonus> listOfBonuses;

        public float scoreOffset = 1.3f;
        private float nightDriveMultiplier = 1f;
        private float counterLineMultipler = 1f;
        private float defaultCounterLineMultipler;

        private void Start()
        {
            defaultCounterLineMultipler = counterLineMultipler;
        }

        void Update()
        {
            if(GameManager.Instance.currentGameState == Helpers.GameState.Playing)
            {
                currentScore += CalculateCurrentScore();
            }
        }

        public float CalculateCurrentScore()
        {
            float tempScoreOffset = scoreOffset;
            float distanceMade = GameManager.Instance.GetPlayerController().GetCurrentSpeed();

            tempScoreOffset *= counterLineMultipler;
            tempScoreOffset *= nightDriveMultiplier;

            return tempScoreOffset * (distanceMade) * Time.deltaTime;
        }

        public void ActivateOppositeLaneBonus(bool active)
        {
            GameMode currentMode = GameManager.Instance.GetCurrentGameMode();
            GameModeBonus bonus = FindBonusForMode(currentMode);

            if (bonus.IsOppositeLaneBonusAvailable)
            {
                if (active)
                {
                    counterLineMultipler = bonus.OppositeLaneBonusMultiplier;
                }
                else
                {
                    counterLineMultipler = 1f;
                }
            }

        }

        public void ActivateNightTimeBonus(DayTimeType type)
        {
            GameMode currentMode = GameManager.Instance.GetCurrentGameMode();
            GameModeBonus bonus = FindBonusForMode(currentMode);

            if (bonus.IsNightBonusAvailable)
            {
                if (type == DayTimeType.Night)
                {
                    nightDriveMultiplier = bonus.NightBonusMultpilier;
                }
                else
                {
                    nightDriveMultiplier = 1f;
                }
            }
        }

        public void ResetScore()
        {
            currentScore = 0;
            nightDriveMultiplier = 1f;
            counterLineMultipler = 1f;
        }

        public float GetCurrentScore()
        {
            return currentScore;
        }

        public GameModeBonus FindBonusForMode(GameMode mode)
        {
            return listOfBonuses.Find(X => X.GameMode == mode);
        }

        public float GetNightBonusMultiplier()
        {
            GameMode mode = GameManager.Instance.GetCurrentGameMode();
            GameModeBonus bonus = FindBonusForMode(mode);
            return bonus.NightBonusMultpilier;
        }

        public float GetOppositeLaneBonusMultiplier()
        {
            GameMode mode = GameManager.Instance.GetCurrentGameMode();
            GameModeBonus bonus = FindBonusForMode(mode);
            return bonus.OppositeLaneBonusMultiplier;
        }

        public bool IsNightBonusAvailableForMode()
        {
            GameMode mode = GameManager.Instance.GetCurrentGameMode();
            GameModeBonus bonus = FindBonusForMode(mode);
            return bonus.IsNightBonusAvailable;
        }

        private void OnEnable()
        {
            GameManager.Instance.GetPlayerController().onPlayerGoOppositeLane += ActivateOppositeLaneBonus;
            GameManager.Instance.GetDayTimeManager().onDayTimeChanged += ActivateNightTimeBonus;
        }

        private void OnDisable()
        {
            if(GameManager.Instance != null && GameManager.Instance.GetPlayerController() != null)
            {
                GameManager.Instance.GetPlayerController().onPlayerGoOppositeLane -= ActivateOppositeLaneBonus;
                GameManager.Instance.GetDayTimeManager().onDayTimeChanged -= ActivateNightTimeBonus;
            }
        }
    }

}
