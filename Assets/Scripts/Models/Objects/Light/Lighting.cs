using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Lighting", menuName = "Scriptables/Lighting", order = 1)]
public class Lighting : ScriptableObject
{
    public Gradient AmbientColor;
    public Gradient DirectionalColor;
    public Gradient FogColor;
}