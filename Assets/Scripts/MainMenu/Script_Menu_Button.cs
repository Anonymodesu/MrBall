using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//script that all buttons in the main menu inherit from
public class Script_Menu_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    private Transform canvasTransform;

    private const float cubieSize = 0.1f;

	// Use this for initialization
	void Start () {
		init();
	}

	// Update is called once per frame
	/*
	void Update () {
		if(EventSystem.current.IsPointerOverGameObject()) {
			MouseHover();
		} else {
			//MouseLeave();
		}
	}
	*/
	
	public void init() {
        canvasTransform = GetComponentInParent<RectTransform>();
	}
	
	public virtual void OnPointerEnter(PointerEventData eventData) {

    }
	
	public virtual void OnPointerExit(PointerEventData eventData) {		

	}

	public void Exit() {
        Application.Quit();
    }
}
