using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public enum BallType
{
    None,
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


    public static bool HasType (List<BallData> ballDataList, BallType type)
    {
        foreach (BallData ball in ballDataList)
        {
            if(ball.BallType == type) return true;
        }
        return false;
    }




}
