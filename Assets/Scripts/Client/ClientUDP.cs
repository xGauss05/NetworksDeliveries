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

    bool toggleInputs = false;

    // Start is called before the first frame update
    void Start()
    {
        UItext = UItextObj.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        UItext.text = clientText;

        if (toggleInputs)
        {
            toggleInputs = false;
            ToggleInputs(false);
        }
    }

    public void StartClient()
    {
        if (!string.IsNullOrEmpty(IPtoConnect) && !string.IsNullOrEmpty(playerName))
        {
            ToggleInputs(true);

            Thread mainThread = new Thread(Connect);
            mainThread.Start();
        }
        else
        {
            clientText += "\nPlease fill the required data.";
        }
    }

    public void ChangeName()
    {
        playerName = usernameIF.text;
        clientText += "\nYour name is now: " + playerName;
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

        try
        {
            Send(serverEndpoint, playerName);
        }
        catch (SocketException ex)
        {
            clientText += "\nConnection failed: " + ex.Message;
            toggleInputs = true;
            return;
        }

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
            toggleInputs = true;
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
                toggleInputs = true;
                break;
            }
        }
    }

    void ToggleInputs(bool toggle)
    {
        usernameIF.gameObject.SetActive(!toggle);
        ipIF.gameObject.SetActive(!toggle);
        chatIF.gameObject.SetActive(toggle);

        usernameTitle.SetActive(!toggle);
        ipTitle.SetActive(!toggle);
        sendBtn.SetActive(toggle);
    }

}
