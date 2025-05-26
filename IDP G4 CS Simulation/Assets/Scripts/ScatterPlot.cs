//Cpde written by https://www.youtube.com/watch?v=8cFALzCB3dA
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScatterPlot : MonoBehaviour 
{
   public class Point
    {
        public float xval;
        public float yval;
        public GameObject pointObj;

        public Point(float x, float y)
        {
            this.xval = x;
            this.yval = y;
        }

    }

    public Canvas canvas;
    public Transform XAxis;
    public Transform YAxis;
    public Transform Main;

    //Increment values
    public int xInc = 100;
    public int yINc = 100;

    public GameObject pointPrefab;
    public GameObject xValPrefab;
    public GameObject yValPrefab;

    private float mainWidth;
    private float mainHeight;

    private List<Point> points = new List<Point>();

    private void Start()
    {
        //Find Rectangle size
        Rect rect = RectTransformUtility.PixelAdjustRect(Main.GetComponent<RectTransform>(), canvas);

        //To allow for tolerance
        mainWidth = rect.width - 100;
        mainHeight = rect.height - 100;


    }
}
