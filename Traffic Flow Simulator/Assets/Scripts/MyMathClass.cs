﻿using UnityEngine;
using System.Collections;

public static class MyMathClass : object {

	/**
	 * @brief Calcula la distancia minima entre dos puntos del espacio tridimensional
	 * @param[in] p1 Un vector (x,y,z) con las coordenadas del primer punto
	 * @param[in] p2 Un vector (x,y,z) con las coordenadas del segundo punto
	 * @return La distancia minima entre los dos puntos
	 */
	public static float Distance (Vector3 p1, Vector3 p2) {
		float dx = p2.x - p1.x;
		float dy = p2.y - p1.y;
		float dz = p2.z - p1.z;
		
		float dx2 = dx * dx;
		float dy2 = dy * dy;
		float dz2 = dz * dz;
		
		float d = Mathf.Sqrt (dx2 + dy2 + dz2);
		
		return d;
	}
	
	/**
	 * @brief Calcula el modulo de un vector de dos dimensiones
	 * @param[in] v Un vector de dos dimensiones
	 * @return El modulo del vector v
	 */
	public static float module2D (Vector2 v) {
		return Mathf.Sqrt (v.x * v.x + v.y * v.y);
	}
	
	/**
	 * @brief Calcula el angulo en grados que hay que girar el vector 
	 * v1 para ponerlo en la direccion y sentido del vector v2
	 * @param[in] v1 El primer vector
	 * @param[in] v2 El segundo vector
	 * @return El angulo calculado en grados [0,360)
	 */
	public static float RotationAngle (Vector2 v1, Vector2 v2) {
		
		float v1_theta = PolarAngle (v1);
		float v2_theta = PolarAngle (v2);
		float angle = v1_theta - v2_theta;
		
		return angle;
	}
	
	/**
	 * @brief Calcula el angulo (en grados) de las coordenadas polares del vector pasado como argumento
	 * @param[in] v El vector
	 * @return El angulo calculado en grados
	 */
	public static float PolarAngle (Vector2 v) {
		float angle_rad = Mathf.Atan2 (v.y, v.x);
		float angle_deg = ((angle_rad * 180f) / Mathf.PI);
		
		if (v.x < 0) {
			if (v.y >= 0) { // Segundo cuadrante
				angle_deg += 180f;
			}
			else if (v.y < 0) { // Tercer cuadrante
				angle_deg -= 180f;
			}
		}
		
		return angle_deg;
	}
	
	/**
	 * @brief Convierte coordenadas polares a cartesianas
	 * @param[in] magnitude La magnitud del vector
	 * @param[in] polar_angle El angulo en grados de las coordenadas polares
	 */
	public static Vector2 PolarToCartesian (float magnitude, float polar_angle) {
		Vector2 v = new Vector2 ();
		
		float polar_angle_rad = (polar_angle * Mathf.PI) / 180;
		
		v.x = magnitude * Mathf.Cos (polar_angle_rad);
		v.y = magnitude * Mathf.Sin (polar_angle_rad);
		
		return v;
	}
}
