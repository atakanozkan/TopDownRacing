using System;
using System.Collections;
using System.Collections.Generic;
using TDR.Helpers;
using UnityEngine;

namespace TDR.Managers
{
    public class DayTimeManager : MonoBehaviour
    {
        [SerializeField, Range(0, 24)] private float _timeOfDay;
        public float TimeOfDay
        {
            get { return _timeOfDay; }
            set { _timeOfDay = value; }

        }
        [SerializeField] private List<DayTime> listOfDayTimes;
        [SerializeField] private float timeOffset = 1.0f;
        [SerializeField] private LightingManager lightingManager;

        public DayTime currentDayTimeZone;
        public Action<DayTimeType> onDayTimeChanged;

        public bool isFullDayEnabled = false;

        public void StartNewZone(GameMode gameMode)
        {
            if (gameMode == GameMode.FullDayMode)
            {
                isFullDayEnabled = true;
                _timeOfDay = UnityEngine.Random.Range(0,24);
            }
            else
            {
                isFullDayEnabled = false;
                if (gameMode == GameMode.NightMode)
                {
                    float midDiff = ((24 - GetDateTimeZone(DayTimeType.Night).timeLine[0] )+
                        GetDateTimeZone(DayTimeType.Night).timeLine[1]) / 2;
                    _timeOfDay = (GetDateTimeZone(DayTimeType.Night).timeLine[0] + midDiff) % 24;
                }
                else
                {
                    float midDiff = (GetDateTimeZone(DayTimeType.Noon).timeLine[1]-
                        GetDateTimeZone(DayTimeType.Morning).timeLine[0]
                     ) / 2;
                    _timeOfDay = (GetDateTimeZone(DayTimeType.Morning).timeLine[0] + midDiff) % 24;
                }

                DayTime checkDateTime = GetDayTimeZoneByTime();
                lightingManager.UpdateLighting(_timeOfDay / 24f);
                ChangeCurrentDayTime(checkDateTime);
            }
        }

        private void Update()
        {
            if(GameManager.Instance.currentGameState != Helpers.GameState.Playing)
            {
                return;
            }

            if (isFullDayEnabled)
            {

                if (Application.isPlaying)
                {
                    TimeOfDay += Time.deltaTime * GetTimePassOffset();
                    TimeOfDay %= 24;
                    lightingManager.UpdateLighting(TimeOfDay / 24f);
                }
                else
                {
                    lightingManager.UpdateLighting(TimeOfDay / 24f);
                }


                DayTime checkDateTime = GetDayTimeZoneByTime();
                if (currentDayTimeZone.dayTimeType != checkDateTime.dayTimeType)
                {
                    ChangeCurrentDayTime(checkDateTime);
                }
            }

        }

        public DayTime GetDayTimeZoneByTime()
        {
            foreach (DayTime dayTime in listOfDayTimes)
            {
                float startTime = dayTime.timeLine[0];
                float endTime = dayTime.timeLine[1];

                if (startTime < endTime)
                {
                    if (_timeOfDay >= startTime && _timeOfDay < endTime)
                    {
                        return dayTime;
                    }
                }
                else
                {
                    if (_timeOfDay >= startTime || _timeOfDay < endTime)
                    {
                        return dayTime;
                    }
                }
            }
            return new DayTime();
        }

        public DayTime GetDateTimeZone(DayTimeType type)
        {
            foreach (DayTime dayTime in listOfDayTimes)
            {
                if (dayTime.dayTimeType == type)
                {
                    return dayTime;
                }
            }
            return new DayTime();
        }


        public void ChangeCurrentDayTime(DayTime dayTime)
        {
            if (currentDayTimeZone.dayTimeType != dayTime.dayTimeType)
            {
                currentDayTimeZone = dayTime;
                onDayTimeChanged?.Invoke(currentDayTimeZone.dayTimeType);
            }
        }

        public bool IsNightTime()
        {
            return currentDayTimeZone.dayTimeType == DayTimeType.Night;
        }

        public float GetTimePassOffset()
        {
            return timeOffset;
        }
    }
}

