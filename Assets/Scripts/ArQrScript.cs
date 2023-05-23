using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ArQrScript : MonoBehaviour
{
    [SerializeField]
    ARTrackedImageManager m_TrackedImageManager;
    [SerializeField]
    Text textField;
    [SerializeField]
    private GameObject indicator;


    public void OnEnable() {m_TrackedImageManager.trackedImagesChanged += OnChanged; textField.text = $"Qr script started"; }

    public void OnDisable() { m_TrackedImageManager.trackedImagesChanged -= OnChanged; textField.text = $"Qr script stopped"; }

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in m_TrackedImageManager.trackables)
        {
            Vector3 imagePos = trackedImage.transform.position;
            Vector3 currentPos = indicator.gameObject.transform.position;
            Vector3 differenceVec = currentPos- imagePos;
            textField.text = $"There are {m_TrackedImageManager.trackables.count} images being tracked.\n" + ($"Image: {trackedImage.referenceImage.name} is at " +
                  $"{imagePos}.\nIndicator pos: {currentPos}\nDifference vector: {differenceVec}\nDistance: {Vector3.Distance(currentPos, imagePos)}\nAngle: {Vector3.Angle(currentPos, imagePos)}");
        }

        //foreach (var newImage in eventArgs.added)
        //{
        //    // Handle added event
        //    textField.text = $"There are {m_TrackedImageManager.trackables.count} images being tracked.\n" + ($"Image: {newImage.referenceImage.name} is NEW at " +
        //          $"{newImage.transform.position}");
        //}

        //foreach (var updatedImage in eventArgs.updated)
        //{
        //    // Handle updated event
        //    textField.text = $"There are {m_TrackedImageManager.trackables.count} images being tracked." + ($"Image: {updatedImage.referenceImage.name} is UPDATED at " +
        //          $"{updatedImage.transform.position}");
        //}

        //foreach (var removedImage in eventArgs.removed)
        //{
        //    // Handle removed event
        //    textField.text = $"There are {m_TrackedImageManager.trackables.count} images being tracked." + ($"Image: {removedImage.referenceImage.name} is REMOVED");
        //}
    }
}
