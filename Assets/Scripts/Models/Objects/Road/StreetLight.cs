using System;
using System.Collections;
using TDR.Managers;
using UnityEngine;

namespace TDR.Models
{
    public class StreetLight : MonoBehaviour
    {
        [SerializeField] private GameObject streetLight;

        public void ControlLight(DayTimeType type)
        {
            StopAllCoroutines();
            if (type == DayTimeType.Night)
            {
                StartCoroutine(UseDelayOnAndOff(() => TurnOnTheLight()));
            }
            else
            {
                StartCoroutine(UseDelayOnAndOff(() => TurnOffTheLight()));
            }
        }

        private IEnumerator UseDelayOnAndOff(Action action)
        {
            yield return new WaitForSeconds(1f);
            action?.Invoke();
        }

        private void TurnOnTheLight()
        {
            streetLight.gameObject.SetActive(true);
        }

        private void TurnOffTheLight()
        {
            streetLight.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (GameManager.Instance.GetDayTimeManager().IsNightTime())
            {
                streetLight.gameObject.SetActive(true);
            }
            else
            {
                streetLight.gameObject.SetActive(false);
            }

            GameManager.Instance.GetDayTimeManager().onDayTimeChanged += ControlLight;
        }

        private void OnDisable()
        {
            if(GameManager.Instance != null)
            {
                GameManager.Instance.GetDayTimeManager().onDayTimeChanged -= ControlLight;
            }
        }
    }
}