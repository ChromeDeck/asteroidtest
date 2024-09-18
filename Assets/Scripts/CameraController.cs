using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 5.0f;
    private float _yaw = 0.0f;
    private float _pitch = 0.0f;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * _rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * _rotationSpeed;

        _yaw += mouseX;
        _pitch -= mouseY;

        _pitch = Mathf.Clamp(_pitch, -80f, 80f);

        transform.eulerAngles = new Vector3(_pitch, _yaw, 0.0f);
    }
}