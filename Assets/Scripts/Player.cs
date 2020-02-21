using System;
using System.Net;

[Serializable]
public class Player
{
    public string username;
    public PlayerType playerType;
    public string address;
    public int port;

    public Player(string name, PlayerType playerSpecies, string addr, int portNum)
    {
        username = name;
        playerType = playerSpecies;
        address = addr;
        port = portNum;
    }
}

public enum PlayerType
{
    ALIEN,
    MARINE
}