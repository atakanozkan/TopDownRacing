using System.Collections;
using System.Collections.Generic;
using TDR.Controllers;
using System;
using TDR.Helpers;
using UnityEngine;
using TDR.Models.Game;

namespace TDR.Managers
{
    public class InputManager : MonoBehaviour
    {
        [Serializable]
        public class KeyBindings
        {
            public KeyCode AccelerateKey = KeyCode.UpArrow;
            public KeyCode BrakeKey = KeyCode.DownArrow;
            public KeyCode MoveRightKey = KeyCode.RightArrow;
            public KeyCode MoveLeftKey = KeyCode.LeftArrow;
        }

        [SerializeField]
        private PlayerController playerController;

        [SerializeField]
        private KeyBindings keyBindings;

        private bool resume = true;

        private void Update()
        {
            GenerateInput();
        }

        private void GenerateInput()
        {
            if (!resume || playerController.isChangingLane)
            {
                return;
            }

            if (Input.GetKey(keyBindings.AccelerateKey))
            {
                playerController.Accelerate();
            }
            else if (Input.GetKey(keyBindings.BrakeKey))
            {
                playerController.Brake();
            }
            else
            {
                playerController.ReleaseGas();
            }

            if (Input.GetKeyDown(keyBindings.MoveRightKey))
            {
                playerController.TriggerPlayerMovement(TDR.Helpers.MoveState.RightMove);
            }
            else if (Input.GetKeyDown(keyBindings.MoveLeftKey))
            {
                playerController.TriggerPlayerMovement(TDR.Helpers.MoveState.LeftMove);
            }
        }

        private void CheckResumeInput(GameState state)
        {
            if(state != GameState.Playing)
            {
                resume = false;
            }
            else
            {
                resume = true;
            }
        }

        public void ApplyKeyBindingSettings(GameSettingsData data)
        {
            keyBindings = new KeyBindings()
            {
                AccelerateKey = data.AccelerateKey,
                BrakeKey = data.BrakeKey,
                MoveLeftKey = data.MoveLeftKey,
                MoveRightKey = data.MoveRightKey
            };
        }

        public KeyBindings GetKeyBindings()
        {
            return keyBindings;
        }

        public void SetKeyBindings(KeyBindings bindings)
        {
            keyBindings = bindings;
        }

        private void OnEnable()
        {
            GameManager.Instance.OnGameStateChanged += CheckResumeInput;
            GameManager.Instance.OnGameSettingsChanged += ApplyKeyBindingSettings;
        }

        private void OnDisable()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= CheckResumeInput;
                GameManager.Instance.OnGameSettingsChanged -= ApplyKeyBindingSettings;
            }
        }

    }

}
