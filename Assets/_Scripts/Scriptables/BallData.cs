using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public enum BallType
{
    Cue,
    Solid,
    Striped,
    Black,
}






[CreateAssetMenu(fileName = "Ball", menuName = "Billiard/Ball SO")]
public class BallData : ScriptableObject
{
    public BallType BallType;
    public Texture2D Texture;
    public Material Material;
}
