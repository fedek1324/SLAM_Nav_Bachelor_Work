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
    private List<Target> navigationTargetObjects = new List<Target>();
    [SerializeField]
    private Slider navigationYOffset;

    private float positionYOffset = -1.5f;
    private float yLineOffset = 1;
    private Target currentTarget;

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
            currentTarget.PositionObject.SetActive(false); // disable prev target visibility
        }
        targetPosition = Vector3.zero;
        currentTarget = navigationTargetObjects.Find(x => x.Name.Equals(buttonText));
        if (currentTarget != null)
        {
            currentTarget.PositionObject.SetActive(true);
            if (!line.enabled)
            {
                ToggleVisibility();
            }

            targetPosition = currentTarget.PositionObject.transform.position;

            mainTitle.text = $"¬€¡–¿ÕÕŒ≈ Ã≈—“Œ:\n{currentTarget.Name}";
        }
    }

    public void ToggleVisibility()
    {
        lineToggle = !lineToggle;
        line.enabled = lineToggle;
    }
}
