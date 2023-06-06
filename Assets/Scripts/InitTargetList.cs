using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InitTargetList : MonoBehaviour
{

    [SerializeField]
    GameObject indicator;
    [SerializeField]
    GameObject buttonExample;
    [SerializeField]
    GameObject navTargets;

    // Start is called before the first frame update
    void Start()
    {
        SetNavigationTarget script = indicator.GetComponent<SetNavigationTarget>();
        AddButtonRecursive(navTargets, script);

        this.transform.GetComponent<SearchScript>().initList();
        //CopyButton(buttonExample, script, "6110");
        //CopyButton(buttonExample, script, "6210");
    }

    public void AddButtonRecursive(GameObject navGO, SetNavigationTarget setNavTarget)
    {
        int childCount = navGO.transform.childCount;
        if (IsTarget(navGO))
        {
            CopyButton(buttonExample, setNavTarget, navGO.name);
            return;
        }
        for (int i = 0; i < childCount; i++)
        {
            GameObject child = navGO.transform.GetChild(i).gameObject;
            AddButtonRecursive(child, setNavTarget);
        }
    }

    public void CopyButton(GameObject originalButton, SetNavigationTarget setNavTarget, string text)
    {
        // Instantiate a new button
        GameObject newButton = Instantiate(originalButton, originalButton.transform.parent);

        TextMeshProUGUI tmpText = newButton.GetComponentInChildren<TextMeshProUGUI>();
        tmpText.text = text;

        Button btn = newButton.GetComponentInChildren<Button>();
        btn.onClick.AddListener(() => {
            setNavTarget.SetCurrentNavigationTarget(tmpText.text);
        });
        
        newButton.SetActive(true);
    }

    public bool IsTarget(GameObject gameObject)
    {
        int childCount = gameObject.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            if (child.name == "isTarget")
            {
                return true;
            }
        }
        return false;
    }

    public static GameObject FindInChildrenRecursive(GameObject parent, string name)
    {
        if (parent.name == name)
            return parent;

        int childCount = parent.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;
            GameObject result = FindInChildrenRecursive(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
}
