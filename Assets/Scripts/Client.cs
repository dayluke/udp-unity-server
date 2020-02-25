using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Client : BaseUdp
{
    public new string name = "test_name";
    public PlayerType playerType = PlayerType.MARINE;
    private const string ipAddress = "127.0.0.1";

    private void Start()
    {
        udp = new UdpClient();
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), listenPort);
        udp.Connect(ipEndPoint);

        if (debug) Debug.LogFormat("Client connected to: {0}:{1}", ipEndPoint.Address.ToString(), ipEndPoint.Port.ToString());

        callableFunctions["JoinLobbyFailed"] = x => JoinLobbyFailed();
        callableFunctions["JoinLobbySuccess"] = x => JoinLobbySuccess();
        callableFunctions["ChangeScene"] = x => ChangeScene(x);

        JoinLobby();
    }

    public override void JoinLobby()
    {
        SendData(String.Format("PlayerJoined -> {0} -> {1}", name, PlayerType.MARINE));
    }

    protected override void Close()
    {
        SendData(String.Format("PlayerLeft -> {0}", name));
        if (debug) Debug.Log("Client closed");
        udp.Close();
    }

    private void JoinLobbySuccess()
    {
        if (debug) Debug.Log("Successfully joined room");
    }

    private void JoinLobbyFailed()
    {
        Debug.LogFormat("JOINING LOBBY FAILED!");
    }

    private void ChangeScene(object[] args)
    {
        string sceneName = args[0].ToString();
        Debug.Log("Scene Name: " + sceneName);
        Debug.Log("ARG [0]: " + args[0] + ", TYPE: " + args[0].GetType());
        if (debug) Debug.Log("Changing scene to: " + args[0].ToString());
        if (debug) Debug.Log("Changing scene to: " + args[0]);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }
}