using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public enum DataStruct
{
	Hand=1,
	Finger=2,
	ScreenTouch=4,
};

public enum TouchState
{
	Down=1,
	Move=2,
	Up=3
};

public struct Position2D
{
	public float x;
	public float y;
};

public struct Position3D
{
	public float x;
	public float y;
	public float z;
};

public struct Orientation
{
	public float x;
	public float y;
	public float z;
};

public struct HandStruct
{
	public DataStruct structType;
	public int handId;
	public int[]fingerId;
	public int frame;
	public Position3D handPosition;
	public Orientation handOrientation;
	public bool valid;
};

public struct FingerStruct
{
	public DataStruct structType;
	public int fingerId;
	public int handId;
	public int frame;
	public Position3D fingerPosition;
	public Orientation fingerOrientation;
	public bool valid;
};

public struct TouchStruct
{
	public DataStruct structType;
	public int touchId;
	public int frame;
	public Position2D touchPosition;
	public TouchState touchState;
	public bool valid;
}

public class Connection : MonoBehaviour
{
	public GameObject crosshairPrefab;
	private ArrayList crosshairs = new ArrayList ();
	
	const int handStructSize = 57;
	const int fingerStructSize = 41;
	const int touchStructSize = 25;
	
	int screenResolutionX = Screen.width;
	int screenResolutionY = Screen.height;
	
	public static HandStruct[]handsData = new HandStruct[2];
	public static FingerStruct[]fingersData = new FingerStruct[10];
	public static TouchStruct[]touchesData = new TouchStruct[10];
	
	int lastFrame=0;
	
	public static void getHandStruct (ref HandStruct handStruct, byte[] rawdatas, int startIndex)
	{
		handStruct.structType = DataStruct.Hand;
		
		byte[] handByte = new byte[handStructSize];
		Array.Copy (rawdatas, startIndex, handByte, 0, handStructSize);
		
		byte[] validByte = new byte[1];
		Array.Copy (handByte, 4, validByte, 0, 1);
		handStruct.valid = BitConverter.ToBoolean (validByte, 0);
		
		if (handStruct.valid == false)
			return;
		
		byte[] handIdByte = new byte[4];
		Array.Copy (handByte, 5, handIdByte, 0, 4);
		handStruct.handId = BitConverter.ToInt32 (handIdByte, 0);
		
		handStruct.fingerId = new int[5];
		
		for (int i=0; i<5; ++i) {
			byte[] fingerIdByte = new byte[4];
			Array.Copy (handByte, 9 + i * 4, fingerIdByte, 0, 4);
			handStruct.fingerId [i] = BitConverter.ToInt32 (fingerIdByte, 0);
		}
		
		byte[] frameByte = new byte[4];
		Array.Copy (handByte, 29, frameByte, 0, 4);
		handStruct.frame = BitConverter.ToInt32 (frameByte, 0);
		
		byte[] positionXByte = new byte[4];
		byte[] positionYByte = new byte[4];
		byte[] positionZByte = new byte[4];
		Array.Copy (handByte, 33, positionXByte, 0, 4);
		Array.Copy (handByte, 37, positionYByte, 0, 4);
		Array.Copy (handByte, 41, positionZByte, 0, 4);
		handStruct.handPosition.x = BitConverter.ToSingle (positionXByte, 0);
		handStruct.handPosition.y = BitConverter.ToSingle (positionYByte, 0);
		handStruct.handPosition.z = BitConverter.ToSingle (positionZByte, 0);
		
		byte[] orientationXByte = new byte[4];
		byte[] orientationYByte = new byte[4];
		byte[] orientationZByte = new byte[4];
		Array.Copy (handByte, 45, orientationXByte, 0, 4);
		Array.Copy (handByte, 49, orientationYByte, 0, 4);
		Array.Copy (handByte, 53, orientationZByte, 0, 4);
		handStruct.handOrientation.x = BitConverter.ToSingle (orientationXByte, 0);
		handStruct.handOrientation.y = BitConverter.ToSingle (orientationYByte, 0);
		handStruct.handOrientation.z = BitConverter.ToSingle (orientationZByte, 0);
	}
	
	public static void getFingerStruct (ref FingerStruct fingerStruct, byte[] rawdatas, int startIndex)
	{
		fingerStruct.structType = DataStruct.Finger;
		
		byte[] fingerByte = new byte[fingerStructSize];
		Array.Copy (rawdatas, startIndex, fingerByte, 0, fingerStructSize);
		
		byte[] validByte = new byte[1];
		Array.Copy (fingerByte, 4, validByte, 0, 1);
		fingerStruct.valid = BitConverter.ToBoolean (validByte, 0);
		
		byte[] fingerIdByte = new byte[4];
		Array.Copy (fingerByte, 5, fingerIdByte, 0, 4);
		fingerStruct.fingerId = BitConverter.ToInt32 (fingerIdByte, 0);
		
		byte[] handIdByte = new byte[4];
		Array.Copy (fingerByte, 9, handIdByte, 0, 4);
		fingerStruct.handId = BitConverter.ToInt32 (handIdByte, 0);
		
		byte[] frameByte = new byte[4];
		Array.Copy (fingerByte, 13, frameByte, 0, 4);
		fingerStruct.frame = BitConverter.ToInt32 (frameByte, 0);
		
		byte[] positionXByte = new byte[4];
		byte[] positionYByte = new byte[4];
		byte[] positionZByte = new byte[4];
		Array.Copy (fingerByte, 17, positionXByte, 0, 4);
		Array.Copy (fingerByte, 21, positionYByte, 0, 4);
		Array.Copy (fingerByte, 25, positionZByte, 0, 4);
		fingerStruct.fingerPosition.x = BitConverter.ToSingle (positionXByte, 0);
		fingerStruct.fingerPosition.y = BitConverter.ToSingle (positionYByte, 0);
		fingerStruct.fingerPosition.z = BitConverter.ToSingle (positionZByte, 0);
		
		byte[] orientationXByte = new byte[4];
		byte[] orientationYByte = new byte[4];
		byte[] orientationZByte = new byte[4];
		Array.Copy (fingerByte, 29, orientationXByte, 0, 4);
		Array.Copy (fingerByte, 33, orientationYByte, 0, 4);
		Array.Copy (fingerByte, 37, orientationZByte, 0, 4);
		fingerStruct.fingerOrientation.x = BitConverter.ToSingle (orientationXByte, 0);
		fingerStruct.fingerOrientation.y = BitConverter.ToSingle (orientationYByte, 0);
		fingerStruct.fingerOrientation.z = BitConverter.ToSingle (orientationZByte, 0);
	}
	
	public static void getTouchStruct (ref TouchStruct touchStruct, byte[] rawdatas, int startIndex)
	{
		touchStruct.structType = DataStruct.ScreenTouch;
		
		byte[] touchByte = new byte[touchStructSize];
		Array.Copy (rawdatas, startIndex, touchByte, 0, touchStructSize);
		
		byte[] validByte = new byte[1];
		Array.Copy (touchByte, 4, validByte, 0, 1);
		touchStruct.valid = BitConverter.ToBoolean (validByte, 0);
		
		byte[] touchIdByte = new byte[4];
		Array.Copy (touchByte, 5, touchIdByte, 0, 4);
		touchStruct.touchId = BitConverter.ToInt32 (touchIdByte, 0);
		
		byte[] frameByte = new byte[4];
		Array.Copy (touchByte, 9, frameByte, 0, 4);
		touchStruct.frame = BitConverter.ToInt32 (frameByte, 0);
		
		byte[] positionXByte = new byte[4];
		byte[] positionYByte = new byte[4];
		Array.Copy (touchByte, 13, positionXByte, 0, 4);
		Array.Copy (touchByte, 17, positionYByte, 0, 4);
		touchStruct.touchPosition.x = BitConverter.ToSingle (positionXByte, 0);
		touchStruct.touchPosition.y = BitConverter.ToSingle (positionYByte, 0);
		
		byte [] stateByte = new byte[4];
		Array.Copy (touchByte, 21, stateByte, 0, 4);
		int stateNumber = BitConverter.ToInt32 (stateByte, 0);
		
		switch (stateNumber) {
		case 1:
			touchStruct.touchState = TouchState.Down;
//			print ("Down");
			break;
		case 2:
			touchStruct.touchState = TouchState.Move;
//			print ("Move");
			break;
		case 3:
			touchStruct.touchState = TouchState.Up;
//			print ("Up");
			break;
		default:
			break;
		}
	}
	
	private void rawDeserialize (byte[] rawdatas)
	{
		int handIndex = 0;
		int fingerIndex = 0;
		int touchIndex = 0;
		
		int startIndex = 0;
		int frameCount = 0;
		
		int dataSize = rawdatas.Length;
		
		bool stop = false;
		
		while (startIndex<dataSize-1) {
			byte[] typeByte = new byte[4];
			Array.Copy (rawdatas, startIndex, typeByte, 0, 4);
			int type = BitConverter.ToInt32 (typeByte, 0);
			
			switch (type) {
			case 1:
				HandStruct handStruct = new HandStruct ();
				getHandStruct (ref handStruct, rawdatas, startIndex);
				startIndex += handStructSize;
				handsData [handIndex] = handStruct;
				handIndex++;
				break;
				
			case 2:
				FingerStruct fingerStruct = new FingerStruct ();
				getFingerStruct (ref fingerStruct, rawdatas, startIndex);
				startIndex += fingerStructSize;
				fingersData [fingerIndex] = fingerStruct;
				fingerIndex++;
				break;

			case 4:
				TouchStruct touchStruct = new TouchStruct ();
				getTouchStruct (ref touchStruct, rawdatas, startIndex);
				startIndex += touchStructSize;
				touchesData [touchIndex] = touchStruct;
				touchIndex++;
				break;
				
			default:
				stop = true;
				break;
			}
			
			if (stop == true)
				break;
		}	
	}
	
	void GetValidInput ()
	{
		InputManager.getLastHands().Clear();
		InputManager.getLastHands().AddRange(InputManager.getHands());
		InputManager.getLastFingers().Clear();
		InputManager.getLastFingers().AddRange(InputManager.getFingers());
		InputManager.getLastScreenTouches().Clear();
		InputManager.getLastScreenTouches().AddRange(InputManager.getScreenTouches());
		
		InputManager.getHands().Clear ();
		InputManager.getFingers().Clear ();
		InputManager.getScreenTouches().Clear ();
		
		
		
		for (int i=0; i<handsData.Length; ++i) {
			if (handsData [i].valid == true) {
				Hand hand = new Hand (handsData [i]);
				InputManager.getHands().Add (hand);
			}
		}
		
		for (int i=0; i<fingersData.Length; ++i) {
			if (fingersData [i].valid == true) {
				Finger finger = new Finger (fingersData [i]);
				InputManager.getFingers().Add (finger);
			}
		}
		
		for (int i=0; i<touchesData.Length; ++i) {
			if (touchesData [i].valid == true) {
				ScreenTouch touch = new ScreenTouch (touchesData [i]);
				InputManager.getScreenTouches().Add (touch);
				
				SceneManager sceneManager = (SceneManager)GameObject.Find ("Scene").GetComponent ("SceneManager");
	
				Vector3 touchPos = touchesData[i].touchPosition.x * sceneManager.newVec2*sceneManager.screenWidth + touchesData[i].touchPosition.y * sceneManager.newVec1*sceneManager.screenHeight;
//				print (touchPos);
			}
		}
		
//		if(InputManager.getLastScreenTouches().Count!=0&&InputManager.getScreenTouches().Count!=0)
//		{
//			print ("frame "+InputManager.getScreenTouches()[0].getFrame()+" last "+InputManager.getLastScreenTouches()[0].getTouchPosition()+" this "+InputManager.getScreenTouches()[0].getTouchPosition());
//		}
	}
	
	ServerSocket serverSocket;
	string str = null;
	
	// Use this for initialization
	void Start ()
	{
		serverSocket = new ServerSocket ();
		serverSocket.Init ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		str = serverSocket.ReturnStr ();
		if (str != null) {
			rawDeserialize (serverSocket.ReturnBytes ());
			
			GetValidInput ();
			
			GameObject Hand1 = GameObject.Find ("Hand1");
			Hand1.transform.position = GetTransformedPosition (new Vector3 (handsData [0].handPosition.x, handsData [0].handPosition.y, handsData [0].handPosition.z));

			GameObject Hand2 = GameObject.Find ("Hand2");
			Hand2.transform.position = GetTransformedPosition (new Vector3 (handsData [1].handPosition.x, handsData [1].handPosition.y, handsData [1].handPosition.z));

			
			GameObject finger1 = GameObject.Find ("Finger1");
			finger1.transform.position = GetTransformedPosition (new Vector3 (fingersData [0].fingerPosition.x, fingersData [0].fingerPosition.y, fingersData [0].fingerPosition.z));
			GameObject finger2 = GameObject.Find ("Finger2");
			finger2.transform.position = GetTransformedPosition (new Vector3 (fingersData [1].fingerPosition.x, fingersData [1].fingerPosition.y, fingersData [1].fingerPosition.z));
			GameObject finger3 = GameObject.Find ("Finger3");
			finger3.transform.position = GetTransformedPosition (new Vector3 (fingersData [2].fingerPosition.x, fingersData [2].fingerPosition.y, fingersData [2].fingerPosition.z));
			GameObject finger4 = GameObject.Find ("Finger4");
			finger4.transform.position = GetTransformedPosition (new Vector3 (fingersData [3].fingerPosition.x, fingersData [3].fingerPosition.y, fingersData [3].fingerPosition.z));
			GameObject finger5 = GameObject.Find ("Finger5");
			finger5.transform.position = GetTransformedPosition (new Vector3 (fingersData [4].fingerPosition.x, fingersData [4].fingerPosition.y, fingersData [4].fingerPosition.z));
			GameObject finger6 = GameObject.Find ("Finger6");
			finger6.transform.position = GetTransformedPosition (new Vector3 (fingersData [5].fingerPosition.x, fingersData [5].fingerPosition.y, fingersData [5].fingerPosition.z));
			GameObject finger7 = GameObject.Find ("Finger7");
			finger7.transform.position = GetTransformedPosition (new Vector3 (fingersData [6].fingerPosition.x, fingersData [6].fingerPosition.y, fingersData [6].fingerPosition.z));
			GameObject finger8 = GameObject.Find ("Finger8");
			finger8.transform.position = GetTransformedPosition (new Vector3 (fingersData [7].fingerPosition.x, fingersData [7].fingerPosition.y, fingersData [7].fingerPosition.z));
			GameObject finger9 = GameObject.Find ("Finger9");
			finger9.transform.position = GetTransformedPosition (new Vector3 (fingersData [8].fingerPosition.x, fingersData [8].fingerPosition.y, fingersData [8].fingerPosition.z));
			GameObject finger10 = GameObject.Find ("Finger10");
			finger10.transform.position = GetTransformedPosition (new Vector3 (fingersData [9].fingerPosition.x, fingersData [9].fingerPosition.y, fingersData [9].fingerPosition.z));
		
		
			int crosshairIndex = 0;
			
			for (int i = 0; i < touchesData.Length; i++) {
				if (crosshairs.Count <= crosshairIndex) {
					// make a new crosshair and cache it
					GameObject newCrosshair = (GameObject)Instantiate (crosshairPrefab, Vector3.zero, Quaternion.identity);
					crosshairs.Add (newCrosshair);
				}
		
				Vector3 screenPosition = new Vector3 ((int)(touchesData [i].touchPosition.x * screenResolutionX), (int)(screenResolutionY - touchesData [i].touchPosition.y * screenResolutionY), 0.0f);
				GameObject thisCrosshair = (GameObject)crosshairs [crosshairIndex];
				thisCrosshair.SetActiveRecursively (true);

				thisCrosshair.transform.position = Camera.mainCamera.ScreenToViewportPoint (screenPosition);
		
				crosshairIndex++;
			}
		
		}
	}

	// Update is called once per frame
	void OnGUI ()
	{		
		
		
	
	}
	
	void OnApplicationQuit ()
	{
		serverSocket.SocketQuit ();
	}
	
	//Transform the coordinate of a point to the reference of the plane
	Vector3 GetTransformedPosition (Vector3 position)
	{	
		GameObject screenReference = GameObject.Find ("ScreenReference");
		Vector3 newPosition = screenReference.transform.InverseTransformPoint (position);
		
		//Reverse the x coordinate to adapt to the reference of left hand
		newPosition.x*=-1;
		return newPosition;
	}
}
