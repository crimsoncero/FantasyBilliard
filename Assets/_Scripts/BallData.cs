using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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
}
