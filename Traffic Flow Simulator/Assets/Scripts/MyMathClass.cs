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
	 * @brief Calcula la distancia minima entre dos puntos del espacio bidimensional
	 * @param[in] p1 Un vector (x,y) con las coordenadas del primer punto
	 * @param[in] p2 Un vector (x,y) con las coordenadas del segundo punto
	 * @return La distancia minima entre los dos puntos
	 */
	public static float Distance (Vector2 p1, Vector2 p2) {
		float dx = p2.x - p1.x;
		float dy = p2.y - p1.y;
		
		float dx2 = dx * dx;
		float dy2 = dy * dy;
		
		float d = Mathf.Sqrt (dx2 + dy2);
		
		return d;
	}
	
	/**
	 * @brief Calcula el modulo de un vector de tres dimensiones
	 * @param[in] v Un vector de tres dimensiones
	 * @return El modulo del vector v
	 */
	public static float module (Vector3 v) {
		return Mathf.Sqrt (v.x * v.x + v.y * v.y + v.z * v.z);
	}
	
	/**
	 * @brief Calcula el modulo de un vector de dos dimensiones
	 * @param[in] v Un vector de dos dimensiones
	 * @return El modulo del vector v
	 */
	public static float module (Vector2 v) {
		return Mathf.Sqrt (v.x * v.x + v.y * v.y);
	}
	
	/**
	 * @brief Calcula el punto medio entre dos puntos del espacio tridimensional
	 * @param[in] p1 Un vector (x,y,z) con las coordenadas del primer punto
	 * @param[in] p2 Un vector (x,y,z) con las coordenadas del segundo punto
	 * @return Un vector (x,y,z) con las coordenadas del punto medio
	 */
	public static Vector3 middlePoint (Vector3 p1, Vector3 p2) {
		Vector3 middle_point = new Vector3();
		middle_point.x = (p1.x + p2.x) / 2;
		middle_point.y = (p1.y + p2.y) / 2;
		middle_point.z = (p1.z + p2.z) / 2;
		return middle_point;
	}
	
	/**
	 * @brief Calcula el punto medio entre dos puntos del espacio bidimensional
	 * @param[in] p1 Un vector (x,y) con las coordenadas del primer punto
	 * @param[in] p2 Un vector (x,y) con las coordenadas del segundo punto
	 * @return Un vector (x,y) con las coordenadas del punto medio
	 */
	public static Vector2 middlePoint (Vector2 p1, Vector2 p2) {
		Vector2 middle_point = new Vector2();
		middle_point.x = (p1.x + p2.x) / 2;
		middle_point.y = (p1.y + p2.y) / 2;
		return middle_point;
	}
	
	/**
	 * @brief Calcula el vector orientacion con origen en p1 y extremo en p2
	 * @param[in] p1 Un vector (x,y,z) con las coordenadas del primer punto
	 * @param[in] p2 Un vector (x,y,z) con las coordenadas del segundo punto
	 * @return El vector (x,y,z) calculado
	 */
	public static Vector3 orientationVector (Vector3 p1, Vector3 p2) {
		Vector3 orientation = new Vector3();
		orientation.x = p2.x - p1.x;
		orientation.y = p2.y - p1.y;
		orientation.z = p2.z - p1.z;
		return orientation;
	}
	
	/**
	 * @brief Calcula el vector orientacion con origen en p1 y extremo en p2
	 * @param[in] p1 Un vector (x,y) con las coordenadas del primer punto
	 * @param[in] p2 Un vector (x,y) con las coordenadas del segundo punto
	 * @return El vector (x,y) calculado
	 */
	public static Vector2 orientationVector (Vector2 p1, Vector2 p2) {
		Vector2 orientation = new Vector2();
		orientation.x = p2.x - p1.x;
		orientation.y = p2.y - p1.y;
		return orientation;
	}
	
	/**
	 * @brief Calcula el angulo en grados que hay que girar el vector 
	 * v1 para ponerlo en la direccion y sentido del vector v2
	 * @param[in] v1 El primer vector
	 * @param[in] v2 El segundo vector
	 * @return El angulo calculado en grados [-180,180]
	 */
	public static float RotationAngle (Vector2 v1, Vector2 v2) {
		
		float v1_theta = PolarAngle (v1);
		float v2_theta = PolarAngle (v2);

		float angle = Mathf.Abs(v2_theta - v1_theta);

		if (v2_theta > v1_theta) {
			angle = -angle;
		}

		return angle;
	}
	
	/**
	 * @brief Calcula el angulo (en grados) de las coordenadas polares del vector pasado como argumento
	 * @param[in] v El vector
	 * @return El angulo calculado en grados [0,360)
	 */
	public static float PolarAngle (Vector2 v) {

		if (v.x == 0 && v.y > 0) {
			return 90f;
		}

		if (v.x == 0 && v.y < 0) {
			return 270f;
		}

		float angle_rad = 0;

		if (v.x > 0 && v.y >= 0) {
			angle_rad = Mathf.Atan (v.y / v.x);
		}
		else if (v.x > 0 && v.y < 0) {
			angle_rad = Mathf.Atan (v.y / v.x) + 2*Mathf.PI;
		}
		else if (v.x < 0) {
			angle_rad = Mathf.Atan (v.y / v.x) + Mathf.PI;
		}

		float angle_deg = ((angle_rad * 180f) / Mathf.PI);
		
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
