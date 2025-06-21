using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Custom/Effect",order = -1)]
public class Effect : ScriptableObject
{
    public string effectName;
    [TextArea(5,20)]
    public string effectDiscription;

    public float effectValue;
    public int effectMulti;
    public int effectLevel;

    public Sprite effectImg;
    public EffectType effectType;
}
