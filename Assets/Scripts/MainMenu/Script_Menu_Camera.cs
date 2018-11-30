using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Script_Menu_Camera : Script_Camera {

    private EventSystem eventSystem;

    //distance from the camera to a canvas
    private const float distance = 2;
    private const float rotationSpeed = 1;

	// Use this for initialization
	void Start () {
        Time.timeScale = 1;
        GameManager.getInstance();
        eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

        transform.position = Random.insideUnitSphere * 0.5f;
        transform.rotation = Quaternion.Euler(Random.insideUnitSphere * 180);
        StartCoroutine(switchMenus(GameObject.Find("Menu").transform));
	}
    
    //switches view from current menu to dest menu
    public IEnumerator switchMenus(Transform dest) {
       eventSystem.enabled = false; //prevent other buttons from being pressed

       Vector3 sourcePos = transform.position;
       Vector3 targetPos = dest.position - distance * dest.forward;
       Quaternion sourceRot = transform.rotation;
       Quaternion targetRot = Quaternion.LookRotation(dest.forward);

       float step = 0;
       while(step < 1) {
            transform.position = Vector3.Lerp(sourcePos, targetPos, step);
            transform.rotation = Quaternion.Lerp(sourceRot, targetRot, step);

            step += rotationSpeed * Time.deltaTime;
            yield return null;
       }

       transform.position = targetPos;
       transform.rotation = targetRot;

       eventSystem.enabled = true;
    }
}
