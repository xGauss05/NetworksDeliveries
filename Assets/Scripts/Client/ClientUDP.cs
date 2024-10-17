using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;
using TMPro;

public class ClientUDP : MonoBehaviour
{
    Socket socket;
    public GameObject UItextObj;
    TextMeshProUGUI UItext;
    string clientText;

    public TMP_InputField usernameIF;
    public TMP_InputField ipIF;
    public TMP_InputField chatIF;

    public GameObject usernameTitle;
    public GameObject ipTitle;
    public GameObject sendBtn;

    string playerName;
    string IPtoConnect;

    // Start is called before the first frame update
    void Start()
    {
        UItext = UItextObj.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        UItext.text = clientText;
    }

    public void StartClient()
    {
        DisableInputs();

        Thread mainThread = new Thread(Connect);
        mainThread.Start();
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
        clientText = "Attempting to connect to server at IP: " + IPtoConnect + " ...";
        int port = 9050;
        IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse(IPtoConnect), port);

        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        // Initial information to send
        Send(serverEndpoint, playerName);

        // Start a thread to listen for incoming messages from the server
        Thread receiveThread = new Thread(() => Receive(serverEndpoint));
        receiveThread.Start();
    }

    void Send(IPEndPoint serverEndpoint, string message)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);
        try
        {
            socket.SendTo(data, data.Length, SocketFlags.None, serverEndpoint);
        }
        catch (SocketException ex)
        {
            clientText += "\nFailed to send message: " + ex.Message;
        }
    }

    public void SendChat()
    {
        if (!chatIF.text.Equals(""))
        {
            IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse(IPtoConnect), 9050);
            Send(serverEndpoint, chatIF.text);
            chatIF.text = "";
        }
    }

    void Receive(IPEndPoint serverEndpoint)
    {
        byte[] data = new byte[1024];
        EndPoint remoteEndpoint = (EndPoint)serverEndpoint;

        while (true)
        {
            try
            {
                int recv = socket.ReceiveFrom(data, ref remoteEndpoint);
                string receivedMessage = Encoding.ASCII.GetString(data, 0, recv);
                clientText += "\n" + receivedMessage;
            }
            catch (SocketException ex)
            {
                clientText += "\nError receiving data: " + ex.Message;
                break;
            }
        }
    }

    void DisableInputs()
    {
        usernameIF.gameObject.SetActive(false);
        ipIF.gameObject.SetActive(false);
        chatIF.gameObject.SetActive(true);

        usernameTitle.SetActive(false);
        ipTitle.SetActive(false);
        sendBtn.SetActive(true);
    }

    void ResetInputs()
    {
        usernameIF.gameObject.SetActive(true);
        ipIF.gameObject.SetActive(true);
        chatIF.gameObject.SetActive(false);

        usernameTitle.SetActive(true);
        ipTitle.SetActive(true);
        sendBtn.SetActive(false);
    }
}

