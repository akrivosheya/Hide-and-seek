using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.2f;

    private Vector3 _velocity = Vector3.zero;

    void LateUpdate()
    {
        if(target == null)
        {
            return;
        }
        Vector3 targetPosition = new Vector3(
        target.position.x, transform.position.y, target.position.z);

        transform.position = Vector3.SmoothDamp(transform.position,
        targetPosition, ref _velocity, smoothTime);
    }
}
