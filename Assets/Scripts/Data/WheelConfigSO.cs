using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WheelConfig", menuName = "Vertigo/Wheel Config")]
public class WheelConfigSO : ScriptableObject
{
    public List<WheelSliceData> slices = new List<WheelSliceData>();
}