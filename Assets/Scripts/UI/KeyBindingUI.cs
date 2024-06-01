using System;
using System.Collections;
using System.Collections.Generic;
using TDR.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TDR.Managers.InputManager;

namespace TDR.UI
{
    public class KeyBindingUI : MonoBehaviour
    {
        private InputManager inputManager;

        public Button accelerateButton;
        public Button brakeButton;
        public Button moveRightButton;
        public Button moveLeftButton;
        public Image inputMask;
        public TextMeshProUGUI maskText;

        private void Awake()
        {
            inputManager = GameManager.Instance.GetInputManager();
            UpdateButtonTexts();

            accelerateButton.onClick.AddListener(() => StartCoroutine(WaitForKeyPress(key => inputManager.GetKeyBindings().AccelerateKey = key, "Accelerate", inputManager.GetKeyBindings().AccelerateKey)));
            brakeButton.onClick.AddListener(() => StartCoroutine(WaitForKeyPress(key => inputManager.GetKeyBindings().BrakeKey = key, "Brake", inputManager.GetKeyBindings().BrakeKey)));
            moveRightButton.onClick.AddListener(() => StartCoroutine(WaitForKeyPress(key => inputManager.GetKeyBindings().MoveRightKey = key, "MoveRight", inputManager.GetKeyBindings().MoveRightKey)));
            moveLeftButton.onClick.AddListener(() => StartCoroutine(WaitForKeyPress(key => inputManager.GetKeyBindings().MoveLeftKey = key, "MoveLeft", inputManager.GetKeyBindings().MoveLeftKey)));
        }

        private void UpdateButtonTexts()
        {
            accelerateButton.GetComponentInChildren<Text>().text = inputManager.GetKeyBindings().AccelerateKey.ToString();
            brakeButton.GetComponentInChildren<Text>().text = inputManager.GetKeyBindings().BrakeKey.ToString();
            moveRightButton.GetComponentInChildren<Text>().text = inputManager.GetKeyBindings().MoveRightKey.ToString();
            moveLeftButton.GetComponentInChildren<Text>().text = inputManager.GetKeyBindings().MoveLeftKey.ToString();
        }

        private IEnumerator WaitForKeyPress(Action<KeyCode> setKey, string action, KeyCode currentKey)
        {
            inputMask.gameObject.SetActive(true);

            bool keyPressed = false;
            KeyBindings bindings = inputManager.GetKeyBindings();

            while (!keyPressed)
            {
                foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        if (keyCode == currentKey)
                        {
                            keyPressed = true;
                            break;
                        }
                        else if (keyCode == bindings.AccelerateKey || keyCode == bindings.BrakeKey || keyCode == bindings.MoveRightKey || keyCode == bindings.MoveLeftKey)
                        {
                            maskText.text = $"Key {keyCode} is already assigned to another action. Press another key.";
                        }
                        else
                        {
                            setKey(keyCode);
                            keyPressed = true;
                            maskText.text = string.Empty;
                        }
                        break;
                    }
                }
                yield return null;
            }
            UpdateButtonTexts();
            inputMask.gameObject.SetActive(false);
        }

        public void ResetToDefault()
        {
            KeyBindings bindings = inputManager.GetKeyBindings();
            bindings.AccelerateKey = KeyCode.UpArrow;
            bindings.BrakeKey = KeyCode.DownArrow;
            bindings.MoveRightKey = KeyCode.RightArrow;
            bindings.MoveLeftKey = KeyCode.LeftArrow;
            UpdateButtonTexts();
        }

        public void SaveBindingsAndGoMenu()
        {
            GameManager.Instance.GetGameSettings().SetKeyBindings(inputManager.GetKeyBindings());
            GameManager.Instance.SaveGameSettings(GameManager.Instance.GetGameSettings());
            GameManager.Instance.TriggerMainMenuSection();
        }
    }

}