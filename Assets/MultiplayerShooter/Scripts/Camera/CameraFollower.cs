using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] private float Speed = 0.01f;

    private Transform _followerTransform;

    void LateUpdate()
    {
        var cameraPosition = transform.position;
        cameraPosition.x = Mathf.Lerp(cameraPosition.x, _followerTransform.position.x, Speed);
        cameraPosition.y = Mathf.Lerp(cameraPosition.y, _followerTransform.position.y, Speed);
        transform.position = cameraPosition;
    }

    public void CameraFollow(Transform transform)
    {
        _followerTransform = transform;
    }
}
