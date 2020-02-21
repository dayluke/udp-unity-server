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
    }

    public void OnJoinClicked()
    {
        SendData(String.Format("PlayerJoined -> {0} -> {1}", name, PlayerType.MARINE));
        // Server needs to send a response to say that the client joined successfully.
        panel.SetActive(false);
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

    private void SendData(string dataToSend)
    {
        Byte[] sendBytes = Encoding.ASCII.GetBytes(dataToSend);
        udp.Send(sendBytes, sendBytes.Length);

        string[] info = dataToSend.Split(new string[]{" -> "}, StringSplitOptions.None);
        string command = info[0];

        if (debug) Debug.LogFormat("Client invoked '{0}' command on server", command);
    }

    protected override void Close()
    {
        if (debug) Debug.Log("Client closed");
        udp.Close();
    }
}