using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FunctionalityWrapper : MonoBehaviour
{
    private ArQrScript newRecenterScript;
    private QrCodeRecenter oldRecenterScript;

    [SerializeField]
    private bool usingOldRecenter = false;
    [SerializeField]
    Text debugText;

    // Start is called before the first frame update
    void Start()
    {
        GameObject newRecenter = GameObject.Find("NewRecenter");
        newRecenterScript = newRecenter.GetComponent<ArQrScript>();
        GameObject oldRecenter = GameObject.Find("QrCodeRecenter");
        oldRecenterScript = oldRecenter.GetComponent<QrCodeRecenter>();
        if (usingOldRecenter)
        {
            newRecenter.SetActive(false);
        } 
        else
        {
            oldRecenter.SetActive(false);
        }
        debugText.text += usingOldRecenter ? "\nInit old scanner" : "\nInit new scanner";
    }

    public void ToggleScanning()
    {
        if (usingOldRecenter)
        {
            oldRecenterScript.ToggleScanning();
        }
        else
        {
            newRecenterScript.ToggleQrCodeScanning();
        }
    }

}
