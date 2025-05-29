using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CodeMonkey.Utils;
using NUnit.Framework;

public class CSVUpdater : MonoBehaviour
{
    private string filename = "Assets/Scripts/IDPGroup4Data.csv";

    private List<int> xlist = new List<int>();
    private List<int> ylist = new List<int>();
    public void UpdateCSV()
    {
        ReadCSV();

        for (int i = 0; i < xlist.Count; i++)
        {
            //xList[i] = UnityEngine.Random.Range(0, 15 + (5 * i));
            xlist[i] = (0 + (i * 5));
            ylist[i] = UnityEngine.Random.Range(0, 25 + (5 * i));
        }
        xlist.Sort();

        Debug.Log(xlist.Count + " & " + ylist.Count);

        WriteCSV(xlist, ylist);

        AssetDatabase.Refresh();

        xlist.Clear();
        ylist.Clear(); 
    }

    private void ReadCSV()
    {
        // List<int> tempXList = new List<int>();
        // List<int> tempYList = new List<int>();

        Debug.Log(filename);
        string csvData = File.ReadAllText(filename);

        string[] lines = csvData.Split("\n");

        string[] individualData = new string[] { };

        for (int i = 0; i < lines.Length - 1; i++)
        {
            individualData = lines[i].Split(",");

            xlist.Add(int.Parse(individualData[0]));
            ylist.Add(int.Parse(individualData[1]));
        }
    }
    private void WriteCSV(List<int> xList, List<int> yList)
    {
        try
        {
            //Debug.Log("X:" + xList[0] + " Y: " + yList[0]);

            if (xList.Count > 0 && yList.Count > 0 )
            {
                //Debug.Log("Pass");
                string csvData = File.ReadAllText(filename);

                string[] lines = csvData.Split("\n");
                string[] individualData = new string[] {};

                for(int i = 0; i < xList.Count; i++)
                {
                    //Debug.Log(xList[i].ToString() + ", " + yList[i].ToString());  

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
