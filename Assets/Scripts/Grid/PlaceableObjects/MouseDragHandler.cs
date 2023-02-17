using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MouseDragHandler
{
    /// <summary>
    /// Tile Layer Default
    /// </summary>
    [SerializeField] 
    private LayerMask rayCollision = 1 << 6;
    private Vector3 yOffset;

    private static Camera mainCamera;
    private const float DISTANCE_ON_NOHIT = 10f;

    public MouseDragHandler()
    {
        yOffset = Vector3.zero;
    }
    public void OnAwake()
    {
        mainCamera = Camera.main;
    }
    public void OnAwake(float _yOffset)
    {
        mainCamera = Camera.main;
        yOffset.y = _yOffset;
    }
    public void OnDragObject(Transform _objectTransform)
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, rayCollision))
        {
            _objectTransform.position = hit.point + yOffset;
            Debug.Log($"Hitting: {hit.collider.gameObject.name}");
        }
        else
        {
            // If nothing hit, put it at some point of the ray casted
            _objectTransform.position = ray.GetPoint(DISTANCE_ON_NOHIT);
        }
    }

    public bool OnDragObjectOnTile(Transform _objectTransform)
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, rayCollision))
        {
            _objectTransform.position = hit.point + yOffset;

            if (hit.collider.TryGetComponent(out GridTile hoveredTile))
            {
                hoveredTile.StartHighlightTimer();
            }
            return true;
        }
        return false;
    }
}
