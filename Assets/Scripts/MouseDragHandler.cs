using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MouseDragHandler
{
    [SerializeField] private LayerMask rayCollision;
    private Camera mainCamera;
    private Vector3 yOffset;

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
    }

    public void OnDragObjectOnTile(Transform _objectTransform)
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, rayCollision))
        {
            _objectTransform.position = hit.point + yOffset;

            GridTile hoveredTile;
            if (hit.collider.TryGetComponent(out hoveredTile))
            {
                hoveredTile.Highlight();
                hoveredTile.StartHighlightTimer();
            }
        }
    }

    #region OptionA

    //private Vector3 mouseOffset;
    //private float objectZPositionOnScreen;
    //private Vector3 mouseZPosition;

    //public MouseDragHandler()
    //{

    //}

    //public void OnMouseClick(Transform _objectTransform)
    //{
    //    objectZPositionOnScreen = Camera.main.WorldToScreenPoint(_objectTransform.position).z;
    //    mouseOffset = _objectTransform.position - GetMouseWorldPosition();
    //}

    //public void DragObject(Transform _objectTransform)
    //{
    //    _objectTransform.SetPositionAndRotation(GetMouseWorldPosition() + mouseOffset + mouseZPosition, _objectTransform.rotation);
    //}

    //private Vector3 GetMouseWorldPosition()
    //{
    //    Vector3 mousePoint = Input.mousePosition;

    //    mousePoint.z = objectZPositionOnScreen;
    //    SetMouseZPosition(mousePoint);
    //    mousePoint.y = 0;

    //    return Camera.main.ScreenToWorldPoint(mousePoint);
    //}

    //private void SetMouseZPosition(Vector3 _mousePoint)
    //{
    //    mouseZPosition.z = Camera.main.ScreenToWorldPoint(_mousePoint).z;

    //    Debug.Log(mouseZPosition);
    //}

    #endregion
}
