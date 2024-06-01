using System;
using UnityEngine;
using TDR.Helpers;
using TDR.Patterns;
using TDR.Controllers;
using TDR.Models;
using TDR.System.SaveLoad;
using System.Collections.Generic;
using TDR.Models.Game;

namespace TDR.Managers
{
    public class GameManager : Singleton<GameManager>
    {
        #region Serialized Field
        [SerializeField]
        private RoadManager roadManager;
        [SerializeField]
        private DayTimeManager dayTimeManager;
        [SerializeField]
        private PlayerController playerController;
        [SerializeField]
        private ScoreManager scoreManager;
        [SerializeField]
        private NPCCarManager npcCarManager;
        [SerializeField]
        private UIManager uiManager;
        [SerializeField]
        private InputManager inputManager;
        #endregion

        [SerializeField]
        private GameSettingsData defaultGameSettingsData;
        [SerializeField]
        private GameSettingsRuleField gameSettingsRuleField;

        private GameMode chosenGameMode;
        private GameSettingsData currentGameSettingsData;

        public List<GameMode> listOfGameModes;
        public GameState currentGameState;

        #region Action
        public Action<GameState> OnGameStateChanged;
        public Action<GameSettingsData> OnGameSettingsChanged;
        #endregion


       
        private void Start()
        {
            LoadSettings();
            TriggerGameSettingsChanged(currentGameSettingsData);
            npcCarManager.BuildInitialItems();
            roadManager.BuildInitialRoadItems();
            ChangeGameState(GameState.MainMenu);
            LoadAllDataAndUpdateUI();
        }


        public void StartNewGame(GameModeItem gameModeItem)
        {
            scoreManager.ResetScore();
            playerController.BuildPlayer();
            chosenGameMode = gameModeItem.gameMode;
            roadManager.GenerateStartRoad();
            npcCarManager.BuildControlDictionaries();
            dayTimeManager.StartNewZone(chosenGameMode);
            ChangeGameState(GameState.Playing);
        }

        public void TryAgain()
        {
            scoreManager.ResetScore();
            playerController.BuildPlayer();
            roadManager.GenerateStartRoad();
            npcCarManager.BuildControlDictionaries();
            dayTimeManager.StartNewZone(chosenGameMode);
            ChangeGameState(GameState.Playing);
        }

        public void TriggerGameOver()
        {
            SaveHighScoreAndDistanceForMode();
            LoadAllDataAndUpdateUI();
            ChangeGameState(GameState.GameOver);
        }

        public void TriggerMainMenuSection()
        {
            ChangeGameState(GameState.MainMenu);
        }

        public void TriggerSettingsSection()
        {
            ChangeGameState(GameState.Settings);
        }

        public void TriggerControlBindingSection()
        {
            ChangeGameState(GameState.ControlBinding);
        }

        public void ChangeGameState(GameState state)
        {
            if (currentGameState != state)
            {
                currentGameState = state;
                OnGameStateChanged?.Invoke(state);
            }

        }

        public GameModeData LoadGameModeData(GameMode mode)
        {
            return SaveLoadSystem.LoadGameModeData(mode);
        }

        public void SaveHighScoreAndDistanceForMode()
        {
            SaveLoadSystem.SaveHighScoreAndDistanceForMode(
                chosenGameMode,
                scoreManager.GetCurrentScore(),
                playerController.GetDistanceTravel());
        }

        public void LoadAllDataAndUpdateUI()
        {
            foreach(GameMode gameMode in listOfGameModes)
            {
                GameModeData gameModeData = LoadGameModeData(gameMode);
                if(gameModeData == null) 
                {
                    continue;
                }
                uiManager.UpdateGameDataUIDataItem(
                    gameModeData.Mode,
                    gameModeData.Score,
                    gameModeData.Distance
                    );
            }
        }

        public GameStatData GetGameStatsData()
        {
            return new GameStatData()
            {
                Mode = chosenGameMode,
                Score = scoreManager.GetCurrentScore(),
                Distance = playerController.GetDistanceTravel()
            };
        }

        public void LoadSettings()
        {
            GameSettingsData gameSettingsData = SaveLoadSystem.LoadGameSettings();

            if(gameSettingsData == null)
            {
                currentGameSettingsData = defaultGameSettingsData;
            }
            else
            {
                currentGameSettingsData = gameSettingsData;
            }
        }

        public void SaveGameSettings(GameSettingsData data)
        {
            TriggerGameSettingsChanged(data);
            SaveLoadSystem.SaveGameSettings(currentGameSettingsData);
        }

        public GameSettingsData GetGameSettings()
        {
            return currentGameSettingsData;
        }


        public GameSettingsData GetDefaultGameSettings()
        {
            return defaultGameSettingsData;
        }

        public void SetGameSettingsData(GameSettingsData gameSettingsData)
        {
            currentGameSettingsData = gameSettingsData;
        }

        public void TriggerGameSettingsChanged(GameSettingsData data)
        {
            OnGameSettingsChanged?.Invoke(data);
        }


        public GameSettingsRuleProperty ConvertSettingsFieldToProperty()
        {
            return gameSettingsRuleField.ToProperty();
        }

        public void SetUIManager(UIManager manager)
        {
            uiManager = manager;
        }

        public UIManager GetUIManager()
        {
            return uiManager;
        }

        public void SetInputManager(InputManager manager)
        {
            inputManager = manager;
        }

        public InputManager GetInputManager()
        {
            return inputManager;
        }

        public void SetRoadManager(RoadManager manager)
        {
            roadManager = manager;
        }

        public RoadManager GetRoadManager()
        {
            return roadManager;
        }

        public void SetPlayerController(PlayerController controller)
        {
            playerController = controller;
        }

        public PlayerController GetPlayerController()
        {
            return playerController;
        }

        public void SetDayTimeManager(DayTimeManager manager)
        {
            dayTimeManager = manager;
        }

        public DayTimeManager GetDayTimeManager()
        {
            return dayTimeManager;
        }

        public void SetScoreManager(ScoreManager manager)
        {
            scoreManager = manager;
        }

        public ScoreManager GetScoreManager()
        {
            return scoreManager;
        }

        public void SetNPCCarManagerManager(NPCCarManager manager)
        {
            npcCarManager = manager;
        }

        public NPCCarManager GetNPCCarManagerManager()
        {
            return npcCarManager;
        }
    }
}