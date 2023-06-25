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
    private float _activeBarrierDuration;

    private float _arcaneBarrierCurrentCD = 0;
    public float ArcaneBarrierCurrentCD
    {
        get
        {
            return _arcaneBarrierCurrentCD;
        }
        private set
        {
            _arcaneBarrierCurrentCD = Mathf.Clamp(value, 0, _arcaneBarrierCooldown);
        }
    }


    [Header("Rogue Abilty Properties")]
    [SerializeField] private float _shadowShotCooldown = 3;

    private float _shadowShotCurrentCD = 0;
    public float ShadowShotCurrentCD
    {
        get
        {
            return _shadowShotCurrentCD;
        }
        private set
        {
            _shadowShotCurrentCD = Mathf.Clamp(value,0,_shadowShotCooldown);
        }
    }

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

    /// <summary>
    /// Only reduce durations when there is a player change
    /// </summary>
    public void ReduceDurations()
    {
        ReduceActiveBarrierDuration();
    }

    /// <summary>
    /// Reduce the cooldown when a player starts its turn (including extra turns)
    /// </summary>
    /// <param name="player"></param>
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
        else
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
                        _activeBarrierDuration = _arcaneBarrierDuration;
                        return true;
                    }
                }

            }
        }
        return false;
    }

    private void ReduceActiveBarrierDuration()
    {
        if (_activeBarrier == null) return; // Only reduce duration if there is an active barrier.
        
        
        _activeBarrierDuration--;
        Debug.Log("Active barrier duration:" + _activeBarrierDuration);
        if(_activeBarrierDuration <= 0)
        {
            Destroy(_activeBarrier.gameObject);
            _activeBarrier = null;
        }
    }

    private bool UseShadowShot()
    {
        if (GM.P2BallType == BallType.None) return false;
        ShadowShotCurrentCD = _shadowShotCooldown;
        StartCoroutine(ShadowShotCoroutine());
        return true;
    }

    private IEnumerator ShadowShotCoroutine()
    {

        GM.CueBall.SetPhasing(GM.P2BallType, true);

        yield return new WaitUntil(() => GM.CurrentAction == PlayerAction.Starting);

        GM.CueBall.SetPhasing(GM.P2BallType, false);
    }

}
