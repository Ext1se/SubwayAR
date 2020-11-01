using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOffset : MonoBehaviour
{
    public Vector3 myOffset;

    void LateUpdate()
    {
        transform.localPosition += myOffset;
    }
}
