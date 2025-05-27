using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class Window_Graph : MonoBehaviour{
    [SerializeField] private Sprite PointSprite;
    [SerializeField] private float xInterval;
    [SerializeField] private int yInterval;
    [SerializeField] private float xMax;

    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    private List<GameObject> gameObjectList; 
    private GameObject tooltipObject;


    private void Awake()
    {
        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("LabelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("LabelTemplateY").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.Find("DashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("DashTemplateY").GetComponent<RectTransform>();
        tooltipObject = graphContainer.Find("ToolTip").gameObject;


        gameObjectList = new List<GameObject>();

        //CreateCircle(new Vector2(200, 200));
        //enter information from calculstions via serialzeField 
        List<int> xList = new List<int>() { 5, 10, 15, 17, 19, 24, 28, 35, 64, 76, 81, 90, 123, 134, 199};
        List<int> yList = new List<int>() { 5, 98, 56, 130, 29, 17, 15, 30, 109, 199, 187, 79, 150, 170, 10};
        ShowGraph(xList, yList);

        // List<int> xList2 = new List<int>() { 10, 20, 25, 27, 29, 24, 28, 35, 64, 84, 86, 100, 134, 150};
        // List<int> yList2 = new List<int>() { 10, 98, 56, 130, 29, 17, 35, 40, 112, 186, 100, 79, 150, 180};
        // ShowGraph(xList2, yList2);


        // FunctionPeriodic.Create(() =>
        // {
        //     xList.Clear(); 
        //     yList.Clear(); 
        //     for (int i = 0; i < 15; i++)
        //     {
        //         xList.Add(UnityEngine.Random.Range(0, 200));
        //         yList.Add(UnityEngine.Random.Range(0, 200));
        //     }
        //     ShowGraph(xList, yList);
        // }, .5f);


    }

    private void ShowGraph(List<int> xList, List<int> yList)
    {
        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;

        float yMax = yList[0];
        float yMin = yList[0];

        float xMax = xList[0];
        float xMin = xList[0];

        foreach(GameObject gameObject in gameObjectList)
        {
            Destroy(gameObject);
        }
        gameObjectList.Clear();

        foreach (int value in yList)
        {
            if(value > yMax)
            {
                yMax = value;
            }
            if(value < yMin)
            {
                yMin = value;
            }
        }
        yMax += (yMax - yMin) * 0.2f;
        yMin -= (yMax - yMin) * 0.2f;

        foreach (int value in xList)
        {
            if(value > xMax)
            {
                xMax = value;
            }
            if(value < xMin)
            {
                xMin = value;
            }
        }
        xMax += (xMax - xMin) * 0.05f;
        xMin -= (xMax - xMin) * 0.05f;

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

            gameObjectList.Add(labelY.gameObject);

            //Y Dashes:
            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer, false);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(-4f, (normalizedValue * graphHeight));
            gameObjectList.Add(dashY.gameObject);
        }

        //Create Graph
        LineGraphVisual lineChartVisual = new LineGraphVisual(graphContainer, PointSprite, Color.green, new Color(1, 1, 1, .5f));
        for (int i = 0; i < xList.Count; i++)
        {
               //x position would also need to be based off this value list
            float xPosition = ((xList[i] - xMin)/ (xMax - xMin)) * graphWidth;
            float yPosition = ((yList[i] - yMin)/ (yMax - yMin)) * graphHeight;

            //GameObject circleGameObject = CreateDot(new Vector2(xPosition, yPosition));
            gameObjectList.AddRange(lineChartVisual.AddGraphVisual(new Vector2(xPosition, yPosition), xPosition, yPosition));
        }

        ShowToolTip("this is a tooltip", new Vector2( 100, 100));

        FunctionPeriodic.Create(() =>
        {
            ShowToolTip("this is a tooltip" + UnityEngine.Random.Range(100, 40000), new Vector2( 100, 100));
        }, .1f);
    }

    private class LineGraphVisual{

        private RectTransform graphContainer; 
        private RectTransform pointTemplate;
        private RectTransform pointTextTemplate;
        private RectTransform pointImageTemplate;

        private Sprite dotSprite;
        private GameObject lastDotObject;
        private Color dotColor;
        private Color dotConnectionColor;

        public LineGraphVisual(RectTransform graphContainer, Sprite dotSprite, Color dotColor, Color dotConnectionColor)
        {
            this.graphContainer = graphContainer;

            pointTemplate = graphContainer.Find("PointTemplate").GetComponent<RectTransform>();
            // pointTextTemplate = pointTemplate.Find("PointTextTemplate").GetComponent<RectTransform>();
            pointImageTemplate = pointTemplate.Find("PointImageTemplate").GetComponent<RectTransform>();

            this.dotSprite = dotSprite;
            this.dotColor = dotColor;
            this.dotConnectionColor = dotConnectionColor;
            lastDotObject = null;
        }

        public List<GameObject> AddGraphVisual(Vector2 graphPosition, float xVal, float yVal)
        {
            List<GameObject> gameObjectList = new List<GameObject>();
            GameObject dotGameObject = CreateDot(graphPosition, xVal, yVal);
            gameObjectList.Add(dotGameObject);

            //Not the very first dot;
            if (lastDotObject != null)
            {
                GameObject dotConnectionObject = CreateDotConnection(lastDotObject.GetComponent<RectTransform>().anchoredPosition, dotGameObject.GetComponent<RectTransform>().anchoredPosition);
                gameObjectList.Add(dotConnectionObject);
            }

            //updates current circle to be the last circle to the next one
            lastDotObject = dotGameObject; 
            return gameObjectList;
        }

        //Code Altered from CodeMonkey: Modified by Conan Enari
        private GameObject CreateDot(Vector2 anchoredPosition, float xVal, float yVal)
        {
            RectTransform point = Instantiate(pointTemplate);
            point.SetParent(graphContainer, false);
            point.gameObject.SetActive(true);
            point.anchoredPosition = anchoredPosition;
            point.sizeDelta = new Vector2(11, 11);
            point.anchorMin = new Vector2(0, 0);
            point.anchorMax = new Vector2(0, 0);

            RectTransform pointImage = Instantiate(pointImageTemplate);
            pointImage.SetParent(point, false);
            pointImage.gameObject.SetActive(true);
            pointImage.GetComponent<Image>().sprite = dotSprite;
            pointImage.GetComponent<Image>().color = dotColor;
            pointImage.sizeDelta = new Vector2(11, 11);

            // RectTransform pointText = Instantiate(pointTextTemplate);
            // pointText.SetParent(point, false);
            // pointText.gameObject.SetActive(true);
            // pointText.GetComponent<Text>().text = "(" + xVal + ", " + yVal + ")";
            // // Debug.Log("(" + anchoredPosition.x + ", " + (anchoredPosition.y + 10f) + ")");

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
    }

    private interface IgraphVisual
    {
        
    }

    private void ShowToolTip(string tooltipText, Vector2 anchoredPosition){
        //make tool visible
        tooltipObject.SetActive(true);

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
