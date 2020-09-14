using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerializedTransform
{
    public float[] _position = new float[3];
    public float[] _rotation = new float[4];
    public float[] _scale = new float[3];

    public void SetValue(Transform transform)
    {
        _position[0] = transform.position.x;
        _position[1] = transform.position.y;
        _position[2] = transform.position.z;

        _rotation[0] = transform.rotation.x;
        _rotation[1] = transform.rotation.y;
        _rotation[2] = transform.rotation.z;
        _rotation[3] = transform.rotation.w;

        _scale[0] = transform.localScale.x;
        _scale[1] = transform.localScale.y;
        _scale[2] = transform.localScale.z;
    }
}

