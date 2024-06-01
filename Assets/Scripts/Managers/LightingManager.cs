using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace TDR.Managers
{

    [ExecuteAlways]
    public class LightingManager : MonoBehaviour
    {
        [SerializeField] private Light DirectionalLight;
        [SerializeField] private Lighting LightingPreset;

        public void UpdateLighting(float timePercent)
        {
            if (LightingPreset == null)
                return;

            RenderSettings.ambientLight = LightingPreset.AmbientColor.Evaluate(timePercent);
            RenderSettings.fogColor = LightingPreset.FogColor.Evaluate(timePercent);

            if (DirectionalLight != null)
            {
                DirectionalLight.color = LightingPreset.DirectionalColor.Evaluate(timePercent);

                DirectionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, 170f, 0));
            }

        }

        private void OnValidate()
        {
            if (DirectionalLight != null)
                return;

            if (RenderSettings.sun != null)
            {
                DirectionalLight = RenderSettings.sun;
            }

            else
            {
                Light[] lights = GameObject.FindObjectsOfType<Light>();
                foreach (Light light in lights)
                {
                    if (light.type == LightType.Directional)
                    {
                        DirectionalLight = light;
                        return;
                    }
                }
            }
        }

    }
}
