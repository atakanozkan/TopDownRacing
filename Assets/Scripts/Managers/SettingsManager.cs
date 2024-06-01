using System;
using System.Collections;
using System.Collections.Generic;
using TDR.Models;
using TDR.Models.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TDR.Managers
{
    [Serializable]
    public class ColorPicker
    {
        public int Index;
        public string ColorName;
        public Button Button;
        public Outline Outline;
    }

    [Serializable]
    public class SliderSetting
    {
        public string SettingName;
        public Slider Slider;
        public TextMeshProUGUI TextMesh;
        public int MinValue;
        public int MaxValue;
    }

    public class SettingsManager : MonoBehaviour
    {
        [SerializeField]
        private List<SliderSetting> sliderSettings;

        [SerializeField]
        private CanvasGroup colorPickerPanel;

        [SerializeField]
        private List<ColorPicker> colorButtons;

        private GameSettingsRuleProperty settingsRules;
        private GameSettingsDataProperty localSettingsProperty;
        private GameSettingsData localSettings;

        private void Start()
        {
            localSettings = GameManager.Instance.GetGameSettings();
            settingsRules = GameManager.Instance.ConvertSettingsFieldToProperty();
            localSettingsProperty = localSettings.ToProperty();

            InitializeSettings();
        }

        private void InitializeSettings()
        {
            foreach (var sliderSetting in sliderSettings)
            {
                float value = GetSettingValue(sliderSetting.SettingName);
                sliderSetting.Slider.value = value;
                sliderSetting.TextMesh.text = value.ToString();
                sliderSetting.Slider.onValueChanged.AddListener(v =>
                {
                    float floatValue = v;
                    UpdateSetting(sliderSetting.SettingName, floatValue);
                    sliderSetting.TextMesh.text = floatValue.ToString();
                });
            }

            SetSliderValuesAndLimits();
            SelectColor(localSettingsProperty.PlayerColorIndex);

            foreach (var colorButton in colorButtons)
            {
                int index = colorButton.Index;
                colorButton.Button.onClick.AddListener(() => SelectColor(index));
            }
        }

        private void SetSliderValuesAndLimits()
        {
            foreach (var sliderSetting in sliderSettings)
            {
                var ruleProperty = settingsRules.GetType().GetProperty($"Max{sliderSetting.SettingName}");
                var sliderField = typeof(GameSettingsDataProperty).GetProperty(sliderSetting.SettingName);
                if (ruleProperty != null && sliderField != null)
                {
                    sliderSetting.Slider.minValue = sliderSetting.MinValue;
                    sliderSetting.Slider.maxValue = Convert.ToSingle(ruleProperty.GetValue(settingsRules));
                    object fieldValue = sliderField.GetValue(localSettingsProperty);
                    if (fieldValue is int intValue)
                    {
                        sliderSetting.Slider.value = intValue;
                    }
                    else if (fieldValue is float floatValue)
                    {
                        sliderSetting.Slider.value = floatValue;
                    }
                    sliderSetting.TextMesh.text = sliderSetting.Slider.value.ToString();
                }
                else
                {
                    Debug.LogWarning($"Property not found: {sliderSetting.SettingName} or Max{sliderSetting.SettingName}");
                }
            }
        }

        private float GetSettingValue(string settingName)
        {
            var property = typeof(GameSettingsDataProperty).GetProperty(settingName);
            if (property != null)
            {
                return Convert.ToSingle(property.GetValue(localSettingsProperty));
            }

            return 0;
        }

        private void UpdateSetting(string settingName, float value)
        {
            var property = typeof(GameSettingsDataProperty).GetProperty(settingName);
            if (property != null)
            {
                if (property.PropertyType == typeof(int))
                {
                    property.SetValue(localSettingsProperty, (int)value);
                }
                else
                {
                    property.SetValue(localSettingsProperty, value);
                }
            }
        }

        private void SelectColor(int index)
        {
            localSettingsProperty.PlayerColorIndex = index;
            foreach (var colorButton in colorButtons)
            {
                colorButton.Outline.enabled = (colorButton.Index == index);
            }
        }

        public void ResetToDefaultSettings()
        {
            localSettings = GameManager.Instance.GetDefaultGameSettings();
            localSettingsProperty = localSettings.ToProperty();
            InitializeSettings();
        }

        public void SaveSettingsAndBackToMenu()
        {
            localSettings = localSettingsProperty.ToField();
            localSettings.SetKeyBindings(GameManager.Instance.GetInputManager().GetKeyBindings());
            GameManager.Instance.SetGameSettingsData(localSettings);
            GameManager.Instance.SaveGameSettings(localSettings);
            GameManager.Instance.TriggerMainMenuSection();
        }
    }
}
