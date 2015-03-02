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

public static class MyMathClass : object {

	/**
	 * @brief Gets the minimum distance between two points in three-dimensional space
	 * @param[in] p1 A vector (x,y,z) with the coordinates of the first point
	 * @param[in] p2 A vector (x,y,z) with the coordinates of the second point
	 * @return The minimum distance between the two points
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
	 * @brief Gets the minimum distance between two points in two-dimensional space
	 * @param[in] p1 A vector (x,y) with the coordinates of the first point
	 * @param[in] p2 A vector (x,y) with the coordinates of the second point
	 * @return The minimum distance between the two points
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
	 * @brief Gets the modulus of a vector of three dimensions
	 * @param[in] v A vector of three dimensions
	 * @return The modulus of v
	 */
	public static float module (Vector3 v) {
		return Mathf.Sqrt (v.x * v.x + v.y * v.y + v.z * v.z);
	}
	
	/**
	 * @brief Gets the modulus of a vector of two dimensions
	 * @param[in] v A vector of two dimensions
	 * @return The modulus of v
	 */
	public static float module (Vector2 v) {
		return Mathf.Sqrt (v.x * v.x + v.y * v.y);
	}
	
	/**
	 * @brief Gets the midpoint between two points in three-dimensional space
	 * @param[in] p1 A vector (x,y,z) with the coordinates of the first point
	 * @param[in] p2 A vector (x,y,z) with the coordinates of the second point
	 * @return A vector (x,y,z) with the coordinates of the midpoint
	 */
	public static Vector3 middlePoint (Vector3 p1, Vector3 p2) {
		Vector3 middle_point = new Vector3();
		middle_point.x = (p1.x + p2.x) / 2;
		middle_point.y = (p1.y + p2.y) / 2;
		middle_point.z = (p1.z + p2.z) / 2;
		return middle_point;
	}
	
	/**
	 * @brief Gets the midpoint between two points in two-dimensional space
	 * @param[in] p1 A vector (x,y) with the coordinates of the first point
	 * @param[in] p2 A vector (x,y) with the coordinates of the second point
	 * @return A vector (x,y) with the coordinates of the midpoint
	 */
	public static Vector2 middlePoint (Vector2 p1, Vector2 p2) {
		Vector2 middle_point = new Vector2();
		middle_point.x = (p1.x + p2.x) / 2;
		middle_point.y = (p1.y + p2.y) / 2;
		return middle_point;
	}
	
	/**
	 * @brief Calculates the orientation vector with origin in p1 and end en p2 in three-dimensional space
	 * @param[in] p1 A vector (x,y,z) with the coordinates of the first point
	 * @param[in] p2 A vector (x,y,z) with the coordinates of the second point
	 * @return The calculated vector (x,y,z)
	 */
	public static Vector3 orientationVector (Vector3 p1, Vector3 p2) {
		Vector3 orientation = new Vector3();
		orientation.x = p2.x - p1.x;
		orientation.y = p2.y - p1.y;
		orientation.z = p2.z - p1.z;
		return orientation;
	}
	
	/**
	 * @brief Calculates the orientation vector with origin in p1 and end en p2 in two-dimensional space
	 * @param[in] p1 A vector (x,y) with the coordinates of the first point
	 * @param[in] p2 A vector (x,y) with the coordinates of the second point
	 * @return The calculated vector (x,y)
	 */
	public static Vector2 orientationVector (Vector2 p1, Vector2 p2) {
		Vector2 orientation = new Vector2();
		orientation.x = p2.x - p1.x;
		orientation.y = p2.y - p1.y;
		return orientation;
	}
	
	/**
	 * @brief Calculates the angle in degrees, to turn the vector v1 to put it in the direction and sense of vector v2
	 * @param[in] v1 The first vector
	 * @param[in] v2 The second vector
	 * @return The angle calculated in degrees (-360,360)
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
	 * @brief Rotates a point in the plane about the origin of coordinates
	 * @param[in] p The point to be rotated
	 * @param[in] degrees Degrees to rotate
	 * @return The result of rotating the point
	 */
	public static Vector2 rotatePoint (Vector2 p, float degrees) {
		float radians = (degrees * Mathf.PI) / 180f;
		
		Vector2 rotated_point = new Vector2 ();
		rotated_point.x = (p.x * Mathf.Cos(radians)) - (p.y * Mathf.Sin(radians));
		rotated_point.y = (p.x * Mathf.Sin(radians)) + (p.y * Mathf.Cos(radians));
		
		return rotated_point;
	}
	
	/**
	 * @brief Calculates the angle (in degrees) of the polar coordinates of the vector passed as argument
	 * @param[in] v The vector
	 * @return The angle calculated in degrees [0,360)
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
	 * @brief Converts polar coordinates to Cartesian
	 * @param[in] magnitude The magnitude of the vector
	 * @param[in] polar_angle The angle in degrees of polar coordinates
	 */
	public static Vector2 PolarToCartesian (float magnitude, float polar_angle) {
		Vector2 v = new Vector2 ();
		
		float polar_angle_rad = (polar_angle * Mathf.PI) / 180;
		
		v.x = magnitude * Mathf.Cos (polar_angle_rad);
		v.y = magnitude * Mathf.Sin (polar_angle_rad);
		
		return v;
	}
	
	/**
	 * @brief Calculate the point in the t section of the Bezier curve defined by p1 (start), p2 (end) and p3 (control).
	 * The algorythm has been obtained from http://devmag.org.za/2011/04/05/bzier-curves-a-tutorial/
	 * @param[in] t The value t can range from 0 to 1. The value 0 corresponds to the start point of the curve; 
	 * the value 1 corresponds to the endpoint of the curve. Values in between correspond to other points on the curve.
	 * @param[in] p0 The start point of the curve
	 * @param[in] p1 One control point of the curve
	 * @param[in] p2 Other control point of the curve
	 * @param[in] p3 The end point of the curve
	 * @return The value of the function is a point on the curve; it depends on the parameter t, and on a set of points,
	 * called the control points (p0,p1,p2,p3)
	 */
	private static Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {
		float u = 1f - t;
		float tt = t*t;
		float uu = u*u;
		float uuu = uu * u;
		float ttt = tt * t;
		
		Vector3 p = uuu * p0; //first term
		p += 3 * uu * t * p1; //second term
		p += 3 * u * tt * p2; //third term
		p += ttt * p3;		  //fourth term
		
		return p;
	}
}
