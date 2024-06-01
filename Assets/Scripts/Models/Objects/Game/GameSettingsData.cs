using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TDR.Managers.InputManager;

namespace TDR.Models.Game
{
    public class GameSettingsDataProperty
    {
        public int MinivanNumber { get; set; }
        public int PoliceCarNumber { get; set; }
        public int AmbulanceNumber { get; set; }
        public int TruckNumber { get; set; }
        public int PlayerMaxSpeed { get; set; }
        public int PlayerNormalSpeed { get; set; }
        public int AccelerationOffset { get; set; }
        public int BrakePowerOffset { get; set; }
        public int PlayerColorIndex { get; set; }
        public KeyCode AccelerateKey { get; set; }
        public KeyCode BrakeKey { get; set; }
        public KeyCode MoveRightKey { get; set; }
        public KeyCode MoveLeftKey { get; set; }


        public GameSettingsData ToField()
        {
            return new GameSettingsData(
                MinivanNumber = MinivanNumber,
                PoliceCarNumber = PoliceCarNumber,
                AmbulanceNumber = AmbulanceNumber,
                TruckNumber = TruckNumber,
                PlayerMaxSpeed = PlayerMaxSpeed,
                PlayerNormalSpeed = PlayerNormalSpeed,
                AccelerationOffset = AccelerationOffset,
                BrakePowerOffset = BrakePowerOffset,
                PlayerColorIndex = PlayerColorIndex,
                AccelerateKey = AccelerateKey,
                BrakeKey = BrakeKey,
                MoveRightKey = MoveRightKey,
                MoveLeftKey = MoveLeftKey
            );
        }
    }

    [Serializable]
    public class GameSettingsData
    {
        public int MinivanNumber;
        public int PoliceCarNumber;
        public int AmbulanceNumber;
        public int TruckNumber;
        public int PlayerMaxSpeed;
        public int PlayerNormalSpeed;
        public int AccelerationOffset;
        public int BrakePowerOffset;
        public int PlayerColorIndex;
        public KeyCode AccelerateKey ;
        public KeyCode BrakeKey;
        public KeyCode MoveRightKey;
        public KeyCode MoveLeftKey;

        public GameSettingsData(
            int minivanNumber, int policeCarNumber, int ambulanceNumber, int truckNumber,
            int playerMaxSpeed, int playerNormalSpeed, int accelerationOffset, int brakePowerOffset,
            int playerColorIndex, KeyCode AccelerateKey, KeyCode BrakeKey, KeyCode MoveRightKey, KeyCode MoveLeftKey)
        {
            MinivanNumber = minivanNumber;
            PoliceCarNumber = policeCarNumber;
            AmbulanceNumber = ambulanceNumber;
            TruckNumber = truckNumber;
            PlayerMaxSpeed = playerMaxSpeed;
            PlayerNormalSpeed = playerNormalSpeed;
            AccelerationOffset = accelerationOffset;
            BrakePowerOffset = brakePowerOffset;
            PlayerColorIndex = playerColorIndex;
            this.AccelerateKey = AccelerateKey;
            this.BrakeKey = BrakeKey;
            this.MoveRightKey = MoveRightKey;
            this.MoveLeftKey = MoveLeftKey;
        }

        public GameSettingsDataProperty ToProperty()
        {
            return new GameSettingsDataProperty
            {
                MinivanNumber = MinivanNumber,
                PoliceCarNumber = PoliceCarNumber,
                AmbulanceNumber = AmbulanceNumber,
                TruckNumber = TruckNumber,
                PlayerMaxSpeed = PlayerMaxSpeed,
                PlayerNormalSpeed = PlayerNormalSpeed,
                AccelerationOffset = AccelerationOffset,
                BrakePowerOffset = BrakePowerOffset,
                PlayerColorIndex = PlayerColorIndex,
                AccelerateKey = AccelerateKey,
                BrakeKey = BrakeKey,
                MoveRightKey = MoveRightKey,
                MoveLeftKey = MoveLeftKey
            };
        }

        public void SetKeyBindings(KeyBindings bindings)
        {
            AccelerateKey = bindings.AccelerateKey;
            BrakeKey = bindings.BrakeKey;
            MoveRightKey = bindings.MoveRightKey;
            MoveLeftKey = bindings.MoveLeftKey;
        }
    }
}