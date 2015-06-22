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
using UnityEngine.UI;
using System.Collections;

public class SliderValue : MonoBehaviour {

	[SerializeField]
	private GameObject text_obj;
	[SerializeField]
	private float multiplier;
	[SerializeField]
	private string append;
	[SerializeField]
	private bool isInt;
	
	void Update ()
	{
		string v_str;
		
		if (!isInt)
		{
			float v = this.GetComponent<Slider>().value * multiplier;
			v_str = v.ToString("0.00");
		}
		else
		{
			int v = Mathf.FloorToInt(this.GetComponent<Slider>().value * multiplier);
			v_str = v.ToString();
		}
		text_obj.GetComponent<Text>().text = v_str + append;
	}
}
