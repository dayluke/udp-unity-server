using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BaseUdp : MonoBehaviour
{
    public const int listenPort = 11000;
    public bool debug = false;
    protected UdpClient udp;
    protected Dictionary<string, Action<object[]>> callableFunctions = new Dictionary<string, Action<object[]>>();

    protected abstract void Start();
    protected virtual void Update()
    {
        ReceiveData();
    }
    protected abstract void Close();

    protected void ReceiveData()
    {
        Task.Run
        (
            async () =>
            {
                try
                {
                    var result = await udp.ReceiveAsync();
                    string data = Encoding.ASCII.GetString(result.Buffer);
                    if (debug) Debug.LogFormat("User received broadcast from: {0}:{1}", result.RemoteEndPoint.Address.ToString(), result.RemoteEndPoint.Port.ToString());

                    object[] info = data.Split(new string[]{" -> "}, StringSplitOptions.None);
                    string command = info[0].ToString();
                    
                    List<object> t = info.ToList();
                    t.RemoveAt(0);

                    // Add the players ip address to the Player information so that the server can send messages back to the client.
                    if (command == "PlayerJoined")
                    {
                        t.Add(result.RemoteEndPoint.Address);
                        t.Add(result.RemoteEndPoint.Port);
                    }
                    
                    info = t.ToArray();

                    if (debug) 
                    {
                        Debug.LogFormat("    Command: {0}", command);
                        Debug.LogFormat("    Parameters: [{0}]", string.Join(", ", info));
                        Debug.LogFormat("    Info Length: {0}", info.Length);
                    }

                    callableFunctions[command](info);
                }
                catch (SocketException e)
                {
                    Debug.Log(e);
                }
            }
        );
    }

    protected void SendData(string dataToSend)
    {
        Byte[] sendBytes = Encoding.ASCII.GetBytes(dataToSend);
        udp.Send(sendBytes, sendBytes.Length);

        string[] info = dataToSend.Split(new string[]{" -> "}, StringSplitOptions.None);
        string command = info[0];

        if (debug) Debug.LogFormat("User invoked '{0}' command on server", command);
    }
}