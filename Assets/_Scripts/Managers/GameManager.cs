using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class GameManager : StaticInstance<GameManager>
{
    public BallController CueBall;
    public BallHandler[] Balls;
    public RectTransform PlacementArea;
    public Camera MainCamera;



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
    public bool IsPausing { get; private set; }

    // Turn Data
    public PlayerAction CurrentAction { get; private set; }
    public BallData FirstContact { get; set; }
    public List<BallData> BallsInThisTurn { get; private set; }
    private bool _checkBallsMoving = false;
    private bool _ballsMoving = false;


    private BallType CurrentPlayerBallType { get { return CurrentPlayer == Player.P1 ? P1BallType : P2BallType; } }


    private void Start()
    {
        IsPausing = false;
        CurrentState = GameState.Ending;
        OnStartingEnter();
    }
 

    // Update is called once per frame
    void Update()
    {
        if(CurrentState == GameState.Wait)
            _ballsMoving = AreBallsMoving();

        if (CurrentState == GameState.Player && !IsPausing)
        {
            OnPlayerUpdate();
        }
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

            Debug.Log($"P1 Type = {P1BallType}");
            Debug.Log($"P2 Type = {P2BallType}");
        }
    }

    public void PauseToggle()
    {
        IsPausing = !IsPausing;
        if (IsPausing)
        {

        }   
    }


    #region State Machine
    void OnStartingEnter()
    {

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


    void OnPlayerEnter()
    {
        Debug.Log($"On Player Enter : {CurrentPlayer} - {CurrentTurnType}");
        
        BallsInThisTurn = new List<BallData>();
        FirstContact = null;

        switch (CurrentTurnType)
        {
            case TurnType.Normal:
            case TurnType.Extra:
                CurrentAction = PlayerAction.Shooting;
                break;
            case TurnType.Penalty:
                CurrentAction = PlayerAction.Penalty;
                break;
        }

        CurrentState = GameState.Player;
    }

    void OnPlayerUpdate()
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
        }


    }

    void PlaceBallPenalty()
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
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => _ballsMoving == false);



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
        else if (BallsInThisTurn.Count > 0) // Scored a ball - Another turn.
        {
            CurrentTurnType = TurnType.Extra;
        }
        else // Nothing special happend
        {
            CurrentPlayer = CurrentPlayer == Player.P1 ? Player.P2 : Player.P1;
            CurrentTurnType = TurnType.Normal;
        }

        /* Call UI Manager to Show banner for turn switch ---
        *
        *
        *
        *
        *//////////////////////////////////////////////////////

        OnPlayerEnter();
    }

    private bool CheckPenalty()
    {
        BallType ballType = CurrentPlayer == Player.P1 ? P1BallType : P2BallType;
        if (FirstContact == null) return true;
        if (FirstContact.BallType == BallType.Black && 
            BallData.HasType(BallsInGame, CurrentPlayerBallType)) return true;
        if (FirstContact.BallType != ballType && ballType != BallType.None) return true;
        if (BallData.HasType(BallsInThisTurn, BallType.Cue)) return true;
        return false;
    }

    void OnEndingEnter(bool isWinner)
    {
        CurrentState = GameState.Ending;
        if (isWinner)
        {
            if(CurrentPlayer == Player.P1)
            {
                Debug.Log("P1 Wins");
            }
            else
            {
                Debug.Log("P2 Wins");
            }
        }
        else
        {
            if (CurrentPlayer == Player.P1)
            {
                Debug.Log("P2 Wins");
            }
            else
            {
                Debug.Log("P1 Wins");
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
}
public enum GameState
{
    Starting,
    Player,
    Ending,
    Pause,
    Wait,
}
#endregion