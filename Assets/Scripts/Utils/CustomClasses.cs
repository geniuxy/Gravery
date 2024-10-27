using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Range
{
    public float min;
    public float max;

    public Range(float _min, float _max)
    {
        min = _min;
        max = _max;
    }
}