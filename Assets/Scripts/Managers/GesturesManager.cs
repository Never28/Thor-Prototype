﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GesturesManager : MonoBehaviour {

    public List<GestureContainer> gestures = new List<GestureContainer>();
    Dictionary<string, int> gestures_dict = new Dictionary<string, int>();

    public GameObject gesturesGrid;
    public GameObject gestureIconTemplate;
    public RectTransform gestureSelector;

    public int index;
    public string gestureAnim;
    public bool closeWeapons;


    void Start() {
        CreateGesturesUI();
    }

    public void SelectGestures(bool pos) {
        if (pos)
            index++;
        else
            index--;

        if (index < 0)
            index = gestures.Count - 1;
        if (index > gestures.Count - 1)
            index = 0;

        IconBase i = gestures[index].iconBase;
        gestureSelector.transform.SetParent(i.transform);
        gestureSelector.anchoredPosition = new Vector2(81,-50);

        gestureAnim = gestures[index].targetAnim;
        closeWeapons = gestures[index].closeWeapons;
    }

    public void HandleGestures(bool isOpen) {
        Debug.Log(isOpen);
        if (isOpen)
        {
            Debug.Log("open");
            if (gesturesGrid.activeInHierarchy == false)
            {
                Debug.Log("active");
                gesturesGrid.SetActive(true);
                gestureSelector.gameObject.SetActive(true);            
            }
        }
        else {
            if (gesturesGrid.activeInHierarchy == true)
            {
                gesturesGrid.SetActive(false);
                gestureSelector.gameObject.SetActive(false);
            
            }
        }
    }

    void CreateGesturesUI() {
        for (int i = 0; i < gestures.Count; i++)
        {
            GameObject go = Instantiate(gestureIconTemplate) as GameObject;
            go.transform.SetParent(gesturesGrid.transform);
            go.transform.localScale = Vector3.one;
            go.name = gestures[i].targetAnim;
            go.SetActive(true);
            IconBase icon = go.GetComponentInChildren<IconBase>();
            icon.icon.sprite = gestures[i].icon;
            icon.id = gestures[i].targetAnim;
            gestures[i].iconBase = icon;
        }

        gesturesGrid.SetActive(false);
        gestureSelector.gameObject.SetActive(false);
        index = 1;
        SelectGestures(false);
    }

    public GestureContainer GetGesture(string id)
    {
        int index = -1;
        if (gestures_dict.TryGetValue(id, out index)) {
            return gestures[index];
        }
        return null;
    }

    public static GesturesManager singleton;
    void Awake() {
        singleton = this;

        for (int i = 0; i < gestures.Count; i++)
        {
            if (gestures_dict.ContainsKey(gestures[i].targetAnim))
            {
                Debug.Log(gestures[i].targetAnim + " is a duplicate");
            }
            else {
                gestures_dict.Add(gestures[i].targetAnim,i);
            }
        }
    }

}

[System.Serializable]
public class GestureContainer
{
    public Sprite icon;
    public string targetAnim;
    public IconBase iconBase;
    public bool closeWeapons;
}