using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class Window_Graph : MonoBehaviour{
    [SerializeField] private Sprite circleSprite;
    [SerializeField] private float xInterval;
    [SerializeField] private int yInterval;
    [SerializeField] private float xMax;
    [SerializeField] private float yMax; 

    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    private RectTransform pointTemplate;
    private RectTransform pointTextTemplate;
    private RectTransform pointImageTemplate;


    private void Awake()
    {
        graphContainer = transform.Find("GraphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("LabelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("LabelTemplateY").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.Find("DashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("DashTemplateY").GetComponent<RectTransform>();
        pointTemplate = graphContainer.Find("PointTemplate").GetComponent<RectTransform>();
        pointTextTemplate = pointTemplate.Find("PointTextTemplate").GetComponent<RectTransform>();
        pointImageTemplate = pointTemplate.Find("PointImageTemplate").GetComponent<RectTransform>();

        //CreateCircle(new Vector2(200, 200));
        //enter information from calculstions via serialzeField 
        List<int> xList = new List<int>() { 5, 10, 15, 17, 19, 24, 28, 35, 64, 76, 81, 90, 123, 134, 199};
        List<int> yList = new List<int>() { 5, 98, 56, 130, 29, 17, 15, 30, 109, 199, 187, 79, 150, 170, 10};
        ShowGraph(xList, yList);

    }

    private RectTransform CreateCircle(Vector2 anchoredPosition)
    {
        RectTransform point = Instantiate(pointTemplate);
        point.SetParent(graphContainer, false);
        point.gameObject.SetActive(true);
        point.anchoredPosition = anchoredPosition;
        point.sizeDelta = new Vector2(11, 11);
        point.anchorMin = new Vector2(0, 0);
        point.anchorMax = new Vector2(0, 0);

        RectTransform pointImage = Instantiate(pointImageTemplate);
        pointImage.SetParent(pointTemplate, false);
        pointImage.gameObject.SetActive(true);
        pointImage.GetComponent<Image>().sprite = circleSprite;
        pointImage.sizeDelta = new Vector2(11, 11);
        //pointImage.anchoredPosition = anchoredPosition;

        //RectTransform pointText = Instantiate(pointTextTemplate);
        //pointText.SetParent(pointTemplate, false);
        //pointText.gameObject.SetActive(true);
        //pointText.GetComponent<Text>().text = "(" + anchoredPosition.x.ToString() + ", " + anchoredPosition.y.ToString() + ")";
        //pointText.anchoredPosition = new Vector2(anchoredPosition.x, anchoredPosition.y + 5.5f);

        return point;
    }

    private void ShowGraph(List<int> xList, List<int> yList)
    {
        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;

        RectTransform lastCircleGameObject = null;
        for (int i = 0; i < yList.Count; i++)
        {
            //x position would also need to be based off this value list
            float xPosition = (xList[i] / xMax) * graphWidth;
            float yPosition = (yList[i] / yMax) * graphHeight;

            //GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            RectTransform circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));

            //Not the very first dot;
            if (lastCircleGameObject != null)
            {
                CreateDotConnection(lastCircleGameObject.anchoredPosition, circleGameObject.anchoredPosition);
            }

            //updates current circle to be the last circle to the next one
            lastCircleGameObject = circleGameObject;
        }

        for (int i = 0; i <= xInterval; i++)
        {
            float normalizedValue = (i * 1f) / xInterval;

            //X labels;
            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer, false);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2((normalizedValue * graphWidth), -7f);
            labelX.GetComponent<Text>().text = Mathf.RoundToInt(normalizedValue * yMax).ToString();

            //X Dashes:
            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(graphContainer, false);
            dashX.gameObject.SetActive(true);
            dashX.anchoredPosition = new Vector2((normalizedValue * graphWidth), -3f);
        }

        for (int i = 0; i <= yInterval; i++)
        {
            // value between 0 to 1; ratio
            float normalizedValue = (i * 1f) / yInterval;

            //Y labels
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer, false);
            labelY.gameObject.SetActive(true);
            labelY.anchoredPosition = new Vector2(-7f, (normalizedValue * graphHeight) + 15f);
            labelY.GetComponent<Text>().text = Mathf.RoundToInt(normalizedValue * yMax).ToString();

            //Y Dashes:
            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer, false);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(-4f, (normalizedValue * graphHeight));
        }
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        //To keep gameObject within the graphContainer object/panel.
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(255, 255, 0, .5f);
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


    }

}
