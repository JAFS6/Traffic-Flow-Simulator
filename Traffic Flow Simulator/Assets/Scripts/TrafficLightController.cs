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
	private Texture		Texture_Red;
	[SerializeField]
	private Texture		Texture_Orange;
	[SerializeField]
	private Texture		Texture_Green;
	[SerializeField]
	private Texture		Texture_Inactive;
	
	public void Start ()
	{
		setRed ();
	}
	
	public void setRed ()
	{
		
	}
	
	public void setOrange ()
	{
		
	}
	
	public void setGreen ()
	{
		
	}
}
