using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    Text textField2;
    [SerializeField]
    private GameObject indicator;

    [SerializeField]
    private ARSession session;
    [SerializeField]
    private ARSessionOrigin sessionOrigin;

    string firstText = "";
    [SerializeField]
    LineRenderer lineRenderer;


    public void OnEnable() {m_TrackedImageManager.trackedImagesChanged += OnChanged; textField.text = $"Qr script started"; }

    public void OnDisable() { m_TrackedImageManager.trackedImagesChanged -= OnChanged; textField.text += $"\nQr script stopped"; }

    private void Start()
    {
        OnDisable();
        lineRenderer.enabled = false;
    }

    private Vector3 RotateVectorAroundY(Vector3 vector, float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.up) * vector; // Vector.up represents Y axis
    }

    public void SetQrCodeRecenterTarget(string targetText, Vector3 offset)
    {
        GameObject qrCodePositionObject = GameObject.Find(targetText); //target obj
        if (qrCodePositionObject != null)
        {
            // Reset position and rotation of ARSession
            session.Reset();

            Vector3 qrCodePositionObjectPos = qrCodePositionObject.transform.position;
            Quaternion qrCodePositionObjectRot = qrCodePositionObject.transform.rotation;
            textField2.text = $"{360 - qrCodePositionObjectRot.eulerAngles.y}";
            Vector3 offsetInGlobalCords = RotateVectorAroundY(offset, qrCodePositionObjectRot.eulerAngles.y);

            sessionOrigin.transform.position = qrCodePositionObject.transform.position + offsetInGlobalCords;
            sessionOrigin.transform.rotation = qrCodePositionObject.transform.rotation;
            //// Add offset for recentering - distance to QR
            //sessionOrigin.transform.position = new Vector3(
            //    (float)(gameObjectPos.x + System.Math.Cos(System.Math.PI/4)), 
            //    gameObjectPos.y,
            //    (float)(gameObjectPos.z + System.Math.Cos(System.Math.PI / 4))
            //    );


        }
    }

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in m_TrackedImageManager.trackables)
        {

        }

        //foreach (var newImage in eventArgs.added)
        //{
        //    // Handle added event
        //    textField.text = $"There are {m_TrackedImageManager.trackables.count} images being tracked.\n" + ($"Image: {newImage.referenceImage.name} is NEW at " +
        //          $"{newImage.transform.position}");
        //}

        foreach (var trackedImage in eventArgs.updated)
        {
            // Handle updated event
            Vector3 imagePos = trackedImage.transform.position;
            Vector3 currentPos = indicator.gameObject.transform.position;
            Vector3 differenceVec = currentPos - imagePos;

            

            string msg = $"There are {m_TrackedImageManager.trackables.count} images being tracked.\n"
                + $"Image: {trackedImage.referenceImage.name} is at " + $"{imagePos}.\n" +
                  $"Indicator pos: {currentPos}\n" +
                  $"Difference vector: {differenceVec}\nDistance: {Vector3.Distance(currentPos, imagePos)}\n" +
                  $"Angle: {Vector3.Angle(currentPos, imagePos)}";

            if (firstText == "")
            {
                firstText = msg;
                //SetQrCodeRecenterTarget(trackedImage.referenceImage.name, differenceVec);

                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, currentPos);
                lineRenderer.SetPosition(1, imagePos);
            }
            textField.text = firstText + "\n\n" + msg;
            //OnDisable();
            //SetQrCodeRecenterTarget(trackedImage.referenceImage.name, differenceVec);
        }

        //foreach (var removedImage in eventArgs.removed)
        //{
        //    // Handle removed event
        //    textField.text = $"There are {m_TrackedImageManager.trackables.count} images being tracked." + ($"Image: {removedImage.referenceImage.name} is REMOVED");
        //}
    }

}
