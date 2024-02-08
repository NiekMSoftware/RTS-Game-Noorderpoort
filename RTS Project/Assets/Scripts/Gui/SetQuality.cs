using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetQuality : MonoBehaviour
{
    public void SetQ(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void LowQuality()
    {
        QualitySettings.SetQualityLevel(0, true);
    }

    public void MediumQuality()
    {
        QualitySettings.SetQualityLevel(3, true);
    }

    public void HighQuality()
    {
        QualitySettings.SetQualityLevel(5, true);
    }
}
