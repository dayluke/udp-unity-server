using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class Client : BaseUdp
{
    public new string name = "test_name";
    public PlayerType playerType = PlayerType.MARINE;
    private const string ipAddress = "127.0.0.1";
    public GameObject panel;

    protected override void Start()
    {
        udp = new UdpClient();
        IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), listenPort);
        udp.Connect(ipEndPoint);

        if (debug) Debug.LogFormat("Client connected to: {0}:{1}", ipEndPoint.Address.ToString(), ipEndPoint.Port.ToString());

        callableFunctions["JoinLobbyFailed"] = x => JoinLobbyFailed();
        callableFunctions["JoinLobbySuccess"] = x => JoinLobbySuccess();
    }

    public void OnJoinClicked()
    {
        SendData(String.Format("PlayerJoined -> {0} -> {1}", name, PlayerType.MARINE));
        // When server changes scene, change scene on all clients.
    }

    public void OnLeaveClicked()
    {
        SendData(String.Format("PlayerLeft -> {0}", name));
        Close();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit ();
        #endif
    }

    protected override void Close()
    {
        if (debug) Debug.Log("Client closed");
        udp.Close();
    }

    private void JoinLobbySuccess()
    {
        if (debug) Debug.Log("Successfully joined room");
        panel.SetActive(false);
    }

    private void JoinLobbyFailed()
    {
        Debug.LogFormat("JOINING LOBBY FAILED!");
    }
}