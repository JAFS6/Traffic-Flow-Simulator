using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Shortcut : MonoBehaviour
{	
	[SerializeField]
	private KeyCode key;
	
	void Update ()
	{
		if (Input.GetKeyDown(key))
		{
			this.GetComponent<Toggle>().isOn = !this.GetComponent<Toggle>().isOn;
		}
	}
}