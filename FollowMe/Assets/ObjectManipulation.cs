using UnityEngine;
using System.Collections;

public class ObjectManipulation : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
//		if (Input.GetMouseButton(0))
//			Translate (1, 1, 1);
//		if(Input.GetMouseButton(1))
//			Rotate (new Vector3(1,0,0),1);
	}
	
	//Translation
	void Translate (float x, float y, float z)
	{
		gameObject.transform.position += new Vector3 (x, y, z);
	}
	
	//Rotation
	void Rotate (Vector3 rotationAxis, float angle)
	{
		gameObject.transform.Rotate (rotationAxis, angle);
	}
	
	//Zoom
	void Zoom (float scaleX, float scaleY, float scaleZ)
	{
		Vector3 initialScale = gameObject.transform.localScale;
		gameObject.transform.localScale = new Vector3 (initialScale.x * scaleX, initialScale.z * scaleY, initialScale.z * scaleZ);
	}
	

}
