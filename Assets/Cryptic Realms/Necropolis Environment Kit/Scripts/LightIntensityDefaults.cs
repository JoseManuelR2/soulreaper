using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LightIntensityDefaults", menuName = "ScriptableObjects/LightIntensityDefaults")]
public class LightIntensityDefaults : ScriptableObject
{
    [System.Serializable]
    public struct LightData
    {
        public string id;
        public float intensity;
    }

    public List<LightData> lightIntensities = new List<LightData>();

    public bool TryGetDefaultIntensity(string id, out float intensity)
    {
        var data = lightIntensities.Find(l => l.id == id);
        intensity = data.intensity;
        return !string.IsNullOrEmpty(data.id);
    }

    public void SaveDefaultIntensity(string id, float intensity)
    {
        int index = lightIntensities.FindIndex(l => l.id == id);
        if (index >= 0)
        {
            lightIntensities[index] = new LightData { id = id, intensity = intensity };
        }
        else
        {
            lightIntensities.Add(new LightData { id = id, intensity = intensity });
        }
    }
}