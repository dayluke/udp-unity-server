using System;
using System.Net;

[Serializable]
public class Player
{
    public string username;
    public PlayerType playerType;
    public IPEndPoint ipAddress;

    public Player(string name, PlayerType playerSpecies, IPEndPoint iPEndPoint)
    {
        username = name;
        playerType = playerSpecies;
        ipAddress = iPEndPoint;
    }
}

public enum PlayerType
{
    ALIEN,
    MARINE
}