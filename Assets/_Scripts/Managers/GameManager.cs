using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : StaticInstance<GameManager>
{
    public BallController CueBall;
    public BallHandler[] Balls;




    // Enablers
    public bool CanShoot { get; set; }
    public bool CanAbility { get; set; }

    // Game Data
    public List<BallData> BallsInThisGame { get; private set; }
    public GameState CurrentState { get; private set; }
    public Player CurrentPlayer { get; private set; }
    public TurnType CurrentTurnType { get; private set; }
    public BallType P1BallType { get; private set; }
    public BallType P2BallType { get; private set; }
    public bool IsPausing { get; private set; }

    // Turn Data
    public BallData FirstContact { get; set; }
    public List<BallData> BallsInThisTurn { get; private set; }

    private bool _ballsMoving = false;

    private void Start()
    {
        IsPausing = false;
        CurrentState = GameState.Starting;
        OnStartingEnter();
    }
 

    // Update is called once per frame
    void Update()
    {
        _ballsMoving = AreBallsMoving();

        if(CurrentState == GameState.Player && !IsPausing) 
        {
            OnPlayerUpdate();
        }
    }


    public void EnteredHole(BallData ballData)
    {

        BallsInThisTurn.Add(ballData);
        BallsInThisGame.Add(ballData);
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
        BallsInThisGame = new List<BallData>();
        CanShoot = false;
        CanAbility = false;


        // Choose First Player to play.
        CurrentPlayer = Random.Range(0,1) == 0 ? Player.P1 : Player.P2;
        
        
        
        
        
        foreach(var b in Balls)
        {
            b.Appear();
        }
        CueBall.Appear();




        OnPlayerEnter();
    }


    void OnPlayerEnter()
    {
        // Data Init:
        BallsInThisTurn = new List<BallData>();
        FirstContact = null;


        CurrentState = GameState.Player;
        CanShoot = true;
    }

    void OnPlayerUpdate()
    {
        // Shooting - CanShoot = true
        // Ability - CanAbility = true
        // Penalty - Place Ball = true



    }


    public void OnPlayerExit()
    {
        StartCoroutine(OnPlayerExitCoroutine());
    }

    private IEnumerator OnPlayerExitCoroutine()
    {
        CanShoot = false;
        yield return new WaitForSeconds(1f);
        yield return new WaitUntil(() => _ballsMoving == false);
        CanShoot = true;

        if (false) // 8 Ball in, game ends
        {
            bool isWinner = false; // Check if won or lost
            OnEndingEnter(true);
        }
        else if (false) // Cue Ball in, penalty for opponent.
        {
            CurrentPlayer = CurrentPlayer == Player.P1 ? Player.P2 : Player.P1;
            CurrentTurnType = TurnType.PenaltyEx;
        }
        else if (false) // First contact was not player's ball.
        {
            CurrentPlayer = CurrentPlayer == Player.P1 ? Player.P2 : Player.P1;
            CurrentTurnType = TurnType.Penalty;
        }
        else if (false) // Scored a ball - Another turn.
        {
            CurrentTurnType = TurnType.Extra;
        }
        else // Nothing special happend
        {
            CurrentPlayer = CurrentPlayer == Player.P1 ? Player.P2 : Player.P1;
            CurrentTurnType = TurnType.Normal;
        }

        //OnPlayerEnter();
    }

    void OnEndingEnter(bool isWinner)
    {

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


public enum TurnType
{
    Normal,
    Extra,
    Penalty,
    PenaltyEx,
}

public enum Player
{
    P1,
    P2,
}

public enum GameState
{
    Starting,
    Player,
    Ending,
}
