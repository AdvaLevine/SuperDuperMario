// Purpose: This script is used to control the Shrek character in the game. It inherits from the PlayerController script.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrekController : PlayerController
{
    private int playerID = 2;
    protected override void Update()
    {
        int totalPlayers = GameManager.Instance.NumberOfPlayers;
        UpdatePlayer(playerID, totalPlayers); 
    }
}