//using System;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.XR.ARFoundation;
//using UnityEngine.XR.ARSubsystems;

//public class ArQrScript : MonoBehaviour
//{
//    [SerializeField]
//    XRReferenceImageLibrary imageLibrary;

//    private ARTrackedImageManager m_TrackedImageManager;

//    [SerializeField]
//    private GameObject indicator;

//    [SerializeField]
//    private ARSession session;
//    [SerializeField]
//    private ARSessionOrigin sessionOrigin;

//    [SerializeField]
//    private GameObject qrCodeScanningPanel;

//    [SerializeField]
//    Text debugText;

//    public bool scanningEnabled = false;

//    public bool doSessionReset = true;
//    public bool useAngle = false;

//    private Quaternion lastTeleportRotation = new Quaternion();

//    GameObject area;
//    GameObject mainPanel;

//    public void EnableAngleUse() { 
//        useAngle = true; 
//    }
//    public void DisableAngleUse() { 
//        useAngle = false; 
//    }

//    GameObject managerContainer;

//    public void EnableSessionReset() { 
//        doSessionReset = true; 
//    }
//    public void DisableSessionReset() { 
//        doSessionReset = false; 
//    }


//    private void Start()
//    {
//        area = GameObject.Find("NavigationArea");
//        mainPanel = GameObject.Find("MainPanel");
//    }

//    public void InitTrackedImageManager()
//    {
//        // maybe save GO link and delete Obj ob Disable Scanner
//        managerContainer = new GameObject();
//        m_TrackedImageManager = managerContainer.AddComponent<ARTrackedImageManager>();
//        m_TrackedImageManager.transform.parent = transform;
//        m_TrackedImageManager.transform.position = sessionOrigin.transform.position;
//        m_TrackedImageManager.transform.rotation = lastTeleportRotation;
//        m_TrackedImageManager.referenceLibrary = imageLibrary;
//        m_TrackedImageManager.enabled = true;
//        m_TrackedImageManager.requestedMaxNumberOfMovingImages = 1;
//        m_TrackedImageManager.maxNumberOfMovingImages = 1;
//        //m_TrackedImageManager.trackedImagePrefab = scannedImagePrefab;
//        m_TrackedImageManager.trackedImagesChanged += OnChanged;
//    }

//    public void DisableScanner()
//    {
//        //m_TrackedImageManager.trackedImagePrefab = scannedImagePrefab;
//        m_TrackedImageManager.trackedImagesChanged -= OnChanged;
//        m_TrackedImageManager.enabled = false;
//        m_TrackedImageManager.StopAllCoroutines();
//        m_TrackedImageManager = null;
//        managerContainer = null;
//    }

//    private Vector3 RotateVectorAroundY(Vector3 vector, float angle)
//    {
//        return Quaternion.AngleAxis(angle, Vector3.up) * (vector); // Vector.up represents Y axis
//    }

//    public void SetQrCodeRecenterTarget(string targetText, Vector3 imagePos, Vector3 currPos, Quaternion imageRot)
//    {
//        // Maybe get currPos from sessionOrigin
//        Vector3 offset = (currPos - imagePos);

//        GameObject qrCodePoint = GameObject.Find(targetText);
//        if (qrCodePoint != null)
//        {
//            // Reset position and rotation of ARSession
//            if (doSessionReset)
//            {
//                session.Reset();
//            }

//            Vector3 qrCodePointPos = (qrCodePoint.transform.position);
//            Quaternion qrCodePointRot = (qrCodePoint.transform.rotation);
//            Vector3 offsetRelativeToNewQr = (RotateVectorAroundY(offset, qrCodePointRot.eulerAngles.y - imageRot.eulerAngles.y));

//            float distance = Vector3.Distance(currPos, imagePos);
//            Vector3 distVector = new Vector3(0, 0, -distance);
//            Vector3 offset2 = (RotateVectorAroundY(distVector, qrCodePointRot.eulerAngles.y));

//            //sessionOrigin.transform.position = (qrCodePointPos + offsetRelativeToNewQr);
//            if (useAngle)
//            {
//                sessionOrigin.transform.position = (qrCodePointPos + offsetRelativeToNewQr);
//            } 
//            else
//            {
//                sessionOrigin.transform.position = (qrCodePointPos + offset2);
//            }
//            sessionOrigin.transform.rotation = (qrCodePointRot); // to do add initial rotation
//            lastTeleportRotation = (qrCodePointRot);

//            // session, EventSystem, QrScannerNew, New Game Object
//            session.transform.position = (qrCodePointPos + offsetRelativeToNewQr);
//            session.transform.rotation = (qrCodePointRot); // to do add initial rotation

//            GameObject eventSystem = GameObject.Find("EventSystem");
//            eventSystem.transform.position = (qrCodePointPos + offsetRelativeToNewQr);
//            eventSystem.transform.rotation = (qrCodePointRot); // to do add initial rotation

//            GameObject qrScannerNew = GameObject.Find("QrScannerNew");
//            qrScannerNew.transform.position = (qrCodePointPos + offsetRelativeToNewQr);
//            qrScannerNew.transform.rotation = (qrCodePointRot); // to do add initial rotation
//        }
//    }

//    public void ToggleQrCodeScanning()
//    {
//        scanningEnabled = !scanningEnabled;
//        qrCodeScanningPanel.SetActive(scanningEnabled);

//        area.SetActive(!scanningEnabled);

//        mainPanel.SetActive(!scanningEnabled);

//        indicator.GetComponent<LineRenderer>().enabled = !scanningEnabled;

//        if (scanningEnabled)
//        {
//            InitTrackedImageManager();
//        }
//        else
//        {
//            DisableScanner();
//        }
//    }

//    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
//    {
//        ARTrackedImage trackedImage = eventArgs.updated.Last();
//        if (trackedImage.trackingState == TrackingState.Tracking) {
//            // Handle updated event
//            ToggleQrCodeScanning();
//            Vector3 imagePos = (trackedImage.transform.position);
//            Vector3 currentPos = (indicator.gameObject.transform.position);
//            SetQrCodeRecenterTarget(trackedImage.referenceImage.name, (imagePos), (currentPos), (trackedImage.transform.rotation));
//        }
//    }

//    //public void QuitApp()
//    //{
//    //    textField2.text += "\nBye";
//    //    try
//    //    {
//    //        DisableScanner();
//    //    }
//    //    catch (Exception e)
//    //    {

//    //    }
//    //    Application.Quit();
//    //}
//}
