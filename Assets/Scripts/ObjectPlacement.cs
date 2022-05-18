using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ObjectPlacement : MonoBehaviour
{
    public GameObject objectPrefab;
    public ARRaycastManager arRaycastManager;

    private GameObject _spawnedObject;
    private float _initialScale;
    private float _initialPinchDistance;
    private Vector3 _initialTouchVector;
    private Vector3 _initialObjectScale;
    private Quaternion _initialRotation;
    
    // Update is called once per frame
    private void Update()
    {
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Debug.Log("touched screen");
            
            Vector2 touchPosition = Input.GetTouch(0).position;
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            arRaycastManager.Raycast(touchPosition, hits, TrackableType.Planes);

            if (hits.Count > 0)
            {
                if (_spawnedObject == null)
                {
                    _spawnedObject = Instantiate(objectPrefab, hits[0].pose.position, hits[0].pose.rotation);
                }
                else
                {
                    _spawnedObject.transform.SetPositionAndRotation(hits[0].pose.position, hits[0].pose.rotation);
                }
            }
        }
        else if (Input.touchCount == 2) //is scaling or rotating
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                _initialTouchVector = touch1.position - touch0.position;
                _initialPinchDistance = _initialTouchVector.magnitude;

                _initialObjectScale = _spawnedObject.transform.localScale;
                _initialRotation = _spawnedObject.transform.rotation;
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
    
    private void CheckScale(float currentDistance) //verifica a distancia entre os dois toques
    {
        float factor = currentDistance / _initialPinchDistance;

        Debug.Log("Scaling: " + factor);

        _spawnedObject.transform.localScale = _initialObjectScale * factor;
    }

    private void CheckRotation(Vector2 currentVector) //verifica o angulo entre os dois toques 
    {
        float angleOffset = Vector2.Angle(_initialTouchVector, currentVector);

        Debug.Log("Rotating: " + angleOffset);

        Vector3 crossProduct = Vector3.Cross(_initialTouchVector, currentVector);

        if (crossProduct.z > 0) //anti clockwise
        {
            _spawnedObject.transform.rotation = Quaternion.Euler(_initialRotation.eulerAngles
                                                                 - new Vector3(0, angleOffset, 0));
        }
        else if (crossProduct.z < 0) //clockwise
        {
            _spawnedObject.transform.rotation = Quaternion.Euler(_initialRotation.eulerAngles
                                                                 + new Vector3(0, angleOffset, 0));
        }
    }
    
}
