using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;
using System.Collections.Generic;

public class ServerUDP : MonoBehaviour
{
    Socket socket;
    IPEndPoint ipep;
    Thread mainThread = null;

    public GameObject UItextObj;
    TextMeshProUGUI UItext;
    string serverText;

    private Dictionary<EndPoint, string> userList = new Dictionary<EndPoint, string>();

    void Start()
    {
        UItext = UItextObj.GetComponent<TextMeshProUGUI>();
        startServer();
    }

    public void startServer()
    {
        serverText = "Starting UDP Server...";
        Debug.Log("Starting UDP Server...");
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        int port = 9050;
        ipep = new IPEndPoint(IPAddress.Any, port);

        try
        {
            socket.Bind(ipep);
            serverText += "\nSocket successfully bound to port 9050.";
            Debug.Log("Socket successfully bound to port 9050.");
        }
        catch (SocketException ex)
        {
            serverText += "\nSocket binding failed: " + ex.Message;
            Debug.Log("Socket binding failed: " + ex.Message);
            return; // Exit if binding failed
        }

        mainThread = new Thread(Receive);
        mainThread.Start();
    }

    void Update()
    {
        UItext.text = serverText;
    }

    void Receive()
    {
        byte[] data = new byte[1024];
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint remoteEP = (EndPoint)(sender);

        serverText += "\nWaiting for clients...";

        while (true)
        {
            try
            {
                int recv = socket.ReceiveFrom(data, ref remoteEP);
                string message = Encoding.ASCII.GetString(data, 0, recv);

                if (!userList.ContainsKey(remoteEP))
                {
                    // Store the player's name on the first message
                    userList[remoteEP] = message;
                    serverText += $"\nPlayer connected: " + message + " from " + remoteEP;
                    Debug.Log("Player connected: " + message + " from " + remoteEP);
                    SendToClient(remoteEP, "You have joined the lobby, " + message);
                }
                else
                {
                    string playerName = userList[remoteEP];
                    serverText += "\nMessage received from " + userList[remoteEP] + ": " + message;
                    Debug.Log("Message received from " + userList[remoteEP] + ": " + message);
                    SendToAllClients(playerName + ": " + message);
                }
            }
            catch (SocketException ex)
            {
                serverText += "\nError receiving data: " + ex.Message;
                Debug.Log("Error receiving data: " + ex.Message);
            }
        }
    }

    void SendToClient(EndPoint Remote, string responseMessage)
    {
        byte[] data = Encoding.ASCII.GetBytes(responseMessage);
        try
        {
            socket.SendTo(data, data.Length, SocketFlags.None, Remote);
        }
        catch (SocketException ex)
        {
            serverText += "\nError sending data: " + ex.Message;
            Debug.Log("Error sending data: " + ex.Message);
        }
    }

    void SendToAllClients(string message)
    {
        foreach (var user in userList.Keys)
        {
            SendToClient(user, message);
        }
    }
}
