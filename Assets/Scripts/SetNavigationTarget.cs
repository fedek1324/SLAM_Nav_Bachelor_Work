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
    private float yLineOffset = 0.5f;
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
    //public float PercentHead = 0.4f;

    //private GameObject lineRenderersGO;
    //private List<LineRenderer> lineRenderers;

    [SerializeField]
    private GameObject arrowPrefab;
    private List<GameObject> arrows;
    int iter= 0;

    // Start is called before the first frame update
    private void Start()
    {
        path = new NavMeshPath();
        line = transform.GetComponent<LineRenderer>();
        line.enabled = lineToggle;

        GameObject parentObject = GameObject.Find("NavigationTarget");
        SetChildrenActiveRecursive(parentObject, false);

        arrows = new List<GameObject>();

    }

    // Update is called once per frame
    private void Update()
    {
        iter++;
        if (iter % 100 != 0)
        {
            return;
        }

        if (lineToggle && targetPosition != Vector3.zero)
        {
            NavMesh.CalculatePath(SetPositionOffset(transform.position), targetPosition, NavMesh.AllAreas, path);
            Vector3[] calculatedPathAndOffset = AddLineOffset();

            line.positionCount = path.corners.Length;
            line.SetPositions(calculatedPathAndOffset);

            //DrawArrowLines(arrows, calculatedPathAndOffset);


            //int lineRenderersNeed = 1;
            //Material material = new Material(line.material);
            //initLineRenderers(lineRenderersNeed, material, lineRenderersGO);
            //DrawArrow(new Vector3(0, 0, 0), new Vector3(0, 0, 1), PercentHead, lineRenderers[0]);
        }
    }

    public void DrawArrowLines(List<GameObject> arrows, Vector3[] calculatedPathAndOffset)
    {
        for (int i = 0; i < arrows.Count; i++)
        {
            Destroy(arrows[i]);
        }
        for (int i = 0; i < calculatedPathAndOffset.Length - 1; i++)
        {
            DrawArrowLine(calculatedPathAndOffset[i], calculatedPathAndOffset[i + 1], 0.5f, line);
        }
    }

    public void DrawArrowLine(Vector3 startPoint, Vector3 endPoint, float arrowSpacing, LineRenderer lineRenderer)
    {
        // Calculate the direction and length of the line
        Vector3 lineDirection = (endPoint - startPoint).normalized;
        float lineLength = Vector3.Distance(startPoint, endPoint);

        // Set the positions of the line
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);

        // Calculate the number of arrows based on the spacing
        int numArrows = Mathf.FloorToInt(lineLength / arrowSpacing);

        // Instantiate arrows along the line
        for (int i = 1; i <= numArrows; i++)
        {
            Vector3 arrowPosition = startPoint + lineDirection * (arrowSpacing * i);
            Quaternion arrowRotation = Quaternion.LookRotation(RotateVectorAroundY(lineDirection, -90));
            GameObject arrow = Instantiate(arrowPrefab, arrowPosition, arrowRotation);
            arrows.Add(arrow);
        }
    }

    private Vector3 RotateVectorAroundY(Vector3 vector, float angle)
    {
        return Quaternion.AngleAxis(angle, Vector3.up) * (vector); // Vector.up represents Y axis
    }

    //private void initLineRenderers(int lineRenderersNeed, Material material, GameObject lineRenderersGO)
    //{
    //    Destroy(lineRenderersGO);
    //    lineRenderersGO = new GameObject();
    //    lineRenderers = new List<LineRenderer>(lineRenderersNeed);
    //    for (int i = 0; i < lineRenderersNeed; i++)
    //    {
    //        LineRenderer lineRenderer = lineRenderersGO.AddComponent<LineRenderer>();
    //        lineRenderer.material = material;
    //    }
    //}

    //public void DrawArrow(Vector3 ArrowOrigin, Vector3 ArrowTarget, float percentHead, LineRenderer lineRenderer)
    //{
    //    lineRenderer.positionCount = 2;
    //    lineRenderer.widthCurve = new AnimationCurve(
    //     new Keyframe(0, 0.4f)
    //     , new Keyframe(0.999f - PercentHead, 0.4f)  // neck of arrow
    //     , new Keyframe(1 - PercentHead, 1f)  // max width of arrow head
    //     , new Keyframe(1, 0f));  // tip of arrow

    //    lineRenderer.SetPositions(new Vector3[] {
    //          ArrowOrigin
    //          , Vector3.Lerp(ArrowOrigin, ArrowTarget, 0.999f - PercentHead)
    //          , Vector3.Lerp(ArrowOrigin, ArrowTarget, 1 - PercentHead)
    //          , ArrowTarget });
    //}
    //public Vector3[] SplitVector(Vector3 vector, float pathLength = 1)
    //{
    //    int numSplits = (int)(vector.magnitude / pathLength);
    //    Vector3 normalizedVector = vector.normalized;

    //    Vector3[] splitVectors = new Vector3[numSplits];

    //    for (int i = 0; i < numSplits; i++)
    //    {
    //        splitVectors[i] = normalizedVector * pathLength;
    //    }

    //    return splitVectors;
    //}

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
