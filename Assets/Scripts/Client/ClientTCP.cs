﻿using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;

public class ClientTCP : MonoBehaviour
{
    public GameObject UItextObj;
    TextMeshProUGUI UItext;
    string clientText;
    Socket server;

    // Start is called before the first frame update
    void Start()
    {
        UItext = UItextObj.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        UItext.text = clientText;
    }

    public void StartClient()
    {
        Thread connect = new Thread(Connect);
        connect.Start();
    }

    void Connect()
    {
        //TO DO 2
        //Create the server endpoint so we can try to connect to it.
        //You'll need the server's IP and the port we binded it to before
        //Also, initialize our server socket.
        //When calling connect and succeeding, our server socket will create a
        //connection between this endpoint and the server's endpoint

        int port = 9050;
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("192.168.206.12"), port);

        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        server.Connect(ipep);

        //TO DO 4
        //With an established connection, we want to send a message so the server acknowledges us
        //Start the Send Thread
        Thread sendThread = new Thread(Send);
        sendThread.Start();

        //TO DO 7
        //If the client wants to receive messages, it will have to start another thread. Call Receive()
        Thread receiveThread = new Thread(Receive);
        receiveThread.Start();

    }
    void Send()
    {
        //TO DO 4
        //Using the socket that stores the connection between the 2 endpoints, call the TCP send function with
        //an encoded message

        server.Send(Encoding.ASCII.GetBytes("Message sent!"));

    }

    //TO DO 7
    //Similar to what we already did with the server, we have to call the Receive() method from the socket.
    void Receive()
    {
        byte[] data = new byte[1024];
        int recv = 0;

        server.Receive(data);
        recv = data.Length;

        clientText = clientText += "\n" + Encoding.ASCII.GetString(data, 0, recv);
    }

}
