using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [HideInInspector]
    public bool UseAr = false;

    public GameObject ButtonSubway;
    public GameObject ButtonCity;
    public GameObject ButtonAR;
    public GameObject Button3D;

    public Color ActiveButtonColor = Color.white;
    public Color DefaultButtonColor = Color.red;
    public Color ActiveTextButtonColor = Color.red;
    public Color DefaultTextButtonColor = Color.white;

    public GameObject PanelSubway;
    public GameObject PanelCity;
    public GameObject PanelAR;
    public GameObject Panel3D;

    public GameObject ImageSubwayMap;
    public GameObject ImageCityMap;
    public GameObject Model3D;
    public GameObject ModelAr;

    private int m_ActivePosition = 0;

    private void Start()
    {
        m_ActivePosition = 0;
        SetActive(m_ActivePosition);
    }

    public void SetActive(int position)
    {
        m_ActivePosition = position;
        Clear();
        switch (position)
        {
            case (0):
                {
                    SetActiveButton(ButtonSubway, true);
                    SetActivePanel(PanelSubway, true);
                    SetActiveObject(ImageSubwayMap, true);
                    break;
                }
            case (1):
                {
                    SetActiveButton(ButtonCity, true);
                    SetActivePanel(PanelCity, true);
                    SetActiveObject(ImageCityMap, true);
                    break;
                }
            case (2):
                {
                    SetActiveButton(ButtonAR, true);
                    SetActivePanel(PanelAR, true);
                    SetActiveObject(ModelAr, true);
                    UseAr = true;
                    break;
                }
            case (3):
                {
                    SetActiveButton(Button3D, true);
                    SetActivePanel(Panel3D, true);
                    SetActiveObject(Model3D, true);
                    break;
                }
        }
    }

    private void Clear()
    {
        UseAr = false;

        SetActiveButton(ButtonSubway);
        SetActiveButton(ButtonCity);
        SetActiveButton(ButtonAR);
        SetActiveButton(Button3D);

        SetActivePanel(PanelSubway);
        SetActivePanel(PanelCity);
        SetActivePanel(PanelAR);
        SetActivePanel(Panel3D);

        SetActiveObject(ImageCityMap);
        SetActiveObject(ImageSubwayMap);
        SetActiveObject(Model3D);
        SetActiveObject(ModelAr);
    }

    private void SetActiveButton(GameObject button, bool isActive = false)
    {
        Color buttonColor;
        Color textColor;
        if (isActive)
        {
            buttonColor = ActiveButtonColor;
            textColor = ActiveTextButtonColor;
        }
        else
        {
            buttonColor = DefaultButtonColor;
            textColor = DefaultTextButtonColor;
        }
        button.GetComponent<Image>().color = buttonColor;
        button.GetComponentInChildren<TextMeshProUGUI>().color = textColor;
    }

    private void SetActivePanel(GameObject panel, bool isActive = false)
    {
        panel.SetActive(isActive);
    }

    private void SetActiveObject(GameObject obj, bool isActive = false)
    {
        obj.SetActive(isActive);
    }
}
