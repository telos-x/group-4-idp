using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelToggle : MonoBehaviour {
    
    [SerializeField] public GameObject Panel;
    [SerializeField] public GameObject PreviousPanel;


    public void OpenPanel()
    {
        if (Panel != null)
        {
            bool isSelfActive = Panel.activeSelf;
            bool isFormerActive = PreviousPanel.activeSelf;


            Panel.SetActive(!isSelfActive);
            PreviousPanel.SetActive(!isFormerActive);

        }
    }
}
