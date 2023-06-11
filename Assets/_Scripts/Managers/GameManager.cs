using System.Collections.Generic;
using UnityEngine;

public class GameManager : StaticInstance<GameManager>
{
    public BallController CueBall;
    public BallHandler[] Balls;




    // Enablers
    public bool CanShoot { get; set; }

    // Game Data
    public List<BallData> BallsInThisGame { get; private set; }
    public GameState CurrentState { get; private set; }
    public Player CurrentPlayer { get; private set; }
    public TurnType CurrentTurnType { get; private set; }
    public BallType P1BallType { get; private set; }
    public BallType P2BallType { get; private set; }

    // Turn Data
    public BallData FirstContact { get; set; }
    public List<BallData> BallsInThisTurn { get; private set; }

    

    private void Start()
    {
        CurrentState = GameState.Starting;
        OnStartingEnter();
    }
 

    // Update is called once per frame
    void Update()
    {
        if(CurrentState == GameState.Player) 
        {
            OnPlayerUpdate();
        }
    }


    public void EnteredHole(BallData ballData)
    {

        BallsInThisTurn.Add(ballData);
        BallsInThisGame.Add(ballData);
    }


    void OnStartingEnter()
    {
        // Data Init:
        BallsInThisGame = new List<BallData>();



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
    }

    void OnPlayerUpdate()
    {
        CanShoot = true;
    }

    void OnPlayerExit()
    {

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

        OnPlayerEnter();
    }

    void OnEndingEnter(bool isWinner)
    {

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
