using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Menu_Camera : Script_Camera {

	#pragma warning disable 0649

    [SerializeField]
    private List<GameObject> menus;

    #pragma warning restore 0649

    //distance from the camera to a canvas
    private const float distance = 2;
    private const float rotationSpeed = 1;
    private readonly Vector3 offset = Vector3.up * 0.2f;

	// Use this for initialization
	void Start () {
        Time.timeScale = 1;
        GameManager.getInstance();

        transform.position = Random.insideUnitSphere * 0.5f;
        transform.rotation = Quaternion.Euler(Random.insideUnitSphere * 180);
        StartCoroutine(switchMenus(menus[0].transform));
	}
    
    //switches view from current menu to dest menu
    public IEnumerator switchMenus(Transform dest) {
       foreach(GameObject menu in menus) {
       		menu.GetComponent<CanvasGroup>().interactable = false;
       }

       Vector3 sourcePos = transform.position;
       Vector3 targetPos = dest.position - distance * dest.forward + offset;
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

       dest.GetComponent<CanvasGroup>().interactable = true;
    }
}
