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

    [SerializeField]
    private GameObject lineRenderer1;
    [SerializeField]
    private GameObject lineRenderer2;
    [SerializeField]
    private GameObject lineRenderer3;


    public void OnEnable() 
    {
        m_TrackedImageManager.trackedImagesChanged += OnChanged; 
        //m_TrackedImageManager.enabled = true; 
        textField.text = $"Qr script started"; 
    }

    public void OnDisable() { 
        m_TrackedImageManager.trackedImagesChanged -= OnChanged; 
        //m_TrackedImageManager.enabled = false; 
        textField.text += $"\nQr script stopped"; 
    }

    private void Start()
    {
        OnDisable();
        textField.text = $"Qr script started!!!!!!";
        lineRenderer.enabled = false;
        LineRenderer lr2 = lineRenderer2.GetComponent<LineRenderer>();
        lr2.enabled = false;
        LineRenderer lr1 = lineRenderer1.GetComponent<LineRenderer>();
        lr1.enabled = false;
        LineRenderer lr3 = lineRenderer3.GetComponent<LineRenderer>();
        lr3.enabled = false;

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
            textField2.text = $"{qrCodePositionObjectRot.eulerAngles.y}";
            //Vector3 offsetInGlobalCords = RotateVectorAroundY(offset, qrCodePositionObjectRot.eulerAngles.y);

            sessionOrigin.transform.position = qrCodePositionObject.transform.position + offset;
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

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            // Handle updated event
            Vector3 imagePos = trackedImage.transform.position;
            Vector3 currentPos = indicator.gameObject.transform.position;
            Vector3 differenceVec = currentPos - imagePos;

            //trackedImage.transform.localScale = new Vector3(trackedImage.referenceImage.size.x, 0.005f, -trackedImage.referenceImage.size.y);
            trackedImage.transform.localScale = new Vector3(trackedImage.referenceImage.size.x, trackedImage.referenceImage.size.y, 0.005f);



            string msg = $"There are {m_TrackedImageManager.trackables.count} images being tracked.\n"
                + $"Image: {trackedImage.referenceImage.name} is at " + $"{imagePos}.\n" +
                  $"Indicator pos: {currentPos}\n" +
                  $"Difference vector: {differenceVec}\nDistance: {Vector3.Distance(currentPos, imagePos)}\n" +
                  $"Size {trackedImage.gameObject}";

            if (firstText == "")
            {
                firstText = msg;
                //SetQrCodeRecenterTarget(trackedImage.referenceImage.name, differenceVec);

                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, currentPos);
                lineRenderer.SetPosition(1, imagePos);
                
                LineRenderer lr1 = lineRenderer1.GetComponent<LineRenderer>(); 
                lr1.enabled = true;
                lr1.SetPosition(0, imagePos);
                float dx = differenceVec.x;
                Vector3 dxRepresent = new Vector3(dx, 0, 0) + imagePos;
                lr1.SetPosition(1, dxRepresent);

                LineRenderer lr2 = lineRenderer2.GetComponent<LineRenderer>();
                lr2.enabled = true;
                lr2.SetPosition(0, dxRepresent);
                Vector3 dyRepresent = new Vector3(0, differenceVec.y, 0) + dxRepresent;
                lr2.SetPosition(1, dyRepresent);

                LineRenderer lr3 = lineRenderer3.GetComponent<LineRenderer>();
                lr3.enabled = true;
                lr3.SetPosition(0, dyRepresent);
                lr3.SetPosition(1, new Vector3(0, 0, differenceVec.z) + dyRepresent);

                // LineRenderer lr1 = new GameObject().AddComponent<LineRenderer>();
                // lr1.gameObject.transform.SetParent(transform, false);
                // // just to be sure reset position and rotation as well
                // lr1.gameObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                SetQrCodeRecenterTarget(trackedImage.referenceImage.name, differenceVec);
            }
            textField.text = firstText + "\n\n" + msg;
            //OnDisable();

        }

        //foreach (var removedImage in eventArgs.removed)
        //{
        //    // Handle removed event
        //    textField.text = $"There are {m_TrackedImageManager.trackables.count} images being tracked." + ($"Image: {removedImage.referenceImage.name} is REMOVED");
        //}
    }

}
