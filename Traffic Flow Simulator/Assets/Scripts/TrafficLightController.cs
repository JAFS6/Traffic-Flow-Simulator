/*
	Copyright 2014-2015 Juan Antonio Fajardo Serrano

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at

		http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/
using UnityEngine;
using System.Collections;

public class TrafficLightController : MonoBehaviour
{
	[SerializeField]
	private GameObject 	Light_Red;
	[SerializeField]
	private GameObject 	Light_Orange;
	[SerializeField]
	private GameObject 	Light_Green;
	[SerializeField]
	private Material	Material_Red;
	[SerializeField]
	private Material	Material_Orange;
	[SerializeField]
	private Material	Material_Green;
	[SerializeField]
	private Material	Material_Inactive;
	
	private float		timeGreen 	= 5f;	// Time in seconds that the traffic light will stay green.
	private float		timeOrange = 2f;	// Time in seconds that the traffic light will stay orange.
	private float		timeRed	= 15f;	// Time in seconds that the traffic light will stay red.
	
	float TimeGreen
	{
		get { return this.timeGreen; }
		set { this.timeGreen = value; }
	}

	float TimeOrange
	{
		get { return this.timeOrange; }
		set { this.timeOrange = value; }
	}

	float TimeRed
	{
		get { return this.timeRed; }
		set { this.timeRed = value; }
	}
	
	public void Start ()
	{
		setRed ();
		StartCoroutine(cycle());
	}
	
	private IEnumerator cycle ()
	{
		while (true)
		{
			setRed ();
			yield return new WaitForSeconds(timeRed);
			setGreen ();
			yield return new WaitForSeconds(timeGreen);
			setOrange ();
			yield return new WaitForSeconds(timeOrange);
		}
	}
	
	private void setRed ()
	{
		Light_Red.GetComponent<MeshRenderer>().material 	= Material_Red;
		Light_Orange.GetComponent<MeshRenderer>().material 	= Material_Inactive;
		Light_Green.GetComponent<MeshRenderer>().material 	= Material_Inactive;
	}
	
	private void setOrange ()
	{
		Light_Red.GetComponent<MeshRenderer>().material 	= Material_Inactive;
		Light_Orange.GetComponent<MeshRenderer>().material 	= Material_Orange;
		Light_Green.GetComponent<MeshRenderer>().material 	= Material_Inactive;
	}
	
	private void setGreen ()
	{
		Light_Red.GetComponent<MeshRenderer>().material 	= Material_Inactive;
		Light_Orange.GetComponent<MeshRenderer>().material 	= Material_Inactive;
		Light_Green.GetComponent<MeshRenderer>().material 	= Material_Green;
	}
}
