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
    //[SerializeField]
    //ARTrackedImageManager m_TrackedImageManager;
    [SerializeField]
    XRReferenceImageLibrary imageLibrary;
    [SerializeField]
    GameObject scannedImagePrefab;

    private ARTrackedImageManager m_TrackedImageManager;

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

    public bool plannedRecenter = false;
    public bool doSessionReset = true;

    private Vector3 totalOffset = new Vector3(0, 0, 0);
    private Quaternion totalQuaternion = new Quaternion();

    public void PlanRecenter() { plannedRecenter = true; textField.text += $"\nPlanned recenter"; }

    public void EnableSessionReset() { doSessionReset = true; textField.text += $"\nEnabled Session reset"; }
    public void DisableSessionReset() { doSessionReset = false; textField.text += $"\nDisabled Session reset"; }

    // There were OnEnable and OnDisable were changing .trackedImagesChanged, i think that onEnable happens
    // on start on when we activate this game object amd onDisable wjen we disable this game obj

    //public void EnableScannerActions() 
    //{
    //    m_TrackedImageManager.trackedImagesChanged += OnChanged;
    //    textField.text = $"Action handle enabled";
    //    //m_TrackedImageManager.enabled = true; 
    //}

    //public void DisableScannerActions() { 
    //    m_TrackedImageManager.trackedImagesChanged -= OnChanged; 
    //    textField.text += $"\nAction handle stopped"; 
    //    //m_TrackedImageManager.enabled = false; 
    //}

    private void Start()
    {
        textField.text += $"\nQr script started! Creating manager";
        lineRenderer.enabled = false;
        LineRenderer lr1 = lineRenderer1.GetComponent<LineRenderer>();
        lr1.enabled = false;
        LineRenderer lr2 = lineRenderer2.GetComponent<LineRenderer>();
        lr2.enabled = false;
        LineRenderer lr3 = lineRenderer3.GetComponent<LineRenderer>();
        lr3.enabled = false;

        InitTrackedImageManager();
    }

    public void InitTrackedImageManager()
    {
        m_TrackedImageManager = new GameObject().AddComponent<ARTrackedImageManager>();
        m_TrackedImageManager.referenceLibrary = imageLibrary;
        m_TrackedImageManager.enabled = true;
        m_TrackedImageManager.requestedMaxNumberOfMovingImages = 1;
        m_TrackedImageManager.maxNumberOfMovingImages = 1;
        m_TrackedImageManager.trackedImagePrefab = scannedImagePrefab;
        m_TrackedImageManager.trackedImagesChanged += OnChanged;
        textField.text += $"\nLoaded tracking manager";
    }

    private Vector3 RotateVectorAroundY(Vector3 vector, float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.up) * CreateVectorCopy(vector); // Vector.up represents Y axis
    }

    public void SetQrCodeRecenterTarget(string targetText, Vector3 imagePos, Vector3 currPos, Quaternion yImageRot)
    {
        Vector3 offset = CreateVectorCopy(currPos - imagePos);
        textField.text = $"Offset before {offset}";
        GameObject qrCodePoint = GameObject.Find(targetText);
        if (qrCodePoint != null)
        {
            // Reset position and rotation of ARSession
            if (doSessionReset)
            {
                session.Reset();
            }

            Vector3 qrCodePointPos = CreateVectorCopy(qrCodePoint.transform.position);
            Quaternion qrCodePointRot = CreateQuaternionCopy(qrCodePoint.transform.rotation);
            textField2.text = $"{qrCodePointRot.eulerAngles.y}";
            Vector3 offsetRelativeToNewQr = CreateVectorCopy(RotateVectorAroundY(offset, qrCodePointRot.eulerAngles.y - yImageRot.eulerAngles.y));

            sessionOrigin.transform.position = CreateVectorCopy(qrCodePointPos + offsetRelativeToNewQr);
            sessionOrigin.transform.rotation = CreateQuaternionCopy(qrCodePointRot); // to do add initial rotation

            totalOffset = totalOffset + CreateVectorCopy(sessionOrigin.transform.position - currPos);

            textField.text += $"Offset after {offset}";

            VisualizePointsDifference(CreateVectorCopy(sessionOrigin.transform.position), CreateVectorCopy(qrCodePointPos));



            //session.Reset();

            //VisualizeVectorsDifference(qrCodePositionObjectPos, qrCodePositionObjectPos + offsetRelativeToNewQr);
            //// Add offset for recentering - distance to QR
            //sessionOrigin.transform.position = new Vector3(
            //    (float)(gameObjectPos.x + System.Math.Cos(System.Math.PI/4)), 
            //    gameObjectPos.y,
            //    (float)(gameObjectPos.z + System.Math.Cos(System.Math.PI / 4))
            //    );
        }
    }

    private void DisableScanner()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnChanged;
        m_TrackedImageManager.enabled = false;
    }

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in m_TrackedImageManager.trackables)
        {
            trackedImage.transform.localScale = new Vector3(trackedImage.referenceImage.size.x, 0.005f, trackedImage.referenceImage.size.y);
            //trackedImage.transform.position += totalOffset;
        }

        //foreach (var newImage in eventArgs.added)
        //{
        //    BAD POSITIONING HERE
        //    // Handle added event
        //    textField.text = $"There are {m_TrackedImageManager.trackables.count} images being tracked.\n" + ($"Image: {newImage.referenceImage.name} is NEW at " +
        //          $"{newImage.transform.position}");
        //}

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            // Handle updated event
            Vector3 imagePos = CreateVectorCopy(trackedImage.transform.position);
            Vector3 currentPos = CreateVectorCopy(indicator.gameObject.transform.position) - totalOffset;
            Vector3 differenceVec = CreateVectorCopy(currentPos - imagePos);

            //VisualizePointsDifference(imagePos, currentPos); deleted bcs it overlays another func call after scan


            string msg = $"There are {m_TrackedImageManager.trackables.count} images being tracked.\n"
                + $"Image: {trackedImage.referenceImage.name} is at " + $"{imagePos}.\n" +
                  $"Indicator pos: {currentPos}\n" +
                  $"Difference vector: {differenceVec}\nDistance: {Vector3.Distance(currentPos, imagePos)}\n" +
                  $"Y rotation {trackedImage.transform.rotation.eulerAngles.y}";

            if (firstText == "")
            {
                firstText = msg;
            }
            if (plannedRecenter)
            {
                plannedRecenter = false;
                SetQrCodeRecenterTarget(trackedImage.referenceImage.name, CreateVectorCopy(imagePos), CreateVectorCopy(currentPos), CreateQuaternionCopy(trackedImage.transform.rotation));
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

    private void VisualizePointsDifference(Vector3 vector1, Vector3 vector2)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, vector1);
        lineRenderer.SetPosition(1, vector2);

        Vector3 difference = CreateVectorCopy(vector2 - vector1);

        //float dx = System.Math.Abs(vector2.x - vector1.x);
        //float dy = System.Math.Abs(vector2.y - vector1.y);
        //float dz = System.Math.Abs(vector2.z - vector1.z);

        LineRenderer lr1 = lineRenderer1.GetComponent<LineRenderer>();
        lr1.enabled = true;
        lr1.SetPosition(0, vector1);
        Vector3 dxRepresent = CreateVectorCopy(new Vector3(difference.x, 0, 0) + vector1);
        lr1.SetPosition(1, dxRepresent);

        LineRenderer lr2 = lineRenderer2.GetComponent<LineRenderer>();
        lr2.enabled = true;
        lr2.SetPosition(0, dxRepresent);
        Vector3 dyRepresent = CreateVectorCopy(new Vector3(0, difference.y, 0) + dxRepresent);
        lr2.SetPosition(1, dyRepresent);

        LineRenderer lr3 = lineRenderer3.GetComponent<LineRenderer>();
        lr3.enabled = true;
        lr3.SetPosition(0, dyRepresent);
        lr3.SetPosition(1, CreateVectorCopy(new Vector3(0, 0, difference.z) + dyRepresent));
    }

    private Vector3 CreateVectorCopy(Vector3 initialVector)
    {
        return new Vector3(initialVector.x, initialVector.y, initialVector.z);
    }

    private Quaternion CreateQuaternionCopy(Quaternion initialQuaternion)
    {
        return new Quaternion(initialQuaternion.x, initialQuaternion.y, initialQuaternion.z, initialQuaternion.w);
    }

}
