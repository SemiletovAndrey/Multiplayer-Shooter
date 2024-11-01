using Fusion;
using UnityEngine;

public class PlayerCameraSetup : NetworkBehaviour
{
    private GameObject _cameraInstance;

    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            _cameraInstance = Camera.main.gameObject;
            _cameraInstance.GetComponent<Camera>().enabled = true;
            _cameraInstance.GetComponent<CameraFollower>().enabled = true;
            _cameraInstance.GetComponent<CameraFollower>().CameraFollow(transform);
        }
    }

    private void OnDestroy()
    {
        if (_cameraInstance != null)
        {
            Destroy(_cameraInstance);
        }
    }
}
