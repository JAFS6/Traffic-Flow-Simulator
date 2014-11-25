using UnityEngine;
using System.Collections;

public class VehicleController : MonoBehaviour {

	private Vector3 direction;
	private float velocity; // Velocidad en metros por segundo

	/**
	 * @brief Establece la direccion del vehiculo
	 * @param[in] d La direccion
	 */
	public void setDirection (Vector3 d) {
		this.direction = new Vector3(d.x,d.y,d.z);
	}

	/**
	 * @brief Establece la velocidad del vehiculo
	 * @param[in] v La velocidad del vehiculo en metros por segundo
	 */
	public void setVelocity (float v) {
		this.velocity = v;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 position = this.transform.position;
		position += this.direction * this.velocity * Time.deltaTime;
		this.transform.position = position;
	}
}
