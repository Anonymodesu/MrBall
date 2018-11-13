using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//script that all buttons in the main menu inherit from
public class Script_Menu_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	private GameObject cubie;
    private Transform cubieTransform;
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
		cubie = GameObject.Find("Cubie");
        cubieTransform = cubie.GetComponent<Transform>();
        canvasTransform = GetComponentInParent<RectTransform>();
        cubieTransform.localScale = cubieSize * Vector3.one;
	}
	
	public virtual void OnPointerEnter(PointerEventData eventData) {
        cubieTransform.position = transform.position + transform.rotation * Vector3.right * 0.5f;
        cubieTransform.gameObject.SetActive(true);
    }
	
	public void OnPointerExit(PointerEventData eventData) {		
		if(cubieTransform != null) {
            cubieTransform.gameObject.SetActive(false);
        } else {
            Debug.Log("Fix bug in Script_MainMenu_Button in MouseLeave(). ");
        }
	}

	public void Exit() {
        Application.Quit();
    }
}
