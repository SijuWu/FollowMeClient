using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;

enum StrokeType
{
	Pan,
	WaitZoom,
	Zoom,
	None
};

public class Hand
{
	public Hand (HandStruct handStruct)
	{
		handId = handStruct.handId;
		
		fingerIds = new List<int> ();
		for (int i=0; i<handStruct.fingerId.Length; ++i) {
			if (handStruct.fingerId [i] != 9999)
				fingerIds.Add (handStruct.fingerId [i]);
		}
		
		frame = handStruct.frame;
		
		//Transform the position to the screen reference
		GameObject screenReference = GameObject.Find ("ScreenReference");
		handPosition = screenReference.transform.InverseTransformPoint (new Vector3 (handStruct.handPosition.x, handStruct.handPosition.y, handStruct.handPosition.z));
		handOrientation = screenReference.transform.InverseTransformPoint (new Vector3 (handStruct.handOrientation.x, handStruct.handOrientation.y, handStruct.handOrientation.z));
	}
		
	public int getId ()
	{
		return handId;
	}
		
//	public ArrayList getFingerIds ()
//	{
//		return fingerIds;
//	}
	public List<int> getFingerIds ()
	{
		return fingerIds;
	}
		
	public int getFrame ()
	{
		return frame;
	}
		
	public Vector3 getHandPosition ()
	{
		return handPosition;
	}
		
	public Vector3 getHandOrientation ()
	{
		return handOrientation;
	}
		
	int handId;
//	private ArrayList fingerIds;
	List<int> fingerIds;
	int frame;
	Vector3 handPosition;
	Vector3 handOrientation;
}
	
public class Finger
{
	
	public Finger (FingerStruct fingerStruct)
	{
		fingerId = fingerStruct.fingerId;
		handId = fingerStruct.handId;
		frame = fingerStruct.frame;
		
		//Transform the position to the screen reference
		GameObject screenReference = GameObject.Find ("ScreenReference");
		fingerPosition = screenReference.transform.InverseTransformPoint (new Vector3 (fingerStruct.fingerPosition.x, fingerStruct.fingerPosition.y, fingerStruct.fingerPosition.z));
		fingerOrientation = screenReference.transform.InverseTransformPoint (new Vector3 (fingerStruct.fingerOrientation.x, fingerStruct.fingerOrientation.y, fingerStruct.fingerOrientation.z));
	}
		
	public int getId ()
	{
		return fingerId;
	}
		
	public int getHandId ()
	{
		return handId;
	}
		
	public int getFrame ()
	{
		return frame;
	}
		
	public Vector3 getFingerPosition ()
	{
		return fingerPosition;
	}
		
	public Vector3 getFingerOrientation ()
	{
		return fingerOrientation;
	}
		
	public int fingerId;
	public int handId;
	public int frame;
	public Vector3 fingerPosition;
	public Vector3 fingerOrientation;

}
	
public class ScreenTouch
{
	public ScreenTouch (TouchStruct touchStruct)
	{
		touchId = touchStruct.touchId;
		frame = touchStruct.frame;
		touchPosition = new Vector2 (touchStruct.touchPosition.x * Screen.width, touchStruct.touchPosition.y * Screen.height);

		touchState = touchStruct.touchState;
	}
		
	public int getId ()
	{
		return touchId;
	}
		
	public int getFrame ()
	{
		return frame;
	}
		
	public Vector2 getTouchPosition ()
	{
		return touchPosition;
	}
		
	public TouchState getTouchState ()
	{
		return touchState;
	}
		
	private int touchId;
	private int frame;
	private Vector2 touchPosition;
	private TouchState touchState;
}

public class Stroke
{
//	public StrokeType getStrokeType ()
//	{
//		return strokeType;
//	}
//	
//	public ArrayList getStrokePoints ()
//	{
//		return strokePoints;
//	}
	public List<ScreenTouch> getStrokePoints ()
	{
		return strokePoints;
	}
	
	public float getX0 ()
	{
		return X0;
	}
	
	public void setX0 (float X0Value)
	{
		X0 = X0Value;
	}
	
	public float getY0 ()
	{
		return Y0;
	}
	
	public void setY0 (float Y0Value)
	{
		Y0 = Y0Value;
	}
	
	public float getRadius ()
	{
		return radius;
	}
	
	public void setRadius (float radiusValue)
	{
		radius = radiusValue;
	}
	
	public float getStartTime ()
	{
		return startTime;
	}
	
	public void setStartTime (float time)
	{
		startTime = time;
	}
	
	public float getEndTime ()
	{
		return endTime;
	}
	
	public void setEndTime (float time)
	{
		endTime = time;
	}
	
	//Stroke points
	List<ScreenTouch> strokePoints = new List<ScreenTouch> ();
	
	//Circle parameters
	float X0;
	float Y0;
	float radius;
	float startTime;
	float endTime;
}

public class InputManager : MonoBehaviour
{
	
	static List<Hand> hands = new List<Hand> ();
	static List<Finger> fingers = new List<Finger> ();
	static List<ScreenTouch> screenTouches = new List<ScreenTouch> ();
	static List<Hand> lastHands = new List<Hand> ();
	static List<Finger> lastFingers = new List<Finger> ();
	static List<ScreenTouch> lastScreenTouches = new List<ScreenTouch> ();
	
	
	//Line render
	LineRenderer line;
	StrokeType strokeType;
	//Active stroke
	Stroke stroke = new Stroke ();
	//vector of all strokes
	List<Stroke> strokes = new List<Stroke> ();
	int lastStrokesCount = 0;
	float scale = 1;
	int lastFrame = 0;
	float lastTime = 0;
	float breakTime = 0;
	Vector3 lastRotateAxis = new Vector3 (0, 0, 0);
	int lastFingerId;
	float lightTheta = 45;
	float lightAlpha = 0;

	public static List<Hand> getHands ()
	{
		return hands;
	}
	
	public static List<Finger> getFingers ()
	{
		return fingers;
	}
	
	public static List<ScreenTouch> getScreenTouches ()
	{
		return screenTouches;
	}
	
	public static List<Hand> getLastHands ()
	{
		return lastHands;
	}
	
	public static List<Finger> getLastFingers ()
	{
		return lastFingers;
	}
	
	public static List<ScreenTouch> getLastScreenTouches ()
	{
		return lastScreenTouches;
	}
	
	
	// Use this for initialization
	void Start ()
	{
		line = gameObject.AddComponent<LineRenderer> ();
		line.material = new Material (Shader.Find ("Particles/Additive"));
		line.SetVertexCount (0);
		line.SetWidth (0.1f, 0.1f);
		line.SetColors (Color.green, Color.green);
		line.useWorldSpace = true;    
	}
	
	void controlLight ()
	{
//		//The first
//		if(screenTouches.Count==2&&lastScreenTouches.Count!=0)
//		{}
		if (screenTouches.Count == 2 && lastScreenTouches.Count == 2) {
			ScreenTouch lastTouch1 = lastScreenTouches [0];
			ScreenTouch lastTouch2 = lastScreenTouches [1];
			
			ScreenTouch touch1 = screenTouches [0];
			ScreenTouch touch2 = screenTouches [1];
			
			if (lastTouch1.getId () == screenTouches [1].getId ()) {
				touch1 = screenTouches [1];
				touch2 = screenTouches [0];
			}
			
			float dis1 = Vector2.Distance (lastTouch1.getTouchPosition (), touch1.getTouchPosition ());
			float dis2 = Vector2.Distance (lastTouch2.getTouchPosition (), touch2.getTouchPosition ());
			
			
			
			float lastDis = Vector2.Distance (lastTouch1.getTouchPosition (), lastTouch2.getTouchPosition ());
			float dis = Vector2.Distance (touch1.getTouchPosition (), touch2.getTouchPosition ());
				
			float disVariance = dis - lastDis;
			
			lightTheta += disVariance / 2;
			
			Vector2 lastVec = new Vector2 ();
			Vector2 vec = new Vector2 ();
			
			if (dis1 > dis2) {
				lastVec = lastTouch1.getTouchPosition () - lastTouch2.getTouchPosition ();
				vec = touch1.getTouchPosition () - touch2.getTouchPosition ();
			}
			if (dis1 < dis2) {
				lastVec = lastTouch2.getTouchPosition () - lastTouch1.getTouchPosition ();
				vec = touch2.getTouchPosition () - touch1.getTouchPosition ();
			}
			
			lastVec.Normalize ();
			vec.Normalize ();
			float cross = lastVec.x * vec.y - lastVec.y * vec.x;
			float alphaVariance = Mathf.Acos (Vector2.Dot (lastVec, vec));
			if (cross > 0)
				lightAlpha += alphaVariance;
			if (cross < 0)
				lightAlpha -= alphaVariance;

		}
	}
	// Update is called once per frame
	void Update ()
	{	
		lastStrokesCount = strokes.Count;
		getStroke ();
		checkCycloGesture ();
		lastTime = Time.time;

		controlLight ();
		GameObject controlLine = GameObject.Find ("ControlLine");
		LineRenderer line = controlLine.GetComponent<LineRenderer> ();
		line.SetVertexCount (10);
		for (int i=0; i<10; ++i) {
			line.SetPosition (i, new Vector3 (100 * i * Mathf.Cos (lightTheta * Mathf.PI / 180)*Mathf.Cos (lightAlpha), 
				                              100 * i * Mathf.Sin (lightTheta * Mathf.PI / 180), 
				                              100 * i * Mathf.Cos (lightTheta * Mathf.PI / 180)*Mathf.Sin (lightAlpha)));
		}
		
		line.SetWidth (10, 10);
		
		
		
		//Rotate by using two fingers in air and one finger on the surface
//		bool rotate = false;
//		Vector3 rotateAxis = CheckRotateAxis ();
//		float rotateAngle = CheckRotateAngle (ref rotate);
//		
//		GameObject cube = GameObject.Find ("Cube");
//		if (rotate == true) {
//			Debug.DrawLine (cube.transform.position, cube.transform.position + lastRotateAxis, Color.red);
//			Debug.DrawLine (cube.transform.position, cube.transform.position - lastRotateAxis, Color.red);
//			cube.transform.RotateAroundLocal (rotateAxis, rotateAngle);
//		} else {
//			Debug.DrawLine (cube.transform.position, cube.transform.position + rotateAxis, Color.red);
//			Debug.DrawLine (cube.transform.position, cube.transform.position - rotateAxis, Color.red);
//			lastRotateAxis = rotateAxis;
//		}
		
		
		
//		//Rotate by using two fingers on the surface and one finger in air
//		Vector2 rotateAxis = CheckRotateAxis ();
//		float rotateAngle = CheckRotateAngle ();
//		
//		GameObject cube = GameObject.Find ("Cube");
//		Debug.DrawLine (cube.transform.position, new Vector3 (rotateAxis.x, cube.transform.position.y, rotateAxis.y), Color.red);
//		Debug.DrawLine (cube.transform.position, new Vector3 (-rotateAxis.x, cube.transform.position.y, -rotateAxis.y), Color.red);
//		
//		cube.transform.RotateAroundLocal (new Vector3 (rotateAxis.x, 0, rotateAxis.y), rotateAngle);
	}
	
//	//Check rotation axis by using two touch input points
//	Vector2 CheckRotateAxis ()
//	{
//		int touchIndex = 0;
//		ScreenTouch[] touches = new ScreenTouch[2];
//		
//		for (int i=0; i<screenTouches.Count; ++i) {
//
//			if (touchIndex == 2)
//				break;
//			touches [touchIndex] = (ScreenTouch)screenTouches [i];
//			touchIndex++;
//		}
//		
//		if (touchIndex == 2) {
//			Vector2 direction = new Vector2 (touches [0].getTouchPosition ().x - touches [1].getTouchPosition ().x, 
//				                             touches [0].getTouchPosition ().y - touches [1].getTouchPosition ().y);
//			
//			
//			return direction;
//				
////
//		} else
//			return new Vector2 (0, 0);
//	}
//	
//	//Check the rotation angle by using the third finger floating in air
//	float CheckRotateAngle ()
//	{	
//		int fingerIndex=-1;
//		int fingerId = -1;
//		for (int i=0; i<fingers.Count; ++i) {
//			Finger finger = (Finger)fingers [i];
//			if (finger.getFingerPosition ().y > 100) {
//				fingerId = finger.getId ();
//				fingerIndex=i;
//				break;
//			}
//		}
//		
//		if (fingerId != -1) {
//			
//			bool sameFinger=false;
//			Finger finger=(Finger)fingers[fingerIndex];
//			
//			for (int i=0; i<lastFingers.Count; ++i) {
//				Finger lastFinger = (Finger)lastFingers [i];
//				
//				if (lastFinger.getId () == fingerId&&lastFinger.getFrame()!=finger.getFrame()&&lastFrame!=lastFinger.getFrame()) {
//					sameFinger=true;
//					print ("last frame "+lastFinger.getFrame()+" this frame "+finger.getFrame()+" "+(finger.getFingerPosition().y-lastFinger.getFingerPosition().y));
//					return(finger.getFingerPosition().y-lastFinger.getFingerPosition().y)/50;
//					lastFrame=lastFinger.getFrame();
//				}
//			}
//			
//			if(sameFinger==false)
//				return 0;
//		}		
//		
//		return 0;
//	}
	
	//Check rotation axis by using two finger input points
	Vector3 CheckRotateAxis ()
	{
		Vector3 rotationAxis = new Vector3 (0, 0, 0);
		bool axisFound = false;
		
		foreach (Hand hand in hands) {
//			print (hand.getId()+" "+hand.getFingerIds().Count);
			for (int i=0; i<hand.getFingerIds().Count; ++i) {
//				print (hand.getFingerIds () [i]);
			}
			
			if (hand.getFingerIds ().Count == 2) {
				int fingerOneId = (int)hand.getFingerIds () [0];
				int fingerTwoId = (int)hand.getFingerIds () [1];
				
				Finger fingerOne = null;
				Finger fingerTwo = null;
				
				foreach (Finger finger in fingers) {
					
					if (finger.getId () == fingerOneId)
						fingerOne = finger;
					
					if (finger.getId () == fingerTwoId)
						fingerTwo = finger;
				}
				
				if (fingerOne.getFingerPosition ().y > 100 && fingerTwo.getFingerPosition ().y > 100) {
					rotationAxis = fingerOne.getFingerPosition () - fingerTwo.getFingerPosition ();
					axisFound = true;
				}
			}
		}
		
		if (axisFound == true)
			return rotationAxis;
		else
			return new Vector3 (0, 0, 0);
	}
	
	float CheckRotateAngle (ref bool rotate)
	{
		float rotateAngle = 0;
		if (screenTouches.Count == 1 && lastScreenTouches.Count == 1) {
			ScreenTouch touch = (ScreenTouch)screenTouches [0];
			ScreenTouch lastTouch = (ScreenTouch)lastScreenTouches [0];
			
			if (touch.getId () == lastFingerId) {
				rotateAngle = touch.getTouchPosition ().y - lastTouch.getTouchPosition ().y;
				rotate = true;
			}
			lastFingerId = touch.getId ();
		}
		
		return rotateAngle / 50;
	}
	
	void getStroke ()
	{
		//Clear stroke and strokes 
		if (screenTouches.Count != 1) {
			stroke.getStrokePoints ().Clear ();
			strokes.Clear ();
			strokeType = StrokeType.None;
			return;
		}	
		//If it is the same frame, do not save the point
		if (lastFrame == screenTouches [0].getFrame ())
			return;
		lastFrame = screenTouches [0].getFrame ();
		
		
		//Set the start time
		if (stroke.getStrokePoints ().Count == 0)
			stroke.setStartTime (Time.time);
		
		//Add touch point to the active stroke
		stroke.getStrokePoints ().Add (screenTouches [0]);
		
		//If there are too few touch points
		if (stroke.getStrokePoints ().Count < 5)
			return;
		
		//Check if the stroke reverse
		if (CheckReverse () == true) {
			//Set the end time
			stroke.setEndTime (Time.time);
//			print (stroke.getEndTime()-stroke.getStartTime());
			strokes.Add (stroke);
			stroke = new Stroke ();
			stroke.setStartTime (Time.time);
			stroke.getStrokePoints ().Add (screenTouches [0]);
		}	

	}
	
	bool CheckReverse ()
	{
		int size = stroke.getStrokePoints ().Count;
		
		ScreenTouch startPoint = (ScreenTouch)stroke.getStrokePoints () [0];
		ScreenTouch lastEndPoint = (ScreenTouch)stroke.getStrokePoints () [size - 2];
		ScreenTouch endPoint = (ScreenTouch)stroke.getStrokePoints () [size - 1];
		
		float lastDis = getDistance (startPoint, lastEndPoint);
		float dis = getDistance (startPoint, endPoint);
		
		//Check the distance between the end point and the start point
		if ((lastDis - dis) > 1)
			return true;
		else
			return false;
	}
	
	void checkCycloGesture ()
	{
		//Fit the first stroke to ecclipse
		if (lastStrokesCount == 0 && strokes.Count == 1) {
			float eccentricity = FitStroke2Ellipse (strokes [0]);
		    
			//Wait for the second reverse to activate cycloZoom
			if (eccentricity < 0.8 && eccentricity > 0)
				strokeType = StrokeType.WaitZoom;
			//Start the cyclopan
			if (eccentricity > 0.8 || eccentricity == 0)
				strokeType = StrokeType.Pan;
		}
		
		if (strokeType == StrokeType.Pan) {
			cycloPan ();
		}
		
		//Activate the cyclozoom
		if (lastStrokesCount == 1 && strokes.Count == 2 && strokeType == StrokeType.WaitZoom) {
			strokeType = StrokeType.Zoom;
			FitStroke2Circle (strokes [0], strokes [1]);
		}
		
		//Fit the two last strokes to a circle, so the circle can be refreshed half a period
		if (strokeType == StrokeType.Zoom && strokes.Count >= 2 && lastStrokesCount != strokes.Count) {
			
			FitStroke2Circle (strokes [strokes.Count - 2], strokes [strokes.Count - 1]);
		}
		
		//Use the cyclozoom to change the scale
		if (strokeType == StrokeType.Zoom && strokes.Count >= 2 && stroke.getStrokePoints ().Count >= 2) {
			cycloZoom ();
		}
	}
	
	void cycloPan ()
	{
		Stroke firstStroke = strokes [0];
		Vector2 startPoint = firstStroke.getStrokePoints () [0].getTouchPosition ();
		Vector2 endPoint = firstStroke.getStrokePoints () [firstStroke.getStrokePoints ().Count - 1].getTouchPosition ();
			
		float dragDis = Vector2.Distance (startPoint, endPoint);
		float dragSpeed = dragDis / (firstStroke.getEndTime () - firstStroke.getStartTime ());
			
		//If the speed is not high enough, do not trigger the cyclopan
		if (dragSpeed < 50) {
			strokes.Clear ();
			stroke.getStrokePoints ().Clear ();
			stroke = new Stroke ();
			strokeType = StrokeType.None;		
		} else {	
			//Get the average frequence of the last three strokes
			float frequenceAve = 0;
			int strokeCount = 0;
			for (int i=strokes.Count-1; i>=0; --i) {
				
				strokeCount++;
				float Te = strokes [i].getEndTime () - strokes [i].getStartTime ();
				float frequence = 0.5f / Te;
					
				if (strokeCount > 3)
					break;
				frequenceAve += frequence;
					
			}
			frequenceAve /= strokeCount;

			
			//Get the gain
			float k = 1.0f;
			float gain = Mathf.Max (1, k * frequenceAve);
			
			Vector2 lastPoint = lastScreenTouches [0].getTouchPosition ();
			Vector2 point = screenTouches [0].getTouchPosition ();
			float dis = Vector2.Distance (lastPoint, point);
			
			//If the break time is bigger than the threshold value, cancel the cyclopan
			if (dis < 2) {
				breakTime += Time.time - lastTime;
				if (breakTime > 0.1) {
					strokes.Clear ();
					stroke.getStrokePoints ().Clear ();
					stroke = new Stroke ();
					strokeType = StrokeType.None;
				}
			} else {
				breakTime = 0;
				
				float dragRange = gain * dis;
				
				Vector2 dragDirection = endPoint - startPoint;
				dragDirection.Normalize ();
				
				GameObject cube = GameObject.Find ("Cube");
				cube.transform.Translate (new Vector3 (dragDirection.x * dragRange, 0, dragDirection.y * dragRange));
					
			}
		}
	}
	
	void cycloZoom ()
	{
		Vector2 checkPoint1 = stroke.getStrokePoints () [0].getTouchPosition ();
		int curveCenterIndex = (int)Mathf.Floor (stroke.getStrokePoints ().Count / 2);
		Vector2 checkPoint2 = stroke.getStrokePoints () [curveCenterIndex].getTouchPosition ();
			
		Vector2 circleCenter = new Vector2 (strokes [strokes.Count - 1].getX0 (), strokes [strokes.Count - 1].getY0 ());
		Vector2 lastVec = new Vector2 (checkPoint1.x - circleCenter.x, checkPoint1.y - circleCenter.y);
		Vector2 vec = new Vector2 (checkPoint2.x - circleCenter.x, checkPoint2.y - circleCenter.y);
			
		//Check the rotation direction
		float cross = lastVec.x * vec.y - lastVec.y * vec.x;
			
		//Check the rotation angle
		Vector2 lastPoint = stroke.getStrokePoints () [stroke.getStrokePoints ().Count - 2].getTouchPosition ();
		Vector2 point = stroke.getStrokePoints () [stroke.getStrokePoints ().Count - 1].getTouchPosition ();
			
		float lastDis = Vector2.Distance (lastPoint, circleCenter);
		float dis = Vector2.Distance (point, circleCenter);
			
		float motion = Vector2.Distance (lastPoint, point);
//			print ("lastDis " + lastDis + " dis " + dis +" motion "+motion +" radius " + strokes [strokes.Count - 1].getRadius ());

		float angleChange = motion / strokes [strokes.Count - 1].getRadius ();
//			print ("angle variation " + angleChange);
			
		float K = 0;
		if (cross > 0)
			K = 0.14f;
		if (cross < 0)
			K = -0.32f;
			
		GameObject cube = GameObject.Find ("Cube");
		//Change the scale
		scale = cube.transform.localScale.x;
		scale = scale * (1 + K * angleChange);
		if (scale < 1)
			scale = 1.0f;
		if (scale > 1000)
			scale = 1000.0f;
//		print ("scale " + scale);
		
		cube.transform.localScale = new Vector3 (scale, scale, scale);
	}
	
	float getDistance (ScreenTouch touch1, ScreenTouch touch2)
	{
		float disX = touch1.getTouchPosition ().x - touch2.getTouchPosition ().x;
		float disY = touch1.getTouchPosition ().y - touch2.getTouchPosition ().y;
		
		float dis = Mathf.Sqrt (disX * disX + disY * disY);
		
		return dis;
	}
	
	float FitStroke2Ellipse (Stroke stroke)
	{
		float eccentricity = 0;
		
		List<float> X = new List<float> ();
		List<float> Y = new List<float> ();
		
		if (stroke.getStrokePoints ().Count < 5)
			return 0;
		
		foreach (ScreenTouch screenTouch in stroke.getStrokePoints()) {
			float x = screenTouch.getTouchPosition ().x;
			float y = screenTouch.getTouchPosition ().y;
			
			X.Add (x);
			Y.Add (y);
		}
		
//		int m=360;
//
//		float xcenter=200;
//		float ycenter=200;
//
//		float aInit=100;
//		float bInit=50;
//		float angle=45*Mathf.PI/180;
//
//		for(int i=0;i<m;++i)
//		{
//			float x=aInit*Mathf.Cos(Mathf.PI/180*i)+xcenter;
//			float y=bInit*Mathf.Sin(Mathf.PI/180*i)+ycenter;
//			
//			float xr=(x-xcenter)*Mathf.Cos(angle)-(y-ycenter)*Mathf.Sin(angle)+xcenter;
//			float yr=(x-xcenter)*Mathf.Sin(angle)+(y-ycenter)*Mathf.Cos(angle)+ycenter;
//			X.Add(xr);
//			Y.Add(yr);
//
//		}
		
		
		float mean_x = mean (X);
		float mean_y = mean (Y);
		
		reduceMean (X, mean_x);
		reduceMean (Y, mean_y);
		
//		double [,] D = new double[m, 5];
		
		double [,] D = new double[stroke.getStrokePoints ().Count, 5];
		
		for (int i=0; i<D.GetLength(0); ++i) {
			D [i, 0] = (double)(X [i] * X [i]);
			D [i, 1] = (double)(X [i] * Y [i]);
			D [i, 2] = (double)(Y [i] * Y [i]);
			D [i, 3] = (double)(X [i]);
			D [i, 4] = (double)(Y [i]);
		}
		
		double [,] sumD = new double[1, 5];
//		for (int j=0; j<sumD.GetLength(1); ++j) {
//			for (int i=0; i<m; ++i) {
//				sumD [0, j] += D [i, j];
//			}
//		}
		for (int j=0; j<sumD.GetLength(1); ++j) {
			for (int i=0; i<stroke.getStrokePoints().Count; ++i) {
				sumD [0, j] += D [i, j];
			}
		}
		
		double [,] S = new double[5, 5];
//		alglib.rmatrixgemm (5, 5, m, 1, D, 0, 0, 1, D, 0, 0, 0, 0, ref S, 0, 0);
		alglib.rmatrixgemm (5, 5, stroke.getStrokePoints ().Count, 1, D, 0, 0, 1, D, 0, 0, 0, 0, ref S, 0, 0);
		
		
		int info;
		alglib.matinvreport report;
		alglib.rmatrixinverse (ref S, out info, out report);
		
		double [,] A = new double[1, 5];
		alglib.rmatrixgemm (1, 5, 5, 1, sumD, 0, 0, 0, S, 0, 0, 0, 0, ref A, 0, 0);
		
		float a = (float)A [0, 0];
		float b = (float)A [0, 1];
		float c = (float)A [0, 2];
		float d = (float)A [0, 3];
		float e = (float)A [0, 4];
		
		float orientation_tolerance = 0.001f;
		
		float orientation_rad;
		float cos_phi;
		float sin_phi;
		
		if (Mathf.Min (Mathf.Abs (b / a), Mathf.Abs (b / c)) > orientation_tolerance) {
			orientation_rad = 0.5f * Mathf.Atan (b / (c - a));
			cos_phi = Mathf.Cos (orientation_rad);
			sin_phi = Mathf.Sin (orientation_rad);
			
			float atemp = a;
			float btemp = b;
			float ctemp = c;
			float dtemp = d;
			float etemp = e;
			
			a = atemp * cos_phi * cos_phi - btemp * cos_phi * sin_phi + ctemp * sin_phi * sin_phi;
			b = 0;
			c = atemp * sin_phi * sin_phi + btemp * cos_phi * sin_phi + ctemp * cos_phi * cos_phi;
			d = dtemp * cos_phi - etemp * sin_phi;
			e = dtemp * sin_phi + etemp * cos_phi;

			float mean_xtemp = mean_x;
			float mean_ytemp = mean_y;

			mean_x = cos_phi * mean_xtemp - sin_phi * mean_ytemp;
			mean_y = sin_phi * mean_xtemp + cos_phi * mean_ytemp;
		} else {
			orientation_rad = 0;
			cos_phi = Mathf.Cos (orientation_rad);
			sin_phi = Mathf.Sin (orientation_rad);
		}
		
		float test = a * c;
		
		if (test > 0) {
			if (a < 0) {
				a = -a;
				c = -c;
				d = -d;
				e = -e;
				
			}

			float X0 = mean_x - d / 2 / a;
			float Y0 = mean_y - e / 2 / c;
				
			float F = 1 + (d * d) / (4 * a) + (e * e) / (4 * c);
			a = Mathf.Sqrt (F / a);
			b = Mathf.Sqrt (F / c);

			float long_axis = 2 * Mathf.Max (a, b);
			float short_axis = 2 * Mathf.Min (a, b);
			
			float powB = Mathf.Pow ((0.5f * short_axis), 2);
			float powA = Mathf.Pow ((0.5f * long_axis), 2);
			
			eccentricity = Mathf.Sqrt (1 - powB / powA);
//			print ("eccentricity " + eccentricity);
			
			float [,] R = new float[,]{
				{cos_phi,sin_phi},
				{-sin_phi,cos_phi}
			};
		}
		
		return eccentricity;
		
	}
	
	void FitStroke2Circle (Stroke stroke1, Stroke stroke2)
	{
		
		List<float> X = new List<float> ();
		List<float> Y = new List<float> ();
		

		
		foreach (ScreenTouch screenTouch in stroke1.getStrokePoints()) {
			float x = screenTouch.getTouchPosition ().x;
			float y = screenTouch.getTouchPosition ().y;
			
			X.Add (x);
			Y.Add (y);
		}
		
		foreach (ScreenTouch screenTouch in stroke2.getStrokePoints()) {
			float x = screenTouch.getTouchPosition ().x;
			float y = screenTouch.getTouchPosition ().y;
			
			X.Add (x);
			Y.Add (y);
		}
		
//		int m =180;
//
//		float xcenter = 200;
//		float ycenter = 200;
//
//		float initialRadius = 100;
//
//		for (int i=0; i<m; ++i) {
//			float x = initialRadius * Mathf.Cos (Mathf.PI / 180 * i) + xcenter;
//			float y = initialRadius * Mathf.Sin (Mathf.PI / 180 * i) + ycenter;
//			
//			
//			X.Add (x);
//			Y.Add (y);
//
//		}
		
		
		float mean_x = mean (X);
		float mean_y = mean (Y);
		
		reduceMean (X, mean_x);
		reduceMean (Y, mean_y);
		
//		double [,] D = new double[m, 3];
		
		double [,] D = new double[stroke1.getStrokePoints ().Count + stroke2.getStrokePoints ().Count, 3];
		
		for (int i=0; i<D.GetLength(0); ++i) {
			D [i, 0] = (double)(X [i] * X [i] + Y [i] * Y [i]);
			D [i, 1] = (double)(X [i]);
			D [i, 2] = (double)(Y [i]);

		}
		
		double [,] sumD = new double[1, 3];
//		for (int j=0; j<sumD.GetLength(1); ++j) {
//			for (int i=0; i<m; ++i) {
//				sumD [0, j] += D [i, j];
//			}
//		}
		for (int j=0; j<sumD.GetLength(1); ++j) {
			for (int i=0; i<stroke1.getStrokePoints().Count+stroke2.getStrokePoints().Count; ++i) {
				sumD [0, j] += D [i, j];
			}
		}
		

		
		double [,] S = new double[3, 3];
//		alglib.rmatrixgemm (3, 3, m, 1, D, 0, 0, 1, D, 0, 0, 0, 0, ref S, 0, 0);
		alglib.rmatrixgemm (3, 3, stroke1.getStrokePoints ().Count + stroke2.getStrokePoints ().Count, 1, D, 0, 0, 1, D, 0, 0, 0, 0, ref S, 0, 0);
		
		
		int info;
		alglib.matinvreport report;
		alglib.rmatrixinverse (ref S, out info, out report);
		
		double [,] A = new double[1, 3];
		alglib.rmatrixgemm (1, 3, 3, 1, sumD, 0, 0, 0, S, 0, 0, 0, 0, ref A, 0, 0);
		
		float a = (float)A [0, 0];
		float b = (float)A [0, 1];
		float c = (float)A [0, 2];
		
		float radius = Mathf.Sqrt (1 / a);
		float X0 = mean_x - b / (2 * a);
		float Y0 = mean_y - c / (2 * a);
		
//		print ("radius" + radius);
		stroke2.setX0 (X0);
		stroke2.setY0 (Y0);
		stroke2.setRadius (radius);
	}
	
	float mean (List<float> vector)
	{
		float mean = 0;
		foreach (float element in vector) {
			mean += element;
		}
		
		
		return mean / vector.Count;
	}
	
	void reduceMean (List<float> vector, float mean)
	{
		for (int i=0; i<vector.Count; ++i) {
			vector [i] -= mean;
		}
	}
	
	
}

