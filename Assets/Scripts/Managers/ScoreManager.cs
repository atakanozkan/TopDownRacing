using System.Collections;
using System.Collections.Generic;
using TDR.Controllers;
using UnityEngine;

namespace TDR.Managers
{
    public class ScoreManager : MonoBehaviour
    {

        [SerializeField] public PlayerController playerController;
        [SerializeField] public DayTimeManager dayTimeManager;
        private float currentScore;

        public float scoreOffset = 1.3f;
        public float nightDriveMultiplier;
        public float counterLineMultipler;

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
            float distanceMade = playerController.GetCurrentSpeed();
            if (playerController.CheckPlayerIfOppositeLine())
            {
                tempScoreOffset *= counterLineMultipler;
            }

            if (dayTimeManager.IsNightTime())
            {
                tempScoreOffset *= nightDriveMultiplier;
            }

            return tempScoreOffset * (distanceMade) * Time.deltaTime;
        }

        public void ResetScore()
        {
            currentScore = 0;
        }

        public float GetCurrentScore()
        {
            return currentScore;
        }
    }

}
