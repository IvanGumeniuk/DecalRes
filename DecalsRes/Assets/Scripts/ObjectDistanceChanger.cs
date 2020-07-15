using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDistanceChanger : MonoBehaviour
{
    [SerializeField] private float distanceChangingSpeed = 0.1f;
    [SerializeField] private Vector2 distanceClamp;

    public Transform target;

    // Update is called once per frame
    void Update()
    {
		if (!Input.GetKey(KeyCode.LeftAlt) && !Input.GetKey(KeyCode.LeftShift))
        {
            float input = Input.mouseScrollDelta.y;

            float z = Mathf.Lerp(target.localPosition.z, target.localPosition.z + input * distanceChangingSpeed, Time.deltaTime);
            z = Mathf.Clamp(z, distanceClamp.x, distanceClamp.y);

            target.localPosition = new Vector3(target.localPosition.x, target.localPosition.y, z);
        }
    }
}
