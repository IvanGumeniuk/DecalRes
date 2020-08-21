using System;
using System.Collections.Generic;
using UnityEngine;

public static class DataHelper
{
    //Vector3 to array
    public static float[] ToArray(this Vector3 vector)
    {
        return new float[] { vector.x, vector.y, vector.z };
    }

    //Vector2 to array
    public static float[] ToArray(this Vector2 vector)
    {
        return new float[] { vector.x, vector.y };
    }

    //Quaternion to array
    public static float[] ToArray(this Quaternion vector)
    {
        return new float[] { vector.x, vector.y, vector.z, vector.w };
    }

    //Color to array
    public static float[] ToArray(this Color color)
    {
        return new float[] { color.r, color.g, color.b, color.a };
    }

    //Object to Vector3
    public static Vector3 ToVector3(this object vector)
    {
        List<float> arr = (vector as List<object>).ConvertAll(x => (float)Convert.ToDouble(x));
        return new Vector3(arr[0], arr[1], arr[2]);
    }

    public static Vector3 ToVector3(this float[] array)
    {
        if (array.Length < 3)
            return Vector3.zero;

        return new Vector3(array[0], array[1], array[2]);
    }

    //Object to Vector2
    public static Vector2 ToVector2(this object vector)
    {
        List<float> arr = (vector as List<object>).ConvertAll(x => (float)Convert.ToDouble(x));
        return new Vector3(arr[0], arr[1]);
    }

    public static Vector2 ToVector2(this float[] array)
	{
        if (array.Length < 2)
            return Vector2.zero;

        return new Vector2(array[0], array[1]);
	}

    //Object to Quaternion
    public static Quaternion ToQuaternion(this object vector)
    {
        List<float> arr = (vector as List<object>).ConvertAll(x => (float)Convert.ToDouble(x));
        return new Quaternion(arr[0], arr[1], arr[2], arr[3]);
    }

    public static Quaternion ToQuaternion(this float[] array)
    {
        if (array.Length < 4)
            return Quaternion.identity;

        return new Quaternion(array[0], array[1], array[2], array[3]);
    }

    //Object to Color
    public static Color ToColor(this object color)
    {
        List<float> arr = (color as List<object>).ConvertAll(x => (float)Convert.ToDouble(x));
        return new Color(arr[0], arr[1], arr[2], arr[3]);
    }
    public static Color ToColor(this float[] array)
    {
        if (array.Length < 4)
            return Color.white;

        return new Color(array[0], array[1], array[2], array[3]);
    }
}