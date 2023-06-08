using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public enum BallType
{
    None,
    Positive,
    Negetive,
}

public enum BallColor
{
    White,
    Black,
    Cyan,
    Yellow,
    Magenta,
    Red,
    Violet,
    Green,
    Orange,
}




[CreateAssetMenu(fileName = "Ball", menuName = "Billiard/Ball SO")]
public class BallData : ScriptableObject
{
    public BallType BallType;
    public BallColor BallColor;
    [ColorUsageAttribute(true, true)] public Color Color;
    public Material Material;
}
