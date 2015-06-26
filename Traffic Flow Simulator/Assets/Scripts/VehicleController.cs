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

public class VehicleController : MonoBehaviour
{
	public enum VehicleType : byte {Car, Bus, Truck, Taxi};
	
	// Dependent variables of each vehicle
	[SerializeField]
	private  VehicleType 	vehicle_type;		// Vehicle type
	[SerializeField]
	private  TransportType 	transport_type;		// Transport type
	[SerializeField]
	private  float 			acceleration = 0.1f;// Vehicle acceleration
	[SerializeField]
	private bool debug_stop = false;

	// Control variables of the vehicle
	private float 		current_speed; 				// Current speed in meters per second
	private GameObject 	target; 					// Current target GuideNode
	private float 		target_distance;			// Last distance to target
	private bool 		obstacle_detected = false;
	
	// Sensors (raycasting)
	private const float sensor_length = 10f; // Sensor range
	private Vector3 collision_ray_pos;
	private Vector3 collision_ray_dir;
	private RaycastHit collision_ray_hit;
	private GameObject FrontDetection;
	
	private int vehicles_layer_mask = 1 << LayerMask.NameToLayer(Constants.Layer_Vehicles);
	
	void Start ()
	{
		this.current_speed = 0;
		this.FrontDetection = this.transform.Find("FrontDetection").gameObject;
	}
	
	void Update ()
	{	
		if (outOfBounds())
		{
			Debug.LogWarning(vehicle_type.ToString()+" has reached map border.");
			destroyVehicle ();
		}
		else
		{
			if (onTarget())
			{
				// Choose target randomly between the candidates
				List<GameObject> candidates = selectPosibleNextGuideNodes ();
				GameObject next_target = null;
				
				if (candidates.Count > 0)
				{
					next_target = candidates[Random.Range(0, candidates.Count)];
				}
				
				if (next_target != null)
				{
					target = next_target;
					this.transform.LookAt(target.transform);
					target_distance = MyMathClass.Distance(this.transform.position, target.transform.position);
				}
				else if (target.GetComponent<GuideNode>().getGuideNodeType() == GuideNodeType.Lane_end)
				{
					Debug.LogWarning(vehicle_type.ToString()+" has reached node limit.");
					destroyVehicle ();
				}
				else
				{
					Debug.LogError(vehicle_type.ToString()+" destroyed due to null target. "+
					"Last guideNode "+target.transform.parent.parent.name+"."+target.transform.parent.name+"."+target.name);
					destroyVehicle ();
				}
			}
			
			if (!SimulationUIController.is_paused)
			{
				Debug.DrawLine(FrontDetection.transform.position + new Vector3(0,0.5f,0), 
				              (FrontDetection.transform.position + (this.transform.forward * 6)) + new Vector3(0,0.5f,0), Color.magenta);
				
				// Colission
				Vector3 collision_ray_pos = new Vector3 (FrontDetection.transform.position.x,
				                                         FrontDetection.transform.position.y + 0.1f,
				                                         FrontDetection.transform.position.z);
				Vector3 collision_ray_dir = new Vector3 ();
				collision_ray_dir = Vector3.Normalize (this.transform.forward);
				
				// Collision ray check on Vehicles layer
				this.obstacle_detected = false;
				
				if (Physics.Raycast(collision_ray_pos,collision_ray_dir, out collision_ray_hit,sensor_length,vehicles_layer_mask))
				{
					Debug.DrawLine(collision_ray_pos,collision_ray_hit.point,Color.yellow);
					
					if (collision_ray_hit.transform.tag == Constants.Tag_Vehicle)
					{
						this.obstacle_detected = true;
					}
				}
				
				if (!this.obstacle_detected)
				{
					// Increase speed if the current speed is under the urban speed limit
					if (this.current_speed < Constants.urban_speed_limit)
					{
						this.current_speed += acceleration * Time.deltaTime;
						
						if (this.current_speed > Constants.urban_speed_limit)
						{
							this.current_speed = Constants.urban_speed_limit;
						}
					}
				}
				else
				{
					if (this.current_speed > 0)
					{
						this.current_speed -= (current_speed) * Time.deltaTime;
						
						if (this.current_speed < 0)
						{
							this.current_speed = 0;
						}
					}
				}
				if (!debug_stop)
				{
				// Movement
				Vector3 position = this.transform.position;
				position += this.transform.forward * this.current_speed * Time.deltaTime;
				this.transform.position = position;
				}
				
			} // End if (!SimulationUIController.is_paused)
		}
	} // End void Update
	
	public void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.tag == Constants.Tag_Vehicle)
		{
			other.gameObject.GetComponent<VehicleController>().setCurrentSpeed(0f);
		}
	}
	
	/**
	 * @brief Sets transport type.
	 * @param[in] t The new transport type.
	 */
	public void setTransportType (TransportType t)
	{
		this.transport_type = t;
	}
	
	/**
	 * @brief Gets transport type.
	 * @return The vehicle's transport type.
	 */
	public TransportType getTransportType ()
	{
		return this.transport_type;
	}
	
	/**
	 * @brief Sets the current speed.
	 * @param[in] newSpeed The new current speed.
	 */
	public void setCurrentSpeed (float newSpeed)
	{
		this.current_speed = newSpeed;
	}
	
	/**
	 * @brief Gets the current speed.
	 * @return The vehicle's current speed.
	 */
	public float getCurrentSpeed ()
	{
		return this.current_speed;
	}
	
	/**
	 * @brief Puts the vehicle in the guidenode position and set its target at next guidenode.
	 * @param[in] guidenode The guidenode.
	 */
	public void setGuideNode (GameObject guidenode)
	{
		// Set current position
		Vector3 position = guidenode.transform.position;
		this.transform.position = position;
		// Choose target randomly between the candidates
		List<GameObject> candidates = guidenode.GetComponent<GuideNode>().getNextGuideNodes();
		target = candidates[Random.Range(0,candidates.Count)];
		this.transform.LookAt(target.transform);
		
		target_distance = MyMathClass.Distance(this.transform.position, target.transform.position);
	}
	
	/**
	 * @brief Checks if the vehicle is out of the bounds of the map.
	 * @return True if it is out, false otherwise.
	 */
	private bool outOfBounds ()
	{
		if (this.transform.position.x > RoadMap.max_x) { return true; }
		
		if (this.transform.position.x < RoadMap.min_x) { return true; }
		
		if (this.transform.position.z > RoadMap.max_z) { return true; }
		
		if (this.transform.position.z < RoadMap.min_z) { return true; }
		
		return false;
	}
	
	/**
	 * @brief Checks if the vehicle position is over the Target GuideNode position. Anyway, target_distance is updated.
	 * @return True if yes, False otherwise.
	 */
	private bool onTarget ()
	{
		float newDistance = MyMathClass.Distance(this.transform.position, target.transform.position);
		
		bool isOnTarget = ( newDistance > target_distance );
		
		target_distance = newDistance;
		
		return isOnTarget;
	}
	
	/**
	 * @brief Gets the list of next guide nodes for the current target and returns those ones who match vehicle transport type.
	 * @return The filtered list.
	 */
	private List<GameObject> selectPosibleNextGuideNodes ()
	{
		List<GameObject> candidates = target.GetComponent<GuideNode>().getNextGuideNodes();
		List<GameObject> filtered = new List<GameObject>();
		
		foreach (GameObject candidate in candidates)
		{
			TransportType candidate_tt = candidate.GetComponent<GuideNode>().getGuideNodeTransportType();
			
			if ((this.transport_type == TransportType.Public) || 
			    (this.transport_type == TransportType.Private && (candidate_tt == TransportType.Private || candidate_tt == TransportType.PublicAndPrivate)))
			{
				filtered.Add(candidate);
			}
		}
		return filtered;
	}
	
	private void destroyVehicle ()
	{
		GameObject SimCtrl = GameObject.Find("SimulationController");
		
		if (this.transport_type == TransportType.Public)
		{
			SimCtrl.GetComponent<SimulationController>().publicVehicleDestroyed();
		}
		else
		{
			SimCtrl.GetComponent<SimulationController>().privateVehicleDestroyed();
		}
		Destroy(this.gameObject);
	}
}
