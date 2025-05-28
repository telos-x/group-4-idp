using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using NUnit.Framework;

public class Window_Graph : MonoBehaviour {

    private static Window_Graph instance;

    [SerializeField] private Sprite PointSprite;
    [SerializeField] private string xAxis;
    [SerializeField] private string yAxis;
    [SerializeField] private float xInterval;
    [SerializeField] private float yInterval;
    [SerializeField] private float xBuffer;
    [SerializeField] private float yBuffer;
    [SerializeField] private bool xZeroStart;
    [SerializeField] private bool yZeroStart;

    private RectTransform graphContainer;
    private RectTransform axisTemplateX;
    private RectTransform axisTemplateY;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    private List<GameObject> gameObjectList;
    private List<IgraphVisualObject> graphVisualObjectList;
    private GameObject tooltipObject;
    private List<RectTransform> yLabelList;
    private List<RectTransform> xLabelList;

    private float graphWidth;
    private float graphHeight;

    // Cached values
    private List<int> xList;
    private List<int> yList;

    private void Awake()
    {
        instance = this;
        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("LabelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("LabelTemplateY").GetComponent<RectTransform>();
        axisTemplateX = graphContainer.Find("AxisTemplateX").GetComponent<RectTransform>();
        axisTemplateY = graphContainer.Find("AxisTemplateY").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.Find("DashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("DashTemplateY").GetComponent<RectTransform>();
        tooltipObject = graphContainer.Find("ToolTip").gameObject;

        xLabelList = new List<RectTransform>();
        yLabelList = new List<RectTransform>();
        gameObjectList = new List<GameObject>();
        graphVisualObjectList = new List<IgraphVisualObject>();

        //enter information from calculstions via import csv
        xList = new List<int>() { 5, 10, 15, 17, 19, 24, 28, 35, 64, 76, 81, 90, 123, 134, 199 };
        yList = new List<int>() { 5, 98, 56, 130, 29, 17, 15, 30, 109, 199, 187, 79, 150, 170, 10 };

        IgraphVisual graphVisual = new LineGraphVisual(graphContainer, PointSprite, Color.yellow, new Color(255, 255, 0, .25f), 15);

        ShowGraph(xList, yList, graphVisual);

        // List<int> xList2 = new List<int>() { 10, 20, 25, 27, 29, 24, 28, 35, 64, 84, 86, 100, 134, 150};
        // List<int> yList2 = new List<int>() { 10, 98, 56, 130, 29, 17, 35, 40, 112, 186, 100, 79, 150, 180};
        // ShowGraph(xList2, yList2);

        int xvalue = 5;
        int yvalue = 5;

        FunctionPeriodic.Create(() =>
        {
            UpdateValue(0, xvalue, yvalue);
            xvalue++;
            yvalue++;
        }, .01f);
    }

    private void ShowGraph(List<int> xList, List<int> yList, IgraphVisual graphVisual)
    {
        graphWidth = graphContainer.sizeDelta.x;
        graphHeight = graphContainer.sizeDelta.y;

        foreach (GameObject gameObject in gameObjectList)
        {
            Destroy(gameObject);
        }
        gameObjectList.Clear();

        yLabelList.Clear();
        xLabelList.Clear();

        foreach (IgraphVisualObject gameObject in graphVisualObjectList)
        {
            gameObject.CleanUp();
        }
        gameObjectList.Clear();

        //Create Max and Min with buffers
        float yMin, yMax;
        CalculateScale(out yMin, out yMax, yList, yBuffer, yZeroStart);

        float xMin, xMax;
        CalculateScale(out xMin, out xMax, xList, xBuffer, xZeroStart);

        RectTransform axisX = Instantiate(axisTemplateX);
        axisX.SetParent(graphContainer, false);
        axisX.gameObject.SetActive(true);
        axisX.GetComponent<Text>().text = xAxis;
        gameObjectList.Add(axisX.gameObject);

        RectTransform axisY = Instantiate(axisTemplateY);
        axisY.SetParent(graphContainer, false);
        axisY.gameObject.SetActive(true);
        axisY.GetComponent<Text>().text = yAxis;
        gameObjectList.Add(axisY.gameObject);

        //Create X labels
        for (int i = 0; i <= xInterval; i++)
        {
            float normalizedValue = (i * 1f) / xInterval;

            //X labels;
            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer, false);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2((normalizedValue * graphWidth), -7f);
            labelX.GetComponent<Text>().text = Mathf.RoundToInt(xMin + (normalizedValue * (xMax - xMin))).ToString();

            xLabelList.Add(labelX);
            gameObjectList.Add(labelX.gameObject);

            //X Dashes:
            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(graphContainer, false);
            dashX.gameObject.SetActive(true);
            dashX.anchoredPosition = new Vector2((normalizedValue * graphWidth), -3f);
            gameObjectList.Add(dashX.gameObject);

        }

        //Create Y labels
        for (int i = 0; i <= yInterval; i++)
        {
            // value between 0 to 1; ratio
            float normalizedValue = (i * 1f) / yInterval;

            //Y labels
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer, false);
            labelY.gameObject.SetActive(true);
            labelY.anchoredPosition = new Vector2(-7f, (normalizedValue * graphHeight) + 15f);


            //yMin acts as the most lowest or origin of the y values
            labelY.GetComponent<Text>().text = Mathf.RoundToInt(yMin + (normalizedValue * (yMax - yMin))).ToString();

            yLabelList.Add(labelY);
            gameObjectList.Add(labelY.gameObject);

            //Y Dashes:
            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer, false);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(-4f, (normalizedValue * graphHeight));
            gameObjectList.Add(dashY.gameObject);
        }

        //Create Graph
        for (int i = 0; i < xList.Count; i++)
        {
            //x position would also need to be based off this value list
            float xPosition = ((xList[i] - xMin) / (xMax - xMin)) * graphWidth;
            float yPosition = ((yList[i] - yMin) / (yMax - yMin)) * graphHeight;

            string tooltipText = "(" + xList[i].ToString() + ", " + yList[i].ToString() + ")";
            IgraphVisualObject graphVisualObject = graphVisual.CreateGraphVisualObject(new Vector2(xPosition, yPosition), tooltipText);

            graphVisualObjectList.Add(graphVisualObject);
        }
    }


    private void UpdateValue(int index, int newXVal, int newYVal)
    {
        float yMinBefore, yMaxBefore;
        CalculateScale(out yMinBefore, out yMaxBefore, yList, yBuffer, yZeroStart);

        float xMinBefore, xMaxBefore;
        CalculateScale(out xMinBefore, out xMaxBefore, xList, xBuffer, xZeroStart);

        /*Debug.Log("Before " + xList[index].ToString());
        Debug.Log("Before " + yList[index].ToString());*/

        xList[index] = newXVal;
        yList[index] = newYVal;

        /*Debug.Log("After " + xList[index].ToString());
        Debug.Log("After " + yList[index].ToString());*/

        float yMin, yMax;
        CalculateScale(out yMin, out yMax, yList, yBuffer, yZeroStart);

        float xMin, xMax;
        CalculateScale(out xMin, out xMax, xList, xBuffer, xZeroStart);

        /*Debug.Log("Before X: Min: " + xMinBefore + " Max: " + xMaxBefore);
        Debug.Log("Before Y: Min: " + yMinBefore + " Max: " + yMaxBefore);

        Debug.Log("After X: Min: " + xMin + " Max: " + xMax);
        Debug.Log("After Y: Min: " + yMin + " Max: " + yMax);*/


        bool xScaleChanged = xMinBefore != xMin || xMaxBefore != xMax;
        bool yScaleChanged = yMinBefore != yMin || yMaxBefore != yMax;

        if(!yScaleChanged)
        {
            //Yscale did not change only change the given value
            float xPosition = ((newXVal - xMin) / (xMax - xMin)) * graphWidth;
            float yPosition = ((newYVal - yMin) / (yMax - yMin)) * graphHeight;

            //Add data point visual
            string tooltipText = "(" + newXVal.ToString() + ", " + newYVal.ToString() + ")";
            graphVisualObjectList[index].SetGraphVisualObjectInfo(new Vector2(xPosition, yPosition), tooltipText);
        } else
        {
            for (int i = 0; i < xList.Count; i++)
            {
                //x position would also need to be based off this value list
                /*Debug.Log("Graph Width: " + graphWidth.ToString());
                Debug.Log("Graph Height: " + graphHeight.ToString());*/

                float xPosition = ((xList[i] - xMin) / (xMax - xMin)) * graphWidth;
                float yPosition = ((yList[i] - yMin) / (yMax - yMin)) * graphHeight;

                /*Debug.Log("X: " + xPosition.ToString() + " Y: " + yPosition.ToString());*/

                string tooltipText = "(" + xList[i].ToString() + ", " + yList[i].ToString() + ")";
                graphVisualObjectList[i].SetGraphVisualObjectInfo(new Vector2(xPosition, yPosition), tooltipText);
            }

            for (int i = 0; i < yLabelList.Count; i++)
            {
                if (yScaleChanged)
                {
                    float normalizedYValue = (i * 1f) / yLabelList.Count;
                    float normalizedXValue = (i * 1f) / xLabelList.Count;
                    yLabelList[i].GetComponent<Text>().text = Mathf.RoundToInt(yMin + (normalizedYValue * (yMax - yMin))).ToString();
                    xLabelList[i].GetComponent<Text>().text = Mathf.RoundToInt(xMin + (normalizedXValue * (xMax - xMin))).ToString();
                }
            }
        }
    }

    //finds the extremas of the given list and provides a minimum and maxmium value, calculated to accound for the number padding.
    //out allows you to output values to variables, proving you the chance to update multiple values from a method output.
    private void CalculateScale(out float pushMin, out float pushMax, List<int> list, float padding, bool zeroStart)
    {
        //Place holder values for the minimum and maximum values
        float max = list[0];
        float min = list[0];

        //foreach loop runs through all int values in the list of integers
        foreach (int value in list)
        {
            if (value > max)
            {
                max = value;
            }
            if (value < min)
            {
                min = value;
            }
        }

        float difference = max - min;
        if(difference <= 0)
        {
            difference = 5f;
        }

        //Creates the buffer maximum and minimum values by adding or substracting by the difference between the max and min times the buffer ratio
        pushMax = max + (difference * padding);
        pushMin = min - (difference * padding);

        if(zeroStart)
        {
            pushMin = .0f; //Start the axis at 0.
        }
    }


    private class LineGraphVisual : IgraphVisual
    {

        private RectTransform graphContainer;
        private RectTransform pointTemplate;
        private RectTransform pointImageTemplate;

        private Sprite dotSprite;
        private LineGraphVisualObject lastLineGraphVisualObject;
        private Color dotColor;
        private Color dotConnectionColor;
        private int pointSize;

        public LineGraphVisual(RectTransform graphContainer, Sprite dotSprite, Color dotColor, Color dotConnectionColor, int pointSize)
        {
            this.graphContainer = graphContainer;

            pointTemplate = graphContainer.Find("PointTemplate").GetComponent<RectTransform>();
            pointImageTemplate = pointTemplate.Find("PointImageTemplate").GetComponent<RectTransform>();
            this.pointSize = pointSize;

            this.dotSprite = dotSprite;
            this.dotColor = dotColor;
            this.dotConnectionColor = dotConnectionColor;

            lastLineGraphVisualObject = null;
        }

        public void CleanUp()
        {
            lastLineGraphVisualObject = null;
        }

        public IgraphVisualObject CreateGraphVisualObject(Vector2 graphPosition, string tooltipText)
        {
            List<GameObject> gameObjectList = new List<GameObject>();

            GameObject dotGameObject = CreateDot(graphPosition, pointSize);

            GameObject dotConnectionObject = null;

            //Not the very first dot;
            if (lastLineGraphVisualObject != null)
            {
                dotConnectionObject = CreateDotConnection(lastLineGraphVisualObject.GetGraphPosition(), dotGameObject.GetComponent<RectTransform>().anchoredPosition);
            }

            LineGraphVisualObject lineGraphVisualObject = new LineGraphVisualObject(dotGameObject, dotConnectionObject, lastLineGraphVisualObject);
            lineGraphVisualObject.SetGraphVisualObjectInfo(graphPosition, tooltipText);

            //updates current circle to be the last circle to the next one
            lastLineGraphVisualObject = lineGraphVisualObject;

            return lineGraphVisualObject;
        }

        //Code Altered from CodeMonkey: Modified by Conan Enari
        private GameObject CreateDot(Vector2 anchoredPosition, int sizing)
        {
            RectTransform point = Instantiate(pointTemplate);
            point.SetParent(graphContainer, false);
            point.gameObject.SetActive(true);
            point.anchoredPosition = anchoredPosition;
            point.sizeDelta = new Vector2(sizing, sizing);
            point.anchorMin = new Vector2(0, 0);
            point.anchorMax = new Vector2(0, 0);

            RectTransform pointImage = Instantiate(pointImageTemplate);
            pointImage.SetParent(point, false);
            pointImage.gameObject.SetActive(true);
            pointImage.GetComponent<Image>().sprite = dotSprite;
            pointImage.GetComponent<Image>().color = dotColor;
            pointImage.sizeDelta = new Vector2(sizing, sizing);

            point.gameObject.AddComponent<Button_UI>();

            return point.gameObject;
        }

        private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
        {
            GameObject gameObject = new GameObject("dotConnection", typeof(Image));

            //To keep gameObject within the graphContainer object/panel.
            gameObject.transform.SetParent(graphContainer, false);
            gameObject.GetComponent<Image>().color = dotConnectionColor;
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

            //Distance and direction calculation
            Vector2 direction = (dotPositionB - dotPositionA).normalized;
            float distance = Vector2.Distance(dotPositionA, dotPositionB);

            //Sets the anchor points/orgin points at 0,0
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(distance, 3f);

            //moves the rectangle between the 2 points
            rectTransform.anchoredPosition = dotPositionA + direction * distance * .5f;

            //rotates the direction of the rectange to be able to connect it between the 2 points.
            rectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(direction));

            gameObject.transform.SetAsLastSibling();

            return gameObject;
        }

        public class LineGraphVisualObject : IgraphVisualObject
        {
            public event EventHandler OnChangedGraphVisualObjectInfo;

            private GameObject dotGameObject;
            private GameObject dotConnectionGameObject;
            private LineGraphVisualObject lastVisualObject;

            public LineGraphVisualObject(GameObject dotGameObject, GameObject dotConnectionGameObject, LineGraphVisualObject lastVisualObject)
            {
                this.dotGameObject = dotGameObject;
                this.dotConnectionGameObject = dotConnectionGameObject;
                this.lastVisualObject = lastVisualObject;

                if (lastVisualObject != null)
                {
                    lastVisualObject.OnChangedGraphVisualObjectInfo += LastVisualObject_OnChangedGraphVisualObjectInfo;
                }
            }
            public void CleanUp()
            {
                Destroy(dotGameObject);
                Destroy(dotConnectionGameObject);
            }

            private void LastVisualObject_OnChangedGraphVisualObjectInfo(object sender, EventArgs e)
            {
                UpdateDotConnection();
            }

            //Sets the new point's value
            public void SetGraphVisualObjectInfo(Vector2 graphPosition, string tooltipText)
            {
                RectTransform rectTransform = dotGameObject.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = graphPosition;
                /*rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);*/
                //Debug.Log(rectTransform.anchoredPosition);
                UpdateDotConnection();
                //Debug.Log(rectTransform.anchoredPosition);

                Button_UI dotButtonUI = dotGameObject.GetComponent<Button_UI>();

                dotButtonUI.MouseOverOnceFunc += () =>
                {
                    ShowToolTip_static(tooltipText, graphPosition);
                };

                dotButtonUI.MouseOutOnceFunc += () =>
                {
                    HideToolTip_static();
                };

                if (OnChangedGraphVisualObjectInfo != null) OnChangedGraphVisualObjectInfo(this, EventArgs.Empty);
            }

            public Vector2 GetGraphPosition()
            {
                RectTransform rectTransform = dotGameObject.GetComponent<RectTransform>();
                return rectTransform.anchoredPosition;
            }

            private void UpdateDotConnection()
            {
                if (dotConnectionGameObject != null)
                {
                    RectTransform dotConnectionRectTransform = dotConnectionGameObject.GetComponent<RectTransform>();

                    //Distance and direction calculation
                    Vector2 direction = (lastVisualObject.GetGraphPosition() - GetGraphPosition()).normalized;

                    float distance = Vector2.Distance(GetGraphPosition(), lastVisualObject.GetGraphPosition());

                    //Sets the anchor points/orgin points at 0,0
                    dotConnectionRectTransform.sizeDelta = new Vector2(distance, 3f);

                    //moves the rectangle between the 2 points
                    dotConnectionRectTransform.anchoredPosition = GetGraphPosition() + direction * distance * .5f;

                    //rotates the direction of the rectange to be able to connect it between the 2 points.
                    dotConnectionRectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(direction));
                }
            }
        }
    }

    private interface IgraphVisual
    {
        IgraphVisualObject CreateGraphVisualObject(Vector2 graphPosition, string tooltipText);
        void CleanUp();
    }

    private interface IgraphVisualObject
    {
        void SetGraphVisualObjectInfo(Vector2 graphPosition, string tooltipText);
        void CleanUp();
    }

    //a Static function so we dont need to make a instance of the class within the main showGraph function.
    public static void ShowToolTip_static(string tooltipText, Vector2 anchoredPosition)
        {
            instance.ShowToolTip(tooltipText, anchoredPosition);
        }
    public static void HideToolTip_static()
        {
            instance.HideToolTip();
        }

    private void ShowToolTip(string tooltipText, Vector2 anchoredPosition){
            //make tool visible
            tooltipObject.SetActive(true);

            //Get anchored/pivot position
            tooltipObject.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;

            //Set the text to the provided text;
            Text toolText = tooltipObject.transform.Find("Text").GetComponent<Text>();
            toolText.text = tooltipText;

            //buffer/padding space
            float textPaddingSize = 4f;

            //create the new preferred background size for the components
            Vector3 backgroundSize = new Vector2(
                toolText.preferredWidth + textPaddingSize * 2f, 
                toolText.preferredHeight  + textPaddingSize * 2f
            );

            tooltipObject.transform.Find("Background").GetComponent<RectTransform>().sizeDelta = backgroundSize;

            tooltipObject.transform.SetAsLastSibling();
        }

    private void HideToolTip()
        {
            tooltipObject.SetActive(false);
        }

}