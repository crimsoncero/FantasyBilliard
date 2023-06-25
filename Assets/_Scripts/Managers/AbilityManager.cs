using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityManager : Singleton<AbilityManager>
{

    [Header("Mage Abilty Properties")]
    [SerializeField] private Transform _arcaneBarrierPrefab;
    [SerializeField] private float _arcaneBarrierCooldown = 6;
    [SerializeField] private float _arcaneBarrierDuration = 2;

    private Transform _activeBarrier = null;
    public float ArcaneBarrierCurrentCD { get; private set; }

    [Header("Rogue Abilty Properties")]
    [SerializeField] private float _shadowShotCooldown = 3;

    public float ShadowShotCurrentCD { get; private set; }

    private GameManager GM { get { return GameManager.Instance; } }



    private void Update()
    {
        if(GM.CanAbility == true)
        {
            if (UseAbility(GM.CurrentPlayer))
            {
                // Player used ability
                GM.UsedAbility();
            }
        }
    }



    /// <summary>
    /// returns whether the given player ability is ready to use.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool IsAbilityReady(Player player)
    {
        return GetPlayerAbilityCD(player) == 0;
    }

    public void ReduceCD(Player player)
    {
        SetPlayerAbilityCD(player, GetPlayerAbilityCD(player) - 1);
    }

    public float GetPlayerAbilityCD(Player player)
    {
        if(player == Player.P1)
            return ArcaneBarrierCurrentCD;

        return ShadowShotCurrentCD;
    }
    private void SetPlayerAbilityCD(Player player, float val)
    {
        if (player == Player.P1)
            ArcaneBarrierCurrentCD = val;

        ShadowShotCurrentCD = val;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public bool UseAbility(Player player)
    {
        if(player == Player.P1)
        {
           return UseArcaneBarrier();
        }
        else
        {
            return UseShadowShot();
        }
    }

    private bool UseArcaneBarrier()
    {
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Ray ray = GM.MainCamera.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {

                    if (Mathf.Abs(hit.point.x) < GM.PlacementArea.localScale.x / 2 &&
                        Mathf.Abs(hit.point.z) < GM.PlacementArea.localScale.y / 2)
                    {
                        _activeBarrier = Instantiate(_arcaneBarrierPrefab, (new Vector3(hit.point.x, 0, hit.point.z)), Quaternion.identity);
                        ArcaneBarrierCurrentCD = _arcaneBarrierCooldown;
                        return true;
                    }
                }

            }
        }
        return false;



       
    }
    
    private bool UseShadowShot()
    {
        ShadowShotCurrentCD = _shadowShotCooldown;
        return true;

    }


}
