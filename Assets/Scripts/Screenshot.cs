using UnityEngine;
using System.Collections;

public class Screenshot : MonoBehaviour 
{
	public KeyCode button;
	public string name = "screenshot";
	public int multiplier = 4;

	void Update () 
	{
		if(Input.GetKeyDown(button))
		{
			Application.CaptureScreenshot(name + System.DateTime.Now.ToOADate() + ".png", multiplier);
		}
	}
}
