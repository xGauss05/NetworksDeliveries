using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;
using System;

public class ClientTCP : MonoBehaviour
{
    public GameObject UItextObj;
    TextMeshProUGUI UItext;
    string clientText;
    Socket server;

    public TMP_InputField usernameIF;
    public TMP_InputField ipIF;
    public TMP_InputField chatIF;

    string playerName;
    string IPtoConnect;

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
        DisableInputs();

        Thread connect = new Thread(Connect);
        connect.Start();
    }

    public void ChangeName()
    {
        playerName = usernameIF.text;
        clientText = "Your name is now: " + playerName;
    }

    public void ChangeIP()
    {
        IPtoConnect = ipIF.text;
    }

    void Connect()
    {
        clientText = "Attempting connect to IP: " + IPtoConnect + " ...";

        int port = 9050;
        IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(IPtoConnect), port);

        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            server.Connect(ipep);
            clientText += "\nConnected to server at " + IPtoConnect + ":" + port;

            Thread sendThread = new Thread(Send);
            sendThread.Start();

            Thread receiveThread = new Thread(Receive);
            receiveThread.Start();
        }
        catch (SocketException ex)
        {
            clientText += "\nConnection failed: " + ex.Message; 
        }
    }

    void Send()
    {
        server.Send(Encoding.ASCII.GetBytes(playerName));
    }

    public void SendChat()
    {
        if (!chatIF.text.Equals(""))
        {
            server.Send(Encoding.ASCII.GetBytes(playerName + " says: " + chatIF.text));
            chatIF.text = "";
        }
    }

    void Receive()
    {
        byte[] data = new byte[1024];
        int recv = 0;

        while (true)
        {
            recv = server.Receive(data);
            if (recv == 0)
            {
                ResetInputs();
                break;
            }
            else
            {
                clientText = clientText + "\n" + Encoding.ASCII.GetString(data, 0, recv);
            }
        }
    }

    void DisableInputs()
    {
        usernameIF.gameObject.SetActive(false);
        ipIF.gameObject.SetActive(false);
        chatIF.gameObject.SetActive(true);
    }

    void ResetInputs()
    {
        usernameIF.gameObject.SetActive(true);
        ipIF.gameObject.SetActive(true);
        chatIF.gameObject.SetActive(false);
    }

}
