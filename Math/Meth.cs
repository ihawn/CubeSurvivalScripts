using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Meth
{
    public static float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f)
    {
        float u, v, S;

        do
        {
            u = 2.0f * UnityEngine.Random.value - 1.0f;
            v = 2.0f * UnityEngine.Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        // Standard Normal Distribution
        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

        // Normal Distribution centered between the min and max value
        // and clamped following the "three-sigma rule"
        float mean = (minValue + maxValue) / 2.0f;
        float sigma = (maxValue - mean) / 3.0f;
        return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }

    public static Vector3 RandomPointOnSphere(float radius = 1f, float _x = 0f, float _y = 0f, float _z = 0f)
    {
        float x = RandomGaussian(-1f, 1f);
        float y = RandomGaussian(-1f, 1f);
        float z = RandomGaussian(-1f, 1f);

        x *= radius / (Mathf.Sqrt(x * x + y * y + z * z));
        y *= radius / (Mathf.Sqrt(x * x + y * y + z * z));
        z *= radius / (Mathf.Sqrt(x * x + y * y + z * z));

        Vector3 center = new Vector3(_x, _y, _z);

        return new Vector3(x, y, z) + center;
    }

    public static Vector2 MaxFromList(List<float> lst)
    {
        float max = -Mathf.Infinity;
        int arg = 0;

        for(int i = 0; i < lst.Count; i++)
        {
            if (lst[i] > max)
            {
                max = lst[i];
                arg = i;
            }
                
        }

        return new Vector2(max, arg);
    }
}
