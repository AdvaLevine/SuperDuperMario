
public class MarioController : PlayerController
{
    private int playerID = 1;
    
    protected override void Update()
    {
        int totalPlayers = GameManager.Instance.NumberOfPlayers;
        UpdatePlayer(playerID, totalPlayers); 
    }
}
