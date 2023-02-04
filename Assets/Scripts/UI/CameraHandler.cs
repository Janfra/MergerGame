using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private float zoomSpeed = 1f;

    [SerializeField] private float maxHeight;
    [SerializeField] private float minHeight;

    private void Update()
    {
        transform.SetPositionAndRotation(GetMovePosition(), transform.rotation);
    }

    private Vector3 GetMovePosition()
    {
        float horizontalSpeed = Time.deltaTime * speed * Input.GetAxis("Horizontal");
        float verticalSpeed = Time.deltaTime * speed * Input.GetAxis("Vertical");

        Vector3 lateralMove = horizontalSpeed * transform.right;
        Vector3 forwardMove = transform.forward;
        forwardMove.y = 0;
        forwardMove.Normalize();
        forwardMove *= verticalSpeed;

        return lateralMove + forwardMove + transform.position;
    }
}
