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
	
	private TrafficLightStatus status;
	
	private float		timeBeforeFirstGreen= 2f;	// Time in seconds that the traffic light will stay red before change to green for first time.
	private float		timeGreen			= 5f;	// Time in seconds that the traffic light will stay green.
	private float		timeOrange			= 2f;	// Time in seconds that the traffic light will stay orange.
	private float		timeRed				= 15f;	// Time in seconds that the traffic light will stay red.
	
	private bool start   = false;
	private bool cycling = false;
	private bool reset   = false;
	private bool firstStartCycleCall = true;
	private float time;
	
	#region Getters and Setters
	public float getTimeBeforeFirstGreen ()
	{
		return this.timeBeforeFirstGreen;
	}
	
	public void setTimeBeforeFirstGreen (float value)
	{
		this.timeBeforeFirstGreen = value;
	}
	
	public float getTimeGreen ()
	{
		return this.timeGreen;
	}
	
	public void setTimeGreen (float value)
	{
		this.timeGreen = value;
	}

	public float getTimeOrange ()
	{
		return this.timeOrange;
	}
	
	public void setTimeOrange (float value)
	{
		this.timeOrange = value;
	}

	public float getTimeRed ()
	{
		return this.timeRed;
	}
	
	public void setTimeRed (float value)
	{
		this.timeRed = value;
	}
	#endregion
	
	public void Start ()
	{
		setRed();
	}
	
	public void startCycle ()
	{
		start = true;
		time = Time.time;
		
		if (firstStartCycleCall)
		{
			firstStartCycleCall = false;
		}
		else
		{
			reset = true;
		}
	}
	
	public void Update ()
	{
		float now = Time.time;
		
		if (reset)
		{
			setRed();
			reset = false;
			cycling = false;
		}
		
		if (start)
		{
			if (!cycling) // Time before first green
			{
				if (now - time >= timeBeforeFirstGreen)
				{
					setGreen();
					cycling = true;
					time = now;
				}
			}
			else
			{
				if (status == TrafficLightStatus.Green && now - time >= timeGreen)
				{
					setOrange();
					time = now;
				}
				else if (status == TrafficLightStatus.Orange && now - time >= timeOrange)
				{
					setRed();
					time = now;
				}
				else if (status == TrafficLightStatus.Red && now - time >= timeRed)
				{
					setGreen();
					time = now;
				}
			}
		}
	}
	
	#region Light Status
	public TrafficLightStatus getTrafficLightStatus ()
	{
		return this.status;
	}
	
	private void setRed ()
	{
		status = TrafficLightStatus.Red;
		Light_Red.GetComponent<MeshRenderer>().material 	= Material_Red;
		Light_Orange.GetComponent<MeshRenderer>().material 	= Material_Inactive;
		Light_Green.GetComponent<MeshRenderer>().material 	= Material_Inactive;
	}
	
	private void setOrange ()
	{
		status = TrafficLightStatus.Orange;
		Light_Red.GetComponent<MeshRenderer>().material 	= Material_Inactive;
		Light_Orange.GetComponent<MeshRenderer>().material 	= Material_Orange;
		Light_Green.GetComponent<MeshRenderer>().material 	= Material_Inactive;
	}
	
	private void setGreen ()
	{
		status = TrafficLightStatus.Green;
		Light_Red.GetComponent<MeshRenderer>().material 	= Material_Inactive;
		Light_Orange.GetComponent<MeshRenderer>().material 	= Material_Inactive;
		Light_Green.GetComponent<MeshRenderer>().material 	= Material_Green;
	}
	#endregion
}
