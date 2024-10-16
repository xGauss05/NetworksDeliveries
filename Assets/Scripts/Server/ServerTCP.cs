using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using System.Text;
using System.Collections.Generic;

public class ServerTCP : MonoBehaviour
{
    Socket socket;
    IPEndPoint ipep;
    Thread mainThread = null;

    public GameObject UItextObj;
    TextMeshProUGUI UItext;
    string serverText;

    public struct User
    {
        public string name;
        public Socket socket;
    }

    private List<User> userList = new List<User>();

    void Start()
    {
        UItext = UItextObj.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        UItext.text = serverText;
    }

    public void startServer()
    {
        serverText = "Starting TCP Server...";

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        int port = 9050;
        ipep = new IPEndPoint(IPAddress.Any, port);

        try
        {
            socket.Bind(ipep);
            serverText += "\nSocket successfully bound to port 9050. IPEP: " + ipep.ToString();
        }
        catch (SocketException ex)
        {
            serverText += "\nSocket binding failed: " + ex.Message;
            return; // Exit if binding failed
        }

        socket.Listen(10);

        mainThread = new Thread(CheckNewConnections);
        mainThread.Start();

    }

    void CheckNewConnections()
    {
        while (true)
        {
            User newUser = new User();
            newUser.name = "";

            try
            {
                newUser.socket = socket.Accept(); // Accept the socket connection
                serverText += "\nNew client connected from: " + newUser.socket.RemoteEndPoint.ToString();

                // Add the new user to the list of connected users
                userList.Add(newUser);

                // Send a success message to the client
                newUser.socket.Send(Encoding.ASCII.GetBytes("Successfully connected to server at " + ipep.ToString()));

                // Start a thread to handle receiving messages from this client
                Thread newConnection = new Thread(() => Receive(newUser));
                newConnection.Start();
            }
            catch (SocketException ex)
            {
                serverText += "\nSocket accept failed: " + ex.Message;
                return; // Exit if binding failed
            }

            //IPEndPoint clientep = (IPEndPoint)socket.RemoteEndPoint;
            //serverText = serverText + "\n" + "Connected with " + clientep.Address.ToString() + " at port " + clientep.Port.ToString();
        }
    }

    void Receive(User user)
    {
        byte[] data = new byte[1024];
        int recv = 0;
        recv = user.socket.Receive(data);
        if (recv > 0)
        {
            user.name = Encoding.ASCII.GetString(data, 0, recv);
            serverText += "\nPlayer connected: " + user.name;
        }

        while (true)
        {
            try
            {
                recv = user.socket.Receive(data);
                if (recv == 0)
                {
                    serverText += "\n" + user.name + " disconnected.";
                    userList.Remove(user);
                    user.socket.Close();
                    break;
                }
                else
                {
                    string receivedMessage = Encoding.ASCII.GetString(data, 0, recv);
                    serverText += "\n" + receivedMessage;

                    foreach (User u in userList)
                        Send(u, receivedMessage);
                }
            }
            catch (SocketException ex)
            {
                serverText += "\nError receiving data: " + ex.Message;
                userList.Remove(user);
                user.socket.Close();
                break;
            }
        }
    }

    void Send(User user, string message)
    {
        byte[] messageBytes = Encoding.ASCII.GetBytes(message);
        try
        {
            user.socket.Send(messageBytes);
        }
        catch (SocketException ex)
        {
            serverText += "\nError sending data to client: " + ex.Message;
        }
    }
}
