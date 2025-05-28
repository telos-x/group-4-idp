using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class CSVUpdater : MonoBehaviour
{
    private string filename = "";

    private List<int> xList;
    private List<int> yList;
    
    void Awake()
    {
        filename = "Assets/Scripts/IDPGroup4Data.csv";
        
        xList = new List<int>() { 5, 10, 15, 17, 19, 24, 28, 35, 64, 76, 81, 90, 123, 134, 199 };
        yList = new List<int>() { 5, 98, 56, 130, 29, 17, 15, 30, 109, 199, 187, 79, 150, 170, 10 };
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            for(int i = 0; i < xList.Count; i++)
            {
                xList[i] = UnityEngine.Random.Range(0, 15 + (5 * i));
                yList[i] = UnityEngine.Random.Range(0, 25 + (5 * i));
            }

            WriteCSV();
        }
            
    }

    void WriteCSV()
    {
        try
        {
            if(xList.Count > 0 && yList.Count > 0 )
            {
                Debug.Log("Pass");
                string csvData = File.ReadAllText(filename);

                string[] lines = csvData.Split("\n");
                string[] individualData = new string[] {};

                for(int i = 0; i < xList.Count; i++)
                {
                    Debug.Log(xList[i].ToString() + ", " + yList[i].ToString());  

                    individualData = lines[i].Split(",");
                    if(individualData.Length > 0)
                    {
                        individualData[0] = xList[i].ToString();
                        individualData[1] = yList[i].ToString();

                        lines[i] = string.Join(",", individualData);
                    }
                }

                string modifiedCSV = string.Join("\n", lines);
                File.WriteAllText(filename, modifiedCSV);

                Debug.Log("CSV file modified successfully!");
            }
            else
            {
                Debug.Log("CSV file error");
            }
        }
        catch(FileNotFoundException)
        {
            Debug.LogError("CSV file not found: " + filename);
        }
        catch(Exception e)
        {
            Debug.LogError("Error modifying CSV file: " + filename);
        }
    }
}
