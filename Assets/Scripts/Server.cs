using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;
using UnityEngine;
using System.Net;
using System.Text;
using UnityEngine.SceneManagement;

public class Server : BaseUdp
{
    public int maxPlayerCount = 1;
    public List<Player> players = new List<Player>();
    public GameObject panel;

    // TODO:
    // Associate server with client. When server is initialised, join server's client.
    // Have a response message from the server to tell the client that it successfully joined.
    // When server clicks change scene, call this on all clients.

    public override void JoinLobby()
    {
        panel.SetActive(false);
        try
        {
            udp = new UdpClient(listenPort);
            callableFunctions["Print"] = x => Print(x.ToString());
            callableFunctions["PlayerJoined"] = x => PlayerJoined(x);
            callableFunctions["PlayerLeft"] = x => PlayerLeft(x);
            if (debug) Debug.Log("Server initialised");
        }
        catch (SocketException e)
        {
            Debug.Log(e);
            Debug.Log("There is already a server running on this port. Swapping to a client.");
            gameObject.AddComponent(typeof(Client));
            Destroy(this);
        }
    }

    private void PlayerJoined(object[] args)
    {
        string playerName = args[0].ToString();
        PlayerType playerType;
        Enum.TryParse(args[1].ToString(), out playerType);
        string ip = args[2].ToString();
        int port = (int) args[3];

        if (players.Count >= maxPlayerCount)
        {
            Debug.LogFormat("Lobby full! '{0}' cannot join.", playerName);
            byte[] fail = Encoding.ASCII.GetBytes("JoinLobbyFailed");
            udp.Send(fail, fail.Length, new IPEndPoint(IPAddress.Parse(ip), port));
            return;
        }

        players.Add(new Player(playerName, playerType, ip, port));

        // Send join succeeded to client.
        byte[] success = Encoding.ASCII.GetBytes("JoinLobbySuccess");
        udp.Send(success, success.Length, new IPEndPoint(IPAddress.Parse(ip), port));
            

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

    public void ChangeScene(string sceneToChangeTo)
    {
        string command = "ChangeScene";
        string dataToSend = String.Format("{0} -> {1}", command, sceneToChangeTo);
        
        if (debug) Debug.Log("Sending to all clients: " + dataToSend);
        SendToAllClients(dataToSend);

        SceneManager.LoadScene(sceneToChangeTo);
    }

    private void SendToAllClients(string message)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);
        foreach (Player player in players)
        {
            udp.Send(data, data.Length, new IPEndPoint(IPAddress.Parse(player.address), player.port));
        }
    }
}