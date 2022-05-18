using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MultipleImageTracking : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;
    public List<GameObject> prefabs;

    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();

    private void Awake()
    {
        foreach (GameObject prefab in prefabs)
        {
            GameObject instancedPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            instancedPrefab.SetActive(false);

            instancedPrefab.name = prefab.name;

            spawnedPrefabs.Add(instancedPrefab.name, instancedPrefab);
        }
    }

    private void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += ImageChanged;
    }

    //para sabermos qd a imagem alterou/apareceu ou desapareceu, precisamos de usar o evento
    private void ImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage addedImage in eventArgs.added)
        {
            GameObject obj = spawnedPrefabs[addedImage.referenceImage.name]; //o nome da imagem tem de ser o mesmo do prefab
            obj.SetActive(true);
            obj.transform.SetPositionAndRotation(addedImage.transform.position, addedImage.transform.rotation);

            Debug.Log(addedImage.name);
        }

        foreach (ARTrackedImage updatedImage in eventArgs.updated)
        {
            GameObject obj = spawnedPrefabs[updatedImage.referenceImage.name];

            if (updatedImage.trackingState == TrackingState.Tracking)
            {
                if (!obj.activeSelf)
                {
                    obj.SetActive(true);
                }

                obj.transform.SetPositionAndRotation(updatedImage.transform.position, updatedImage.transform.rotation);
            }
            else
            {
                obj.SetActive(false);
            }
        }
        
        foreach (ARTrackedImage removedImage in eventArgs.removed)
        {
            GameObject obj = spawnedPrefabs[removedImage.referenceImage.name];
            obj.SetActive(false);
        }
    }
    
    private void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= ImageChanged;
    }
}
