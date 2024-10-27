using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffect : ScriptableObject
{
    [TextArea]
    public string equipmentEffectDescription;

    public virtual void ExecuteItemEffect(Transform _targetTransform)
    {
        Debug.Log("Execute item effect");
    }
}
