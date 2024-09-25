using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioController : PlayerController
{
    private int playerID = 1;
    
    protected override void Update()
    {
        UpdatePlayer(playerID); 
    }
}
