using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SetNavigationTarget : MonoBehaviour
{
    [SerializeField]
    private TMP_Dropdown navigationTargetDropDown;
    [SerializeField]
    private Slider navigationYOffset;

    private float positionYOffset = -1.5f;
    private float yLineOffset = 1;
    private GameObject currentTarget;

    [SerializeField] 
    private TMP_Text mainTitle;

    //[SerializeField]
    //private Camera topDownCamera;

    //[SerializeField]
    //private GameObject navTargetObject;

    private NavMeshPath path; // current calculated papth
    private LineRenderer line; // linerenderer to display path
    private Vector3 targetPosition = Vector3.zero; // current target position

    private bool lineToggle = false;

    // Start is called before the first frame update
    private void Start()
    {
        path = new NavMeshPath();
        line = transform.GetComponent<LineRenderer>();
        line.enabled = lineToggle;

        GameObject parentObject = GameObject.Find("NavigationTarget");
        SetChildrenActiveRecursive(parentObject, false);

    }

    // Update is called once per frame
    private void Update()
    {
        if (lineToggle && targetPosition != Vector3.zero)
        {
            NavMesh.CalculatePath(SetPositionOffset(transform.position), targetPosition, NavMesh.AllAreas, path);
            line.positionCount = path.corners.Length;
            Vector3[] calculatedPathAndOffset = AddLineOffset();
            line.SetPositions(calculatedPathAndOffset);
        }
    }

    private Vector3 SetPositionOffset(Vector3 position)
    {
        // to NavMesh choose right navMesh
        return position + new Vector3(0, positionYOffset, 0);
    }

    private Vector3[] AddLineOffset()
    {
        Vector3[] calculatedLine = new Vector3[path.corners.Length];
        for (int i = 0; i < path.corners.Length; i++)
        {
            //float yOffset = navigationYOffset.value;
            calculatedLine[i] = path.corners[i] + new Vector3(0, yLineOffset, 0);
        }
        return calculatedLine;
    }

    public void SetCurrentNavigationTarget(string buttonText)
    {
        if (currentTarget != null)
        {
            currentTarget.SetActive(false); // disable prev target visibility
        }
        targetPosition = Vector3.zero;

        SetChildrenActiveRecursive(GameObject.Find("NavigationTarget"), true);
        GameObject target = FindInChildrenRecursive(GameObject.Find("NavigationTarget"), buttonText);
        SetChildrenActiveRecursive(GameObject.Find("NavigationTarget"), false);

        currentTarget = target;
        if (currentTarget != null)
        {
            SetParentsActiveRecursive(currentTarget, true);
            currentTarget.SetActive(true);
            if (!line.enabled)
            {
                ToggleVisibility();
            }

            targetPosition = currentTarget.transform.position;

            mainTitle.text = $"ÂÛÁÐÀÍÍÎÅ ÌÅÑÒÎ:\n{currentTarget.name}";
        }
    }

    public void ToggleVisibility()
    {
        lineToggle = !lineToggle;
        line.enabled = lineToggle;
    }

    public static void SetChildrenActiveRecursive(GameObject parent, bool active)
    {
        int childCount = parent.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            GameObject child = parent.transform.GetChild(i).gameObject;
            child.SetActive(active);
            SetChildrenActiveRecursive(child, active);
        }
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

    public static void SetParentsActiveRecursive(GameObject child, bool active)
    {
        Transform parent = child.transform.parent;
        if (parent != null)
        {
            GameObject parentObject = parent.gameObject;
            parentObject.SetActive(active);
            SetParentsActiveRecursive(parentObject, active);
        }
    }
}
