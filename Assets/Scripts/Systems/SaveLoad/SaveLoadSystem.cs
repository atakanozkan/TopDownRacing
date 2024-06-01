using System;
using System.Collections;
using System.Collections.Generic;
using TDR.Controllers;
using TDR.Helpers;
using TDR.Managers;
using TDR.Models.Game;
using UnityEngine;

namespace TDR.System.SaveLoad
{
    [Serializable]
    public class GameModeData
    {
        public TDR.Helpers.GameMode Mode;
        public float Score;
        public float Distance;

        public GameModeData(TDR.Helpers.GameMode mode, float score, float distance)
        {
            Mode = mode;
            Score = score;
            Distance = distance;
        }
    }

    public static class SaveLoadSystem
    {
        private const string DayLightModeKey = "DayLightMode";
        private const string NightModeKey = "NightMode";
        private const string FullDayModeKey = "FullDayMode";
        private const string GameSettingsKey = "GameSettings";

        public static void SaveGameModeData(GameModeData data)
        {
            string key = GetKey(data.Mode);
            string jsonData = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(key, jsonData);
            PlayerPrefs.Save();
        }

        public static void SaveHighScoreAndDistanceForMode(GameMode chosenGameMode, float gameScore, float gameDistance)
        {
            GameModeData savedData = LoadGameModeData(chosenGameMode);
            if (savedData == null || gameScore > savedData.Score || gameDistance > savedData.Distance)
            {
                float newScore = Mathf.Max(gameScore, savedData?.Score ?? 0);
                float newDistance = Mathf.Max(gameDistance, savedData?.Distance ?? 0);

                if (savedData == null || newScore != savedData.Score || newDistance != savedData.Distance)
                {
                    var data = new GameModeData(chosenGameMode, newScore, newDistance);
                    SaveGameModeData(data);
                }
            }
        }

        public static GameModeData LoadGameModeData(TDR.Helpers.GameMode mode)
        {
            string key = GetKey(mode);
            if (PlayerPrefs.HasKey(key))
            {
                string jsonData = PlayerPrefs.GetString(key);
                return JsonUtility.FromJson<GameModeData>(jsonData);
            }
            else
            {
                return null;
            }
        }

        public static void SaveGameSettings(GameSettingsData settingsData)
        {
            string jsonData = JsonUtility.ToJson(settingsData);
            PlayerPrefs.SetString(GameSettingsKey, jsonData);
            PlayerPrefs.Save();
        }

        public static GameSettingsData LoadGameSettings()
        {
            if (PlayerPrefs.HasKey(GameSettingsKey))
            {
                string jsonData = PlayerPrefs.GetString(GameSettingsKey);
                return JsonUtility.FromJson<GameSettingsData>(jsonData);
            }
            else
            {
                return null;
            }
        }

        private static string GetKey(Helpers.GameMode mode)
        {
            switch (mode)
            {
                case Helpers.GameMode.DayLightMode:
                    return DayLightModeKey;
                case Helpers.GameMode.NightMode:
                    return NightModeKey;
                case Helpers.GameMode.FullDayMode:
                    return FullDayModeKey;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
