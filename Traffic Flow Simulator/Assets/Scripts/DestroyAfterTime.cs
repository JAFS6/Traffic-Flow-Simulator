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

/*
 * @brief Destroys the GameObject at wich this is attached to after aliveTime seconds
 */
public class DestroyAfterTime : MonoBehaviour
{
	private float aliveTime = 10f; // seconds
	private float startTime;
	
	public void Start ()
	{
		this.startTime = Time.time;
	}
	
	public void Update ()
	{
		float now = Time.time;
		
		if (now - this.startTime > aliveTime)
		{
			Destroy(this.gameObject);
		}
	}
	
	public void setAliveTime (float t)
	{
		aliveTime = t;
	}
}
