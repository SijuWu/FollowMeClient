using System.Collections;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using UnityEngine;

public class ServerSocket : MonoBehaviour
{
	const int handStructSize = 57;
	const int fingerStructSize = 41;
	const int touchStructSize = 25;
	
	//Server
	Socket serverSocket;
	//Client
	Socket clientSocket;
	//Thread of connection
	Thread connectThread;
	//IP of the client
	IPEndPoint clientIP;
	string returnStr;
	string receiveStr;
	byte[] receiveBytes;
	string sendStr;
	int receiveDataLength;
	byte[] receiveData = new byte[1024];
	byte[] sendData = new byte[1024];
	bool running;

	public void Init ()
	{
		returnStr = null;
		receiveStr = null;
		
		//Get ip
		string hostName = System.Net.Dns.GetHostName ();
		System.Net.IPHostEntry ipEntry = System.Net.Dns.GetHostEntry (hostName);
		//IP address list
		System.Net.IPAddress[] addr = ipEntry.AddressList;
		
		IPEndPoint ipEndPoint = new IPEndPoint (addr [0], 8000);
		serverSocket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		serverSocket.Bind (ipEndPoint);
		serverSocket.Listen (10);
		
		running = true;
		
		//Create new thread
		connectThread = new Thread (new ThreadStart (GoClient));
		connectThread.Start ();		
	}
	
	void GoClient ()
	{
//		ConnectClient ();
		if (clientSocket == null)
			clientSocket = serverSocket.Accept ();
		
		while (running) {
			if (clientSocket.Available > 0) {
			
				
				receiveData = new byte[2 * handStructSize + 10 * fingerStructSize + 10 * touchStructSize];
				receiveDataLength = clientSocket.Receive (receiveData);
				receiveStr = Encoding.ASCII.GetString (receiveData, 0, receiveDataLength);	
			}
		}
	}
	
	public string ReturnStr ()
	{
		lock (this) {
			if (receiveStr != null) {
				returnStr = receiveStr.Substring (0);
				receiveStr = null;
				return returnStr;
			} else {
				returnStr = null;
				return returnStr;
			}
		}	
	}
	
	public byte[] ReturnBytes ()
	{
		lock (this) {
			return receiveData;
		}	
	}
	
	public void SocketQuit ()
	{
		running = false;
		if (clientSocket != null) {
			
			clientSocket.Close ();
			
		}
		
		if (connectThread != null) {
			connectThread.Interrupt ();
			connectThread.Abort ();
		}
		serverSocket.Close ();
	}
}
