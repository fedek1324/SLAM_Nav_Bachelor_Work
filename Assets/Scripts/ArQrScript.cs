using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private Quaternion lastTeleportRotation = new Quaternion();

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

        //m_TrackedImageManager.trackedImagesChanged += OnChanged;
        //textField.text += $"\nLoaded tracking manager";
        //DisableScanner();
        //InitTrackedImageManager();
    }

    public void InitTrackedImageManager()
    {
        m_TrackedImageManager = new GameObject().AddComponent<ARTrackedImageManager>();
        m_TrackedImageManager.transform.parent = transform;
        m_TrackedImageManager.transform.position = sessionOrigin.transform.position;
        m_TrackedImageManager.transform.rotation = lastTeleportRotation;
        m_TrackedImageManager.referenceLibrary = imageLibrary;
        m_TrackedImageManager.enabled = true;
        m_TrackedImageManager.requestedMaxNumberOfMovingImages = 1;
        m_TrackedImageManager.maxNumberOfMovingImages = 1;
        m_TrackedImageManager.trackedImagePrefab = scannedImagePrefab;
        m_TrackedImageManager.trackedImagesChanged += OnChanged;
        textField.text += $"\nLoaded tracking manager";
    }

    public void DisableScanner()
    {
        m_TrackedImageManager.trackedImagePrefab = scannedImagePrefab;
        m_TrackedImageManager.trackedImagesChanged -= OnChanged;
        m_TrackedImageManager.enabled = false;
        m_TrackedImageManager.StopAllCoroutines();
        m_TrackedImageManager = null;
        textField.text += $"\nDisabled tracking manager";
    }

    private Vector3 RotateVectorAroundY(Vector3 vector, float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.up) * CreateVectorCopy(vector); // Vector.up represents Y axis
    }

    public void SetQrCodeRecenterTarget(string targetText, Vector3 imagePos, Vector3 currPos, Quaternion imageRot)
    {
        // Maybe get currPos from sessionOrigin
        Vector3 offset = CreateVectorCopy(currPos - imagePos);
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
            textField2.text = $"\n{qrCodePointRot.eulerAngles.y} - {imageRot.eulerAngles.y} = {qrCodePointRot.eulerAngles.y - imageRot.eulerAngles.y}";
            textField2.text += $"\nMoving";
            Vector3 offsetRelativeToNewQr = CreateVectorCopy(RotateVectorAroundY(offset, qrCodePointRot.eulerAngles.y - imageRot.eulerAngles.y));

            //// calculates bad if a spawn with angle
            //Vector3 rotation = CreateVectorCopy(sessionOrigin.transform.rotation.eulerAngles);
            //rotation.y += qrCodePointRot.eulerAngles.y - imageRot.eulerAngles.y;
            //Quaternion newRot = ToQ(rotation.y, rotation.x, rotation.z);
            
            // Quaternion copy = ToQ(qrCodePointRot.eulerAngles.y, qrCodePointRot.eulerAngles.x, qrCodePointRot.eulerAngles.z); // YXZ
            // textField2.text += $"\n qrCodeQuaternion: {qrCodePointRot}\nCopy: {copy}";

            sessionOrigin.transform.position = CreateVectorCopy(qrCodePointPos + offsetRelativeToNewQr);
            sessionOrigin.transform.rotation = CreateQuaternionCopy(qrCodePointRot); // to do add initial rotation
            lastTeleportRotation = CreateQuaternionCopy(qrCodePointRot);

            textField2.text += $"\nMoving1";

            // session, EventSystem, QrScannerNew, New Game Object
            session.transform.position = CreateVectorCopy(qrCodePointPos + offsetRelativeToNewQr);
            session.transform.rotation = CreateQuaternionCopy(qrCodePointRot); // to do add initial rotation

            textField2.text += $"\nMoving2";

            GameObject eventSystem = GameObject.Find("EventSystem");
            eventSystem.transform.position = CreateVectorCopy(qrCodePointPos + offsetRelativeToNewQr);
            eventSystem.transform.rotation = CreateQuaternionCopy(qrCodePointRot); // to do add initial rotation

            textField2.text += $"\nMoving3";

            GameObject qrScannerNew = GameObject.Find("QrScannerNew");
            qrScannerNew.transform.position = CreateVectorCopy(qrCodePointPos + offsetRelativeToNewQr);
            qrScannerNew.transform.rotation = CreateQuaternionCopy(qrCodePointRot); // to do add initial rotation

            textField2.text += $"\nMoving4";

            //We need it (maybe) when we create AR Tracked Image Manager in runtime 
            //GameObject newGO = GameObject.Find("New Game Object");
            //newGO.transform.position = CreateVectorCopy(qrCodePointPos + offsetRelativeToNewQr);
            //newGO.transform.rotation = CreateQuaternionCopy(qrCodePointRot); // to do add initial rotation

            //textField2.text += $"\nMoving5";


            totalOffset = totalOffset + CreateVectorCopy(sessionOrigin.transform.position - currPos);
            textField2.text += $"\nCalling Visualize";
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

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        textField2.text += $"\nOnChanged";
        foreach (var trackedImage1 in m_TrackedImageManager.trackables)
        {
            trackedImage1.transform.localScale = new Vector3(trackedImage1.referenceImage.size.x, 0.005f, trackedImage1.referenceImage.size.y);
            //trackedImage.transform.position += totalOffset;
        }

        //foreach (var newImage in eventArgs.added)
        //{
        //    BAD POSITIONING HERE
        //    // Handle added event
        //    textField.text = $"There are {m_TrackedImageManager.trackables.count} images being tracked.\n" + ($"Image: {newImage.referenceImage.name} is NEW at " +
        //          $"{newImage.transform.position}");
        //}


        //foreach (ARTrackedImage trackedImage in eventArgs.updated)
        //{
        ARTrackedImage trackedImage = eventArgs.updated.Last();
            // Handle updated event
            Vector3 imagePos = CreateVectorCopy(trackedImage.transform.position);
            //Vector3 currentPos = CreateVectorCopy(indicator.gameObject.transform.position) - totalOffset;
            Vector3 currentPos = CreateVectorCopy(indicator.gameObject.transform.position);
            Vector3 differenceVec = CreateVectorCopy(currentPos - imagePos);

            //VisualizePointsDifference(imagePos, currentPos); deleted bcs it overlays another func call after scan


            string msg = $"There are {m_TrackedImageManager.trackables.count} images being tracked.\n"
                + $"Image: {trackedImage.referenceImage.name}\n is at " + $"{imagePos}.\n" +
                  $"Indicator pos: {currentPos}\n" +
                  $"Difference vector: {differenceVec}\nDistance: {Vector3.Distance(currentPos, imagePos)}\n" +
                  $"Y rotation {trackedImage.transform.rotation.eulerAngles.y}";

            if (firstText == "" && currentPos != new Vector3(0,0,0) && imagePos != new Vector3(0, 0, 0))
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

        //}

        //foreach (var removedImage in eventArgs.removed)
        //{
        //    // Handle removed event
        //    textField.text = $"There are {m_TrackedImageManager.trackables.count} images being tracked." + ($"Image: {removedImage.referenceImage.name} is REMOVED");
        //}
    }

    private void VisualizePointsDifference(Vector3 vector1, Vector3 vector2)
    {
        textField2.text += "Visualizing position offset";
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

    private Quaternion ToQ(float yaw, float pitch, float roll) // YXZ
    {
        yaw *= Mathf.Deg2Rad;
        pitch *= Mathf.Deg2Rad;
        roll *= Mathf.Deg2Rad;
        float rollOver2 = roll * 0.5f;
        float sinRollOver2 = (float)Math.Sin((double)rollOver2);
        float cosRollOver2 = (float)Math.Cos((double)rollOver2);
        float pitchOver2 = pitch * 0.5f;
        float sinPitchOver2 = (float)Math.Sin((double)pitchOver2);
        float cosPitchOver2 = (float)Math.Cos((double)pitchOver2);
        float yawOver2 = yaw * 0.5f;
        float sinYawOver2 = (float)Math.Sin((double)yawOver2);
        float cosYawOver2 = (float)Math.Cos((double)yawOver2);
        Quaternion result;
        result.w = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
        result.x = cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2;
        result.y = sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2;
        result.z = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;

        return result;
    }
}
