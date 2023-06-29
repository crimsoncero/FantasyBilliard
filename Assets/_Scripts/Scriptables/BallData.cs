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

    /// <summary>
    /// Checks if the entered BallType is contained in the given BallData List
    /// </summary>
    /// <param name="ballDataList"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool HasType (List<BallData> ballDataList, BallType type)
    {
        foreach (BallData ball in ballDataList)
        {
            if(ball.BallType == type) return true;
        }
        return false;
    }




}
