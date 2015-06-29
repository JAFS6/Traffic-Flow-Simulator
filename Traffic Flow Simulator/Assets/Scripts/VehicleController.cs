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
	private  float 			vehicleLength = 2f; // Lenght of this vehicle
	[SerializeField]
	private bool debug_stop = false;

	// Control variables of the vehicle
	private float 		current_speed; 				// Current speed in meters per second
	private GameObject 	target; 					// Current target GuideNode
	private float 		target_distance;			// Last distance to target
	private bool 		obstacle_detected = false;
	public bool			crashed = false;
	public float		crashTime;
	
	// Sensors (raycasting)
	private const float sensor_lenght = 10f;
	
	private float maxSpeedAllowed;
	private int vehicles_layer_mask = 1 << LayerMask.NameToLayer(Constants.Layer_Vehicles);
	
	void Start ()
	{
		this.current_speed = 0;
	}
	
	void Update ()
	{	
		if (outOfBounds())
		{
			Debug.LogWarning(vehicle_type.ToString()+" has reached map border.");
			destroyVehicle ();
		}
		else if (this.crashed)
		{
			float now = Time.time;
			
			if (now - this.crashTime >= 5)
			{
				this.destroyVehicle();
			}
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
				RaycastHit collision_ray_hit;
				/*
				Check if there are any object (vehicle) on vehicle's layer between this vehicle and the target
				guide node at a distance from this vehicle lower than sensor_lenght.
				If the target is closer than sensor_lenght, check the next target if it exists. Only for continuation nodes.
				The intersections will be regulated by semaphores.
				*/
				this.obstacle_detected = false;
				float distanceToObstacle = distanceToNextVehicle (sensor_lenght, this.transform.position, target, out collision_ray_hit);
				
				if (distanceToObstacle < sensor_lenght)
				{
					Debug.DrawLine(this.transform.position + (Vector3.up * 0.1f), collision_ray_hit.point, Color.yellow);
					
					this.obstacle_detected = true;
					this.maxSpeedAllowed = collision_ray_hit.transform.gameObject.GetComponent<VehicleController>().getCurrentSpeed();
					
					
					if (distanceToObstacle > 1)
					{
						this.maxSpeedAllowed = 0.2f;
					}
					else
					{
						this.maxSpeedAllowed = 0f;
					}
				}
				else
				{
					this.maxSpeedAllowed = Constants.urban_speed_limit;
				}
				
				if (!this.obstacle_detected)
				{
					// Increase speed if the current speed is under the max speed
					if (this.current_speed < this.maxSpeedAllowed)
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
					if (this.current_speed > this.maxSpeedAllowed)
					{
						this.current_speed -= 10 * this.acceleration * Time.deltaTime;
						
						if (this.current_speed < 0)
						{
							this.current_speed = 0;
						}
					}
				}
				
				if (!debug_stop && !this.crashed)
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
			float crashTime_aux = Time.time;
			other.gameObject.GetComponent<VehicleController>().crashed = true;
			other.gameObject.GetComponent<VehicleController>().current_speed = 0f;
			other.gameObject.GetComponent<VehicleController>().crashTime = crashTime_aux;
			this.crashed = true;
			this.current_speed = 0f;
			this.crashTime = crashTime_aux;
		}
	}
	/*
	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == Constants.Tag_Vehicle)
		{
			float crashTime_aux = Time.time;
			collision.gameObject.GetComponent<VehicleController>().crashed = true;
			collision.gameObject.GetComponent<VehicleController>().current_speed = 0f;
			collision.gameObject.GetComponent<VehicleController>().crashTime = crashTime_aux;
			this.crashed = true;
			this.current_speed = 0f;
			this.crashTime = crashTime_aux;
		}
	}*/
	
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
	 * @brief Gets the lenght of the vehicle.
	 * @return The vehicle's lenght.
	 */
	public float getVehicleLength ()
	{
		return this.vehicleLength;
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
	
	private float distanceToNextVehicle (float sensorLenght, Vector3 position, GameObject TargetGuideNode, out RaycastHit hit)
	{
		float min_distanceToObstacle = Constants.infinite;
		
		RaycastHit collision_ray_hit;
		Vector3 p1 = new Vector3(position.x, 0.2f, position.z);
		Vector3 p2 = new Vector3(TargetGuideNode.transform.position.x, 0.2f, TargetGuideNode.transform.position.z);
		
		bool lineCastHit = Physics.Linecast (p1, p2, out collision_ray_hit, vehicles_layer_mask);
		hit = collision_ray_hit;
		
		if (lineCastHit && collision_ray_hit.transform.tag == Constants.Tag_Vehicle)
		{
			min_distanceToObstacle = collision_ray_hit.distance - (this.vehicleLength / 2);
			hit = collision_ray_hit;
		}
		else
		{
			float PositionToTargetGuideNodeDist = MyMathClass.Distance(position, TargetGuideNode.transform.position);
			
			if (sensorLenght > PositionToTargetGuideNodeDist)
			{
				float remainingSensorLenght = sensorLenght - PositionToTargetGuideNodeDist;
				List<GameObject> nextGuideNodes = TargetGuideNode.GetComponent<GuideNode>().getNextGuideNodes();
				
				foreach (GameObject guideNode in nextGuideNodes)
				{
					RaycastHit collision_ray_hit2;
					float distanceToObstacle = PositionToTargetGuideNodeDist + distanceToNextVehicle (remainingSensorLenght, TargetGuideNode.transform.position, guideNode, out collision_ray_hit2);
					
					if (distanceToObstacle < min_distanceToObstacle)
					{
						min_distanceToObstacle = distanceToObstacle;
						hit = collision_ray_hit2;
					}
				}
			}
		}
		return min_distanceToObstacle;
	}
}
