using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TDR.Models
{
    /*
        We need to add Max prefix before adding rule
     */
    public class GameSettingsRuleProperty
    {
        public int MaxMinivanNumber { get; set; }
        public int MaxPoliceCarNumber { get; set; }
        public int MaxAmbulanceNumber { get; set; }
        public int MaxTruckNumber { get; set; }
        public int MaxPlayerMaxSpeed { get; set; }
        public int MaxPlayerNormalSpeed { get; set; }
        public int MaxAccelerationOffset { get; set; }
        public int MaxBrakePowerOffset { get; set; }
    }

    [Serializable]
    public class GameSettingsRuleField
    {
        public int MaxMinivanNumber;
        public int MaxPoliceCarNumber;
        public int MaxAmbulanceNumber;
        public int MaxTruckNumber;
        public int MaxPlayerMaxSpeed;
        public int MaxPlayerNormalSpeed;
        public int MaxAccelerationOffset;
        public int MaxBrakePowerOffset;

        public GameSettingsRuleProperty ToProperty()
        {
            return new GameSettingsRuleProperty
            {
                MaxMinivanNumber = MaxMinivanNumber,
                MaxPoliceCarNumber = MaxPoliceCarNumber,
                MaxAmbulanceNumber = MaxAmbulanceNumber,
                MaxTruckNumber = MaxTruckNumber,
                MaxPlayerMaxSpeed = MaxPlayerMaxSpeed,
                MaxPlayerNormalSpeed = MaxPlayerNormalSpeed,
                MaxAccelerationOffset = MaxAccelerationOffset,
                MaxBrakePowerOffset = MaxBrakePowerOffset
            };
        }
    }
    
}
