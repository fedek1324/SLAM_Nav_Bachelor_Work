using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using System.IO;
using Packages;
using UnityEngine.UI;
using System.Linq;

public class HandleTargetInteractiveInfo : MonoBehaviour
{
    [SerializeField]
    Text debugText;
    // Start is called before the first frame update
    void Start()
    {
        AddInteractiveTextRecursive(gameObject);
        ReadCSVFile(Path.Combine(Application.dataPath, "4floorInteractiveData.csv"));
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

    public void ReadCSVFile(string filePath)
    {
        List<string[]> data = new List<string[]>();

        using (StreamReader reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] row = line.Split(',');

                data.Add(row);
            }
        }

        // Process the data as desired
        foreach (string[] row in data)
        {
            //debugText.text = string.Join(',', row);
            foreach (string cell in row)
            {
                debugText.text += cell + ", ";
            }
            debugText.text += "\n";
        }
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
