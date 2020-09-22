using UnityEngine;
using System.Collections;

public class Pure_FPP_Camera : MonoBehaviour {
	
	[Space(10)]
	[Tooltip("The Transform, which should rotate horizontally with mouse movement - in FPS it should be the Character Controller")]
	 public Transform HorizontalRotatingTransform;
	 [Tooltip("The Transform, which should rotate vertically with mouse movement - in FPS it should be the FPS camera itself")]
	 public Transform VerticalRotatingTransform;
	 [Tooltip("Horizontal aiming sensitivity")]
	 public float HorizontalSensitivity;
	 [Tooltip("Vertical aiming sensitivity")]
	 public float VerticalSensitivity;
	[Tooltip("How far can the view be moved vertically up and down?")]
	public float MaxVerticalAngle = 90;
	[Tooltip("How far can the view be moved vertically up and down?")]
	public float MinVerticalAngle = -90;
	 [Tooltip("Input Axis for horizontal aiming")]
	 public string HorizontalInput = "Mouse X";
	 [Tooltip("Input Axis for vertical aiming")]
	 public string VerticalInput = "Mouse Y";
	 
	 private Vector3 TheHorizontalVector; //Vector of rotation applied to HorizontalRotatingTransform
	 private Vector3 TheVerticalVector; //Vector of rotation applied to HorizontalRotatingTransform
	 
	 void Update () 
	 {
		if (HorizontalRotatingTransform != null) 
		{
			TheHorizontalVector.y += Input.GetAxis (HorizontalInput) * HorizontalSensitivity ;
			TheVerticalVector.x = Mathf.Clamp(TheVerticalVector.x, -360, 360);
		}
		if (VerticalRotatingTransform != null) 
		{
			TheVerticalVector.x += -Input.GetAxis (VerticalInput) * VerticalSensitivity ;
			TheVerticalVector.x = Mathf.Clamp(TheVerticalVector.x, MinVerticalAngle, MaxVerticalAngle);
		}
		

		HorizontalRotatingTransform.localRotation = Quaternion.Euler(TheHorizontalVector.x, TheHorizontalVector.y, TheHorizontalVector.z);
		VerticalRotatingTransform.localRotation = Quaternion.Euler (TheVerticalVector.x, TheVerticalVector.y, TheVerticalVector.z);


	 }
}