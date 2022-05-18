using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MultipleObjectPlacement : MonoBehaviour
{
    public GameObject objectPrefab;
    public ARRaycastManager raycastManager;
    public Camera mainCamera;

    private InteractableObject _currentSelectedObject;

    private float _initialPinchDistance;
    private Vector2 _initialTouchVector;
    private Vector3 _initialObjectScale;
    private Quaternion _initialRotation;

    private void Update()
    {
        bool touchedObject;

        if (Input.touchCount == 1)
        {
            Vector2 touchPosition = Input.GetTouch(0).position;

            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                touchedObject = IsTouchingInteractableObject(touchPosition);

                if (!touchedObject)
                {
                    InstantiateObject(touchPosition);
                }
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved && _currentSelectedObject != null)
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                
                //verifica se está a tocar num plano calculado pela AR
                raycastManager.Raycast(touchPosition, hits, TrackableType.Planes);

                if (hits.Count > 0)
                {
                    _currentSelectedObject.transform.position = hits[0].pose.position;
                }
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                _initialTouchVector = touch1.position - touch0.position;
                _initialPinchDistance = _initialTouchVector.magnitude;

                _initialObjectScale = _currentSelectedObject.transform.localScale;
                _initialRotation = _currentSelectedObject.transform.rotation;
            }
            else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                Vector2 currentVector = touch1.position - touch0.position;
                float currentDistance = currentVector.magnitude;

                CheckScale(currentDistance);
                CheckRotation(currentVector);
            }
        }
    }

    private bool IsTouchingInteractableObject(Vector2 touchPosition)
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(touchPosition);

        Physics.Raycast(ray, out hit);

        if (hit.collider != null && hit.collider.CompareTag("Interactable"))
        {
            UpdateSelectedObject(hit.collider.gameObject.GetComponent<InteractableObject>());
            Debug.Log("Selected object");

            return true;
        }

        return false;
    }

    private void InstantiateObject(Vector2 touchPosition)
    {
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        raycastManager.Raycast(touchPosition, hits, TrackableType.Planes);

        if (hits.Count > 0)
        {
            GameObject spawnedObject = Instantiate(objectPrefab, hits[0].pose.position, hits[0].pose.rotation);
            UpdateSelectedObject(spawnedObject.GetComponent<InteractableObject>());
        }
    }

    private void UpdateSelectedObject(InteractableObject newObject)
    {
        if (_currentSelectedObject != null)
        {
            _currentSelectedObject.Deselect();
        }

        _currentSelectedObject = newObject;
        _currentSelectedObject.Select();
    }

    private void CheckScale(float currentDistance)
    {
        float factor = currentDistance / _initialPinchDistance;
        _currentSelectedObject.transform.localScale = _initialObjectScale * factor;
    }

    private void CheckRotation(Vector2 currentVector)
    {
        float angleOffset = Vector2.Angle(_initialTouchVector, currentVector);

        Vector3 crossProduct = Vector3.Cross(_initialTouchVector, currentVector);

        if (crossProduct.z > 0) //anti clockwise
        {
            _currentSelectedObject.transform.rotation =
                Quaternion.Euler(_initialRotation.eulerAngles - new Vector3(0, angleOffset, 0));
        }
        else if (crossProduct.z < 0) //clockwise
        {
            _currentSelectedObject.transform.rotation =
                Quaternion.Euler(_initialRotation.eulerAngles + new Vector3(0, angleOffset, 0));
        }
    }
}
