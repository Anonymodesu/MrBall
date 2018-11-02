using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//script that all buttons in the main menu inherit from
public class Script_Menu_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	GameObject cubie;
    Transform cubieTransform;
    Transform canvasTransform;

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
		cubie = GameObject.Find("Cubie");
        cubieTransform = cubie.GetComponent<Transform>();
        canvasTransform = GetComponentInParent<RectTransform>();
	}
	
	public virtual void OnPointerEnter(PointerEventData eventData) {
        cubieTransform.position = transform.position + canvasTransform.rotation * Vector3.left * 10;
        cubieTransform.localScale = new Vector3(1.5f,1.5f,1.5f);
    }
	
	public void OnPointerExit(PointerEventData eventData) {		
		if(cubieTransform != null) {
            cubieTransform.position = transform.position + canvasTransform.rotation * Vector3.left * 10;
            cubieTransform.localScale = Vector3.zero;
        } else {
            Debug.Log("Fix bug in Script_MainMenu_Button in MouseLeave(). ");
        }
	}

	public void Exit() {
        Application.Quit();
    }
}
