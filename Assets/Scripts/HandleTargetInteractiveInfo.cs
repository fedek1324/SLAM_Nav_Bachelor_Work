using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleTargetInteractiveInfo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AddInteractiveTextRecursive(gameObject);
    }

    public void AddInteractiveTextRecursive(GameObject navGO)
    {
        if (IsTarget(navGO))
        {
            SetInteractiveText(navGO);
            return;
        }
        int childCount = navGO.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            GameObject child = navGO.transform.GetChild(i).gameObject;
            AddInteractiveTextRecursive(child);
        }
    }

    public void SetInteractiveText(GameObject gameObject)
    {
        TextMesh text = gameObject.GetComponentInChildren<TextMesh>();
        text.text = "AAAAAAAAAAAAAAAAA";
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
}
