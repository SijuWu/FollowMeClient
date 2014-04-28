using UnityEngine;
using System.Collections;

public class SceneManager : MonoBehaviour
{
	//The corners of the screen mesh
	Vector3[] screenCornerCoordinates = new Vector3[16];
	Vector3[] newScreenCoordinates = new Vector3[16];
	
	//The corners in the local reference
	Vector3[] screenRef = new Vector3[16];
	
	//The width of the screen mesh
	public float screenWidth;
	//The height of the screen mesh
	public float screenHeight;
	
	//The center of the screen
	Vector3 screenCenter ;
	
	//Normalized vector in the direction of the height
	Vector3 vec1 ;
	//Normalized vector in the direction of the width
	Vector3 vec2 ;
	//The third normalized vector
	Vector3 vec3 ;
	//New normalized vector in the direction of the height
	public Vector3 newVec1 ;
	//New normalized vector in the direction of the width
	public Vector3 newVec2 ;
	//The new third normalized vector
	Vector3 newVec3 ;
	
	public void initializePlane ()
	{
//		gameObject.AddComponent ("MeshFilter");
//		gameObject.AddComponent ("MeshRenderer");
//
//		//Set the corners of the plane
//		Mesh mesh = new Mesh ();
//		GetComponent<MeshFilter> ().mesh = mesh;
		
		screenCornerCoordinates [0] = new Vector3 (-453.0f, -401.5f, 952.0f);
		screenCornerCoordinates [1] = new Vector3 (439.4f, -418.7f, 993.0f);
		screenCornerCoordinates [2] = new Vector3 (443.5f, 91.4f, 946.0f);
		screenCornerCoordinates [3] = screenCornerCoordinates [2] + screenCornerCoordinates [0] - screenCornerCoordinates [1];	
		
		//The heightVector and the widthVector
		Vector3 heightVector = screenCornerCoordinates [3] - screenCornerCoordinates [0];
		Vector3 widthVector = screenCornerCoordinates [1] - screenCornerCoordinates [0];
		
//		screenWidth = widthVector.magnitude;
//		screenHeight = heightVector.magnitude;
		screenWidth = 900;
		screenHeight = 900*9/16;;
		
		//Three normalized axis of the screen plane, the original point is the top-left corner
		vec1 = heightVector.normalized;
		vec2 = widthVector.normalized;
		vec3 = Vector3.Cross (vec1, vec2);
	}
	
	public void transformScreenPlane ()
	{
		//Set the screenReference
		//X:width Z:height
		GameObject screenReference = GameObject.Find ("ScreenReference");
		screenReference.transform.position = screenCornerCoordinates [0];
		screenReference.transform.LookAt (screenCornerCoordinates [1], vec3);
		
		//Calculate the coordinates of four corners in the screenReference
		screenRef [0] = screenReference.transform.InverseTransformPoint (screenCornerCoordinates [0]);
		screenRef [1] = screenReference.transform.InverseTransformPoint (screenCornerCoordinates [1]);
		screenRef [2] = screenReference.transform.InverseTransformPoint (screenCornerCoordinates [2]);
		screenRef [3] = screenReference.transform.InverseTransformPoint (screenCornerCoordinates [3]);
		
		//Change the x coordinate to adapt to the reference of left hand
		screenRef[1].x*=-1;
		screenRef[2].x*=-1;
		screenRef[3].x*=-1;
		
		GameObject cube1=GameObject.Find("Cube1");
		GameObject cube2=GameObject.Find("Cube2");
		cube1.transform.position=screenRef[1];
		cube2.transform.position=screenRef[2];
		
		//The heightVector and the widthVector
		Vector3 newHeightVector = screenRef [3] - screenRef [0];
		Vector3 newWidthVector = screenRef [1] - screenRef [0];
		
		//Three normalized axis of the screen plane, the original point is the top-left corner
		newVec1 = newHeightVector.normalized;
		newVec2 = newWidthVector.normalized;
		newVec3 = Vector3.Cross (newVec2, newVec1);
		
		screenRef [4] = screenRef [0] + 10F * newVec3;
		screenRef [5] = screenRef [1] + 10F * newVec3;
		screenRef [6] = screenRef [2] + 10F * newVec3;
		screenRef [7] = screenRef [3] + 10F * newVec3;
		
//		//		Mesh mesh = new Mesh ();
//		GetComponent<MeshFilter> ().mesh = mesh;
//		
//		//Add the corners to the mesh
//		mesh.vertices = new Vector3[] {screenRef [0], screenRef [1], screenRef [2],screenRef [3],screenRef [4], screenRef [5], screenRef [6],screenRef [7]};
//
//		//Set the triangle order
//		mesh.triangles = new int[] {0, 1, 2, 0, 2, 3, 0, 3, 2, 0, 2, 1,
//			0, 7, 4, 0, 4, 7, 0, 3, 7, 0, 7, 3,
//			0, 5, 4, 0, 4, 5, 0, 1, 5, 0, 5, 1,
//			2, 5, 1, 2, 1, 5, 2, 6, 5, 2, 5, 6,
//			2, 7, 6, 2, 6, 7, 2, 3, 7, 2, 7, 3,
//			4, 5, 6, 4, 6, 5, 4, 6, 7, 4, 7, 6,
//		};
//		
//
//		mesh.RecalculateNormals ();
//		mesh.Optimize ();
//	
//		//Set the mesh of the meshFilter
//		MeshFilter meshFilter = gameObject.GetComponent<MeshFilter> ();
//		meshFilter.mesh = mesh;
//		
//		//Set the mesh of the meshCollider
		MeshCollider meshCollider = gameObject.GetComponent<MeshCollider> ();
	}
	
	public void setCameraAndLight ()
	{
		GameObject screenReference = GameObject.Find ("ScreenReference");
		screenCenter = screenRef [0] + newVec1 * screenHeight * 0.5F + newVec2 * screenWidth * 0.5F;
		
		//Set the kinectCamera
		Camera kinectCamera = GameObject.Find ("KinectCamera").camera;
		kinectCamera.transform.position = GetTransformedPosition (new Vector3(0,0,0));
	
		Vector3 direction = GetTransformedPosition (new Vector3 (0, 0, 10));
		Vector3 up = GetTransformedPosition (new Vector3 (0, -10, 0));
//		
//		GameObject sphere1 = GameObject.Find ("Sphere1");
//		GameObject sphere2 = GameObject.Find ("Sphere2");
		
//		sphere1.transform.position = direction;
//		sphere2.transform.position = up;
		
//		kinectCamera.transform.LookAt (sphere1.transform, sphere2.transform.position - kinectCamera.transform.position);
		kinectCamera.transform.LookAt(direction,new Vector3(-1,0,0));

		
		kinectCamera.far = 2000;
		
		//Set the userCamera
		Camera userCamera = GameObject.Find ("UserCamera").camera;
		
		
		userCamera.transform.position = screenCenter + new Vector3 ((float)System.Math.Sin (0) * 1000, (float)System.Math.Cos (0) * 1000, 0);
		userCamera.transform.LookAt (screenCenter,new Vector3(-1,0,0));
//		userCamera.transform.Rotate (0, 0, 270);

		//Set the light
		
		GameObject light1 = GameObject.Find ("Directional light1");
		light1.transform.position = screenCenter + 300.0F * newVec3;
		light1.transform.LookAt (new Vector3 (-235, 25, 200));
		
		GameObject light2 = GameObject.Find ("Directional light2");
		light2.transform.position = screenCenter + 300.0F * newVec3;
		light2.transform.LookAt (new Vector3 (-235, -25, 700));
//		
		GameObject light3 = GameObject.Find ("Directional light3");
		light3.transform.position = screenCenter + 300.0F * newVec3;
		light3.transform.LookAt (new Vector3 (-47, 25, 456));
	}
	
	public void initializeEnvironment ()
	{
		GameObject ground = GameObject.Find ("Ground");
		ground.transform.position = screenCenter;
		//ground.transform.Rotate (new Vector3 (1, 0, 0), 180);
		ground.transform.position += new Vector3 (0, -ground.transform.position.y, 0);
	}
	
	// Use this for initialization
	void Start ()
	{
		//Initialize the screen mesh
		initializePlane ();
		//Get the coordinates of the mesh in the local reference
		transformScreenPlane ();
		//Set the cameras and the light in the environment
		setCameraAndLight ();
		//Initial the environments
		initializeEnvironment ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
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
