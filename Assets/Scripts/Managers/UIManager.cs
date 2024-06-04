using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using TDR.Helpers;
using TDR.Managers;
using Unity.VisualScripting;
using TDR.Models;
using UnityEngine.SocialPlatforms.Impl;
using System.Linq;
using TDR.Models.Game;

namespace TDR.Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private Button settingsButton;

        [SerializeField] private TextMeshProUGUI topGamePanelScoreText;
        [SerializeField] private TextMeshProUGUI topGamePanelSpeedText;
        [SerializeField] private TextMeshProUGUI topGamePanelDistanceText;
        [SerializeField] private TextMeshProUGUI resumeCountDownText;
        [SerializeField] private TextMeshProUGUI gameOverPanelScoreText;
        [SerializeField] private TextMeshProUGUI gameOverPanelDistanceText;
        [SerializeField] private TextMeshProUGUI oppositeLaneBonusText;
        [SerializeField] private TextMeshProUGUI nightTimeBonusText;

        #region Canvas Groups
        [SerializeField] private CanvasGroup settingsPanel;
        [SerializeField] private CanvasGroup controlsPanel;
        [SerializeField] private CanvasGroup gamePausePanel;
        [SerializeField] private CanvasGroup mainMenu;
        [SerializeField] private CanvasGroup chooseModePanel;
        [SerializeField] private CanvasGroup countdownPanel;
        [SerializeField] private CanvasGroup gamePanel;
        [SerializeField] private CanvasGroup gameOverPanel;
        #endregion

        [SerializeField] private Image mainMask;
        [SerializeField] private Image worldMask;
        [SerializeField] private Image menuBackground;

        [SerializeField] private List<GameModeUIData> gameModeUIDataItems;
        private List<CanvasGroup> listOfPanels;

        public bool isGamePauseOpen = false;

        public int speedTextOffset = 10;

        private void Start()
        {
            listOfPanels = new List<CanvasGroup>
            {
                controlsPanel,
                settingsPanel,
                gamePausePanel,
                mainMenu,
                chooseModePanel,
                countdownPanel,
                gamePanel,
                gameOverPanel
            };
        }

        private void Update()
        {
            if(GameManager.Instance.currentGameState == GameState.Playing)
            {
                UpdateTopPanelItems();
            }
        }

        private void UpdateTopPanelItems()
        {
            topGamePanelSpeedText.text =((int) (GameManager.Instance.GetPlayerController().GetCurrentSpeed()* speedTextOffset)).ToString()+ " KM\\H";
            float distance = GameManager.Instance.GetPlayerController().GetDistanceTravel();

            float distanceInKm = distance / 1000f; 
            topGamePanelDistanceText.text = distanceInKm.ToString("F2") + " KM";

            topGamePanelScoreText.text = ((int) GameManager.Instance.GetScoreManager().GetCurrentScore()).ToString();
        }

        public void SetMaskState(Image mask, bool isActive, Action onClickAction = null)
        {
            if (isActive)
            {
                SetMaskClickAction(mask, onClickAction);
                mask.gameObject.SetActive(true);
                mask.DOFade(0.5f, 0.15f).From(0f);
            }
            else
            {
                mask.gameObject.SetActive(false);
            }
        }

        public void GoBackMenu()
        {
            OpenSinglePanel(mainMenu);
            mainMenu.gameObject.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero);
        }

        private void SetMaskClickAction(Image mask, Action action)
        {
            EventTrigger trigger = mask.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            trigger.triggers.Clear();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) => { action?.Invoke(); });
            trigger.triggers.Add(entry);
        }

        public void FadeGameOverPanel()
        {
            OpenSinglePanel(gameOverPanel);
            gameOverPanel.gameObject.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero);
        }

        public void FadeMenuPanel()
        {
            OpenSinglePanel(mainMenu);
            mainMenu.gameObject.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero);
        }

        public void FadeControlsPanel()
        {
            OpenSinglePanel(controlsPanel);
            mainMenu.gameObject.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero);
        }

        public void FadeSettingsPanel()
        {
            OpenSinglePanel(settingsPanel);
            settingsPanel.gameObject.transform.DOScale(Vector3.one, 0.7f).From(Vector3.zero);
        }

        public void FadeChooseModePanel()
        {
            OpenSinglePanel(chooseModePanel);
            chooseModePanel.gameObject.transform.DOScale(Vector3.one, 0.5f).From(Vector3.zero);
        }

        public void ControlGamePausePanel()
        {
            if (!isGamePauseOpen)
            {
                FadeInGamePausePanel();
            }
            else
            {
                FadeOutGamePausePanelPanel();
            }
        }

        public void FadeInGamePausePanel()
        {
            isGamePauseOpen = true;
            DOTween.Kill(gamePausePanel.transform);
            SetMaskState(mainMask, true, FadeOutGamePausePanelPanel);
            OpenSinglePanel(gamePausePanel);
            GameManager.Instance.ChangeGameState(GameState.Pause);
            gamePausePanel.transform.DOScaleY(1f, 0.5f).SetEase(Ease.OutBack, 4f).From(0f);
        }

        public void FadeOutGamePausePanelPanel()
        {
            isGamePauseOpen = false;
            DOTween.Kill(gamePausePanel.transform);
            gamePausePanel.transform.DOScaleY(0f, 0.3f).OnComplete(() =>
            {
                StartCoroutine(StartCountdown());

            });
        }

        public void ControlPanels(GameState gameState)
        {
            if (gameState == GameState.GameOver)
            {
                GameStatData gameStats = GameManager.Instance.GetGameStatsData();
                gameOverPanelScoreText.text = ((int) gameStats.Score).ToString();
                gameOverPanelDistanceText.text = (gameStats.Distance / 1000f).ToString("F2") + " KM";
                mainMask.DOFade(0.5f, 0.5f).SetDelay(0.1f).From(0f);
                nightTimeBonusText.gameObject.SetActive(false);
                oppositeLaneBonusText.gameObject.SetActive(false);
                SetMaskState(mainMask, true);
                isGamePauseOpen = false;
                DG.Tweening.Sequence sequence = DOTween.Sequence();
                sequence.PrependInterval(0.8f).OnComplete(() => FadeGameOverPanel());
            }
            else if (gameState == GameState.MainMenu)
            {
                SetMaskState(mainMask, false);
                DG.Tweening.Sequence sequence = DOTween.Sequence();
                sequence.PrependInterval(0.2f).OnComplete(() => FadeMenuPanel());
            }
            else if (gameState == GameState.Playing)
            {
                mainMask.DOFade(0.5f, 0.5f).SetDelay(0.8f).From(0f);
                SetMaskState(mainMask, false);
                OpenSinglePanel(gamePanel);
            }
            else if (gameState == GameState.Settings) {
                SetMaskState(mainMask, false);
                FadeSettingsPanel();
            }
            else if (gameState == GameState.ControlBinding)
            {
                SetMaskState(mainMask, false);
                FadeControlsPanel();
            }
        }

        private void UpdateOppositeLaneBonusUI(bool active)
        {
            if (active)
            {
                if(oppositeLaneBonusText.gameObject.active == false) {
                    oppositeLaneBonusText.gameObject.SetActive(true);
                    oppositeLaneBonusText.text =
                    "Opposite Lane Bonus: " + GameManager.Instance.GetScoreManager().
                    GetOppositeLaneBonusMultiplier().ToString() + "x";
                    oppositeLaneBonusText.DOFade(1f, 1f).SetDelay(0.1f).From(0f);
                }
            }
            else
            {
                oppositeLaneBonusText.gameObject.SetActive(false);
            }
        }

        private void UpdateNightTimeBonusUI(DayTimeType type)
        {
            if(type == DayTimeType.Night &&
                GameManager.Instance.GetScoreManager().
                IsNightBonusAvailableForMode())
            {
                if (nightTimeBonusText.gameObject.active == false)
                {
                    nightTimeBonusText.gameObject.SetActive(true);
                    nightTimeBonusText.text =
                        "Night Bonus: " + GameManager.Instance.GetScoreManager().GetNightBonusMultiplier().ToString()+"x";
                    nightTimeBonusText.DOFade(1f, 1f).SetDelay(0.1f).From(0f);
                }
            }
            else
            {
                nightTimeBonusText.gameObject.SetActive(false);
            }
        }

        private IEnumerator StartCountdown()
        {
            OpenSinglePanel(countdownPanel);
            SetMaskState(mainMask, true);
            for (int i = 3; i > 0; i--)
            {
                resumeCountDownText.text = i.ToString();
                yield return new WaitForSeconds(1f);
            }
            GameManager.Instance.ChangeGameState(GameState.Playing);
            SetMaskState(mainMask, false);
        }

        public void UpdateGameDataUIDataItem(GameMode gameMode, float score, float distance)
        {
            GameModeUIData gameModeUIData = GetGameModeUIDataItem(gameMode);

            if(gameModeUIData != null)
            {
                gameModeUIData.Score = score.ToString();
                float distanceInKm = distance / 1000f;
                gameModeUIData.Distance = distanceInKm.ToString("F2") + " KM";
                gameModeUIData.TargetScoreTextMesh.text = "Highest Score: " + gameModeUIData.Score;
                gameModeUIData.TargetDistanceTextMesh.text = "Highest Distance: " + gameModeUIData.Distance;
            }
        }

        private GameModeUIData GetGameModeUIDataItem(GameMode gameMode)
        {
            return gameModeUIDataItems.FirstOrDefault(item => item.Mode == gameMode);
        }


        public void OpenSinglePanel(CanvasGroup currentPanel)
        {
            foreach(CanvasGroup panel in listOfPanels)
            {
                if(currentPanel == panel)
                {
                    panel.gameObject.SetActive(true);
                }
                else
                {
                    panel.gameObject.SetActive(false);
                }
            }

            if (currentPanel == mainMenu || currentPanel == chooseModePanel || currentPanel == gameOverPanel || currentPanel == controlsPanel)
            {
                menuBackground.gameObject.SetActive(true);
            }
            else
            {
                menuBackground.gameObject.SetActive(false);
            }
        }

        public Image GetMainMask()
        {
            return mainMask;
        }

        private void OnEnable()
        {
            GameManager.Instance.OnGameStateChanged += ControlPanels;
            GameManager.Instance.GetPlayerController().onPlayerGoOppositeLane += UpdateOppositeLaneBonusUI;
            GameManager.Instance.GetDayTimeManager().onDayTimeChanged += UpdateNightTimeBonusUI;
        }

        private void OnDisable()
        {
            if(GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= ControlPanels;
                GameManager.Instance.GetPlayerController().onPlayerGoOppositeLane -= UpdateOppositeLaneBonusUI;
                GameManager.Instance.GetDayTimeManager().onDayTimeChanged -= UpdateNightTimeBonusUI;
            }
        }
    }
}