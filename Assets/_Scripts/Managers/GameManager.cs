using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using Sequence = DG.Tweening.Sequence;

public class GameManager : StaticInstance<GameManager>
{
    public BallController CueBall;
    public BallHandler[] Balls;
    public RectTransform PlacementArea;
    public Camera MainCamera;

    public Transform[] BallTransforms;

    // Enablers
    public bool CanShoot { get; set; }
    public bool CanAbility { get; set; }

    // Game Data
    public List<BallData> BallsInGame { get; private set; }
    public GameState CurrentState { get; private set; }
    public Player CurrentPlayer { get; private set; }
    public TurnType CurrentTurnType { get; private set; }
    public BallType P1BallType { get; private set; }
    public BallType P2BallType { get; private set; }
    public bool TypeSetThisTurn { get; private set; }

    // Turn Data
    public PlayerAction CurrentAction { get; private set; }
    public BallData FirstContact { get; set; }
    public List<BallData> BallsInThisTurn { get; private set; }
    public BallType CurrentPlayerBallType { get { return CurrentPlayer == Player.P1 ? P1BallType : P2BallType; } }


    private PlayerAction _pausedAction = PlayerAction.Starting;
    private Vector3[] _ballsStartPositions;

    private bool _usedAbilityTrigger = false;
    private UiManager UI { get { return UiManager.Instance; } }
    private AbilityManager AbM { get { return AbilityManager.Instance; } }

    private void Start()
    {
        _ballsStartPositions = new Vector3[BallTransforms.Length];
        for (int i = 0; i < BallTransforms.Length; i++)
        {
            _ballsStartPositions[i] = BallTransforms[i].position;
        }
        CurrentState = GameState.Ending;
        
    }
 

    // Update is called once per frame
    void Update()
    {
        if (CurrentState == GameState.Player)
        {
            OnPlayerUpdate();
        }
    }

    private void LateUpdate()
    {
        if (_usedAbilityTrigger)
        {
            CueBall.StopAiming();
            CurrentAction = PlayerAction.Shooting;
            _usedAbilityTrigger = false;
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if(pause && CurrentState != GameState.Ending)
        {
            Debug.Log("Lost Focus");
            MenuManager.Instance.PauseToggle(true);
        }

    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus && CurrentState != GameState.Ending)
        {
            Debug.Log("Lost Focus");
            MenuManager.Instance.PauseToggle(true);
        }

    }

    public void StartGame()
    {
        CurrentState = GameState.Ending;
        OnStartingEnter();
    }

    public void TweenBallDissolve(float dissolve, BallType ballType)
    {
        foreach(var ball in Balls)
        {
            if (ball.BallData.BallType == ballType && !ball.IsDissolved)
            {
                ball.TweenDissolve(dissolve, 0.5f);
            }
        }
    }


    public void ToggleAbility(int p)
    {
        CueBall.StopAiming();
        Player player = (Player)p;
        if (CurrentPlayer != player) return;

        if(CurrentAction == PlayerAction.Ability)
        {
            CurrentAction = PlayerAction.Shooting;
        }
        else if (CurrentAction == PlayerAction.Shooting)
        {
            if (AbM.IsAbilityReady(player))
            {
                CurrentAction = PlayerAction.Ability;
            }
        }
    }

    public void UsedAbility()
    {
        CueBall.StopAiming();
        _usedAbilityTrigger = true;
    }

    public void EnteredHole(BallData ballData)
    {
        BallsInThisTurn.Add(ballData);
        BallsInGame.Remove(ballData);

        if (P1BallType == BallType.None) // Assign Ball Type on first ball entered
        {
            if (ballData.BallType == BallType.Cue || ballData.BallType == BallType.Black) return; // Do not count cue and black balls for ball type.
            
            if (CurrentPlayer == Player.P1)
            {
                P1BallType = ballData.BallType;
                P2BallType = P1BallType == BallType.Solid ? BallType.Striped : BallType.Solid;
            }
            else
            {
                P2BallType = ballData.BallType;
                P1BallType = P2BallType == BallType.Solid ? BallType.Striped : BallType.Solid;
            }
            TypeSetThisTurn = true;
            Debug.Log($"P1 Type = {P1BallType}");
            Debug.Log($"P2 Type = {P2BallType}");
        }
    }

    public bool PauseToggle(bool state)
    {
        CueBall.StopAiming();
        if(state == true)
        {
            if(CurrentState == GameState.Player)
            {
                _pausedAction = CurrentAction;
                CurrentAction = PlayerAction.Pausing;
                return true;
            }
        }

        CurrentAction = _pausedAction;
        return false;
    }


    #region State Machine
    void OnStartingEnter()
    {
        //Ball Init:
        for(int i = 0; i < BallTransforms.Length; i++)
        {
            BallTransforms[i].position = _ballsStartPositions[i];
        }

        // Data Init:
        BallsInGame = new List<BallData>();
        foreach(var ball in Balls)
        {
            BallsInGame.Add(ball.BallData);
        }
        CanShoot = false;
        CanAbility = false;
        

        // Choose First Player to play.
        CurrentPlayer = Random.Range(0,2) == 0 ? Player.P1 : Player.P2;
        CurrentTurnType = TurnType.Normal;
        
        
        
        foreach(var b in Balls)
        {
            b.Appear();
        }
        CueBall.Appear();




        OnPlayerEnter();
    }

    private void OnPlayerEnter()
    {
        Debug.Log($"On Player Enter : {CurrentPlayer} - {CurrentTurnType}");
        CurrentAction = PlayerAction.Starting;
        StartCoroutine(OnPlayerEnterCoroutine());
    }
    private IEnumerator OnPlayerEnterCoroutine()
    {
        
        BallsInThisTurn = new List<BallData>();
        FirstContact = null;
        TypeSetThisTurn = false;
        
        AbM.ReduceCD(CurrentPlayer);
        if (CurrentTurnType != TurnType.Extra) AbM.ReduceDurations();
        UI.UpdateUI();


        Debug.Log($"P1 Ability CD: {AbM.ArcaneBarrierCurrentCD} \n P2 Ability CD: {AbM.ShadowShotCurrentCD}");
        yield return null;


        switch (CurrentTurnType)
        {
            case TurnType.Normal:
                Sequence seqNormal = CurrentPlayer == Player.P1 ? UI.P1BannerFlash() : UI.P2BannerFlash();
                yield return seqNormal.Play().WaitForCompletion();
                CurrentAction = PlayerAction.Shooting;
                break;
            case TurnType.Extra:
                Sequence seqExtra = CurrentPlayer == Player.P1 ? UI.P1ExtraBannerFlash() : UI.P2ExtraBannerFlash();
                yield return seqExtra.Play().WaitForCompletion(); 
                CurrentAction = PlayerAction.Shooting;
                break;
            case TurnType.Penalty:
                Sequence seqPenalty = CurrentPlayer == Player.P1 ? UI.P1PenaltyBannerFlash() : UI.P2PenaltyBannerFlash();
                yield return seqPenalty.Play().WaitForCompletion();
                CurrentAction = PlayerAction.Penalty;
                break;
        }

        CurrentState = GameState.Player;
    }

    private void OnPlayerUpdate()
    {
        switch (CurrentAction)
        {
            case PlayerAction.Penalty:
                CanShoot = false;
                CanAbility = false;
                PlaceBallPenalty();
                break;
            case PlayerAction.Shooting:
                CanShoot = true;
                CanAbility = false;
                break;
            case PlayerAction.Ability:
                CanShoot = false;
                CanAbility = true;
                break;
            case PlayerAction.Starting:
                CanShoot = false;
                CanAbility = false;
                break;
            case PlayerAction.Pausing:
                CanShoot = false;
                CanAbility = false;
                break;
        }


    }

    private void PlaceBallPenalty()
    {
        if(Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                Ray ray = MainCamera.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit hit;

                if(Physics.Raycast(ray, out hit))
                {

                    if (Mathf.Abs(hit.point.x) < PlacementArea.localScale.x / 2 &&
                        Mathf.Abs(hit.point.z) < PlacementArea.localScale.y / 2)
                    {
                        CueBall.Appear(new Vector3(hit.point.x, 0, hit.point.z));
                        CurrentAction = PlayerAction.Shooting;

                    }
                }
                
            }
        }
    }

    public void OnPlayerExit()
    {
        CurrentState = GameState.Wait;
        StartCoroutine(OnPlayerExitCoroutine());
    }

    private IEnumerator OnPlayerExitCoroutine()
    {
        CanShoot = false;
        yield return new WaitForSeconds(2f);
        while (AreBallsMoving())
        {
            yield return new WaitForSeconds(0.1f);
        }


  


        if (BallData.HasType(BallsInThisTurn,BallType.Black)) // 8 Ball in, game ends
        {
            BallType ballType = CurrentPlayer == Player.P1 ? P1BallType : P2BallType;
            bool isWinner = !BallData.HasType(BallsInGame, ballType); // Check if won or lost
            if (ballType == BallType.None) isWinner = false;
            OnEndingEnter(isWinner);
            yield break;
        }
        else if (CheckPenalty()) // First contact was not player's ball or entered white ball.
        {
            CurrentPlayer = CurrentPlayer == Player.P1 ? Player.P2 : Player.P1;
            CurrentTurnType = TurnType.Penalty;
        }
        else if (BallData.HasType(BallsInThisTurn, CurrentPlayerBallType)) // Scored a ball - Another turn.
        {
            CurrentTurnType = TurnType.Extra;
        }
        else // Nothing special happend
        {
            CurrentPlayer = CurrentPlayer == Player.P1 ? Player.P2 : Player.P1;
            CurrentTurnType = TurnType.Normal;
        }

        OnPlayerEnter();
    }

    private bool CheckPenalty()
    {
        BallType ballType = CurrentPlayerBallType;

        if (FirstContact == null) { Debug.Log("No Touch"); return true; } // Made no contact
        
        if(!TypeSetThisTurn && ballType != BallType.None)
        {
            if (FirstContact.BallType != ballType) // First Contact was not the player's ball type
            {
                // Check if not hit the black ball when it is the last ball.
                if (!(FirstContact.BallType == BallType.Black && !BallData.HasType(BallsInGame, ballType))) { Debug.Log("Hit Wrong Ball"); return true; }

            }
        }
        
        if (BallData.HasType(BallsInThisTurn, BallType.Cue)) { Debug.Log("Entered Cue Ball"); return true; };
        return false;
    }

    private void OnEndingEnter(bool isWinner)
    {
        StartCoroutine(OnEndingCoroutine(isWinner));
    }

    private IEnumerator OnEndingCoroutine(bool isWinner)
    {
        CurrentState = GameState.Ending;
        if (isWinner)
        {
            if(CurrentPlayer == Player.P1)
            {
                Debug.Log("P1 Wins");
                yield return UI.P1WinFlash().Play().WaitForCompletion();
            }
            else
            {
                Debug.Log("P2 Wins");
                yield return UI.P2WinFlash().Play().WaitForCompletion();
            }
        }
        else
        {
            if (CurrentPlayer == Player.P1)
            {
                Debug.Log("P2 Wins");
                yield return UI.P2WinFlash().Play().WaitForCompletion();
            }
            else
            {
                Debug.Log("P1 Wins");
                yield return UI.P1WinFlash().Play().WaitForCompletion();
            }
        }

    }
    #endregion
    private bool AreBallsMoving()
    {
            if (CueBall.IsMoving)
            {
                return true;
            }
            else foreach (var b in Balls)
                {
                    if (b.IsMoving)
                    {
                        return true;
                    }
                }

        return false;

    }



}

#region Enums
public enum TurnType
{
    Normal,
    Extra,
    Penalty,
}
public enum Player
{
    P1,
    P2,
}
public enum PlayerAction
{
    Penalty,
    Shooting,
    Ability,
    Starting,
    Pausing,
}
public enum GameState
{
    Starting,
    Player,
    Ending,
    Wait,
}
#endregion