using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;
using UnityEngine;
using System.Net;

public class Server : BaseUdp
{
    public Client serversClient;
    public List<Player> players = new List<Player>();


    // TODO:
    // Associate server with client. When server is initialised, join server's client.
    // Have a response message from the server to tell the client that it successfully joined.
    // When server clicks change scene, call this on all clients.

    protected override void Start()
    {
        try
        {
            udp = new UdpClient(listenPort);
            callableFunctions["Print"] = x => Print(x.ToString());
            callableFunctions["PlayerJoined"] = x => PlayerJoined(x);
            callableFunctions["PlayerLeft"] = x => PlayerLeft(x);
            if (debug) Debug.Log("Server initialised");
            
            // serversClient.OnJoinClicked();
        }
        catch (SocketException e)
        {
            Debug.Log(e);
            Debug.Log("There is already a server running on this port. Deleting this gameobject.");
            Destroy(gameObject);
        }
    }

    private void PlayerJoined(object[] args)
    {
        string playerName = args[0].ToString();
        PlayerType playerType;
        Enum.TryParse(args[1].ToString(), out playerType);
        IPEndPoint ip = new IPEndPoint((IPAddress.Parse(args[2].ToString()));

        players.Add(new Player(playerName, playerType, ip));

        if (debug) Debug.LogFormat("{0} joined the lobby as an {1}", playerName, playerType);
        if (debug) Debug.LogFormat("There are now {0} players in the lobby.", players.Count);
    }

    private void PlayerLeft(object[] args)
    {
        string playerName = args[0].ToString();
        players.Remove(players.Single(player => player.username == playerName));
        
        if (debug) Debug.LogFormat("Player {0} has left the lobby. There are now {1} players in the lobby.", playerName, players.Count);
    }

    private void Print(string str)
    {
        Debug.LogFormat("Server received: {0}", str);
    }

    protected override void Close()
    {
        if (debug) Debug.Log("Server closed");
        udp.Close();
    }
}