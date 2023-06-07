using System.Collections.Generic;
using UnityEngine;

public class GameManager : StaticInstance<GameManager>
{
    public BallController CueBall;
    public BallHandler[] Balls;




    // Enablers
    public bool CanShoot { get; set; }

    // Game Analytics
    public List<BallData> BallsInThisGame { get; private set; }


    // Turn Analytics
    public BallData FirstContact { get; private set; }
    public List<BallData> BallsInThisTurn { get; private set; }

    private void Start()
    {
        CanShoot = true;
    }
 

    // Update is called once per frame
    void Update()
    {
        
    }
}

enum GameState
{
    Starting,
    PlayerOne,
    PlayerTwo,
    Ending,
}
