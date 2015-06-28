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
using System.Collections.Generic;

public class GuideNode : MonoBehaviour
{
	private GuideNodeType type; 				// Type of this GuideNode
	private TransportType tt;					// Type of transport which can go throught this GuideNode: Public, Private, PublicAndPrivate
	private List<GameObject> next_GuideNodes;	// The set of next GuideNodes
	
	public GuideNode ()
	{
		next_GuideNodes = new List<GameObject>();
	}
	
	void Update ()
	{
		Color color = Color.yellow;
		
		if (type == GuideNodeType.Lane_start)
		{
			color = Color.green;
		}
		else if (type == GuideNodeType.Lane_end)
		{
			color = Color.red;
		}
		else if (type == GuideNodeType.OnLane)
		{
			color = Color.yellow;
		}
		
		Debug.DrawLine(this.transform.position, 
		               this.transform.position + new Vector3(0,5,0), color);
		
		if (next_GuideNodes.Count > 0)
		{
			foreach (GameObject next in next_GuideNodes)
			{
				Vector3 middle = MyMathClass.middlePoint(this.transform.position, next.transform.position);
			
				Debug.DrawLine(this.transform.position + new Vector3(0,0.2f,0),
				               middle + new Vector3(0,0.2f,0), Color.white);
				
				Debug.DrawLine(middle + new Vector3(0,0.2f,0),
				               next.transform.position + new Vector3(0,0.2f,0), Color.cyan);
			}
		}
	}
	
	public void setGuideNodeType (GuideNodeType new_type)
	{
		type = new_type;
	}
	
	public GuideNodeType getGuideNodeType ()
	{
		return type;
	}
	
	public void setGuideNodeTransportType (TransportType new_tt)
	{
		tt = new_tt;
	}
	
	public TransportType getGuideNodeTransportType ()
	{
		return tt;
	}
	
	public void addNextGuideNode (GameObject n)
	{
		next_GuideNodes.Add(n);
	}
	
	public List<GameObject> getNextGuideNodes ()
	{
		List<GameObject> ret = new List<GameObject>();
		
		foreach (GameObject next in next_GuideNodes)
		{
			ret.Add(next);
		}
		
		return ret;
	}
	
	public int getNextGuideNodesCount ()
	{
		return next_GuideNodes.Count;
	}
}