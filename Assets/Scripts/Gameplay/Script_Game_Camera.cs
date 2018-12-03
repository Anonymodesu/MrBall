using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Game_Camera : Script_Camera {
    private float mouseSensitivity;
    private GameObject player;
	private Vector3 rotation; // vector representing an Euler rotation in the standard basis; 

	private Vector3 xAxis;
	private Vector3 yAxis;
	private Vector3 zAxis; //the current coordinate system being used, relevant when gravity changes
	private const float gravityChangeSpeed = 0.1f; //how quickly the camera adjusts to new gravities

	//upper half rotation is from 0 at the middle down to -90 at the top
	private const float upperLimit = -89; 
	private const float lowerLimit = 89;
	//lower half rotation is from 0 at middle down to 90 at the bottom

	//determines distance from camera to the ball
	private float currentOffset; //overall distance
	private const float baseOffset = 1;
	private float yDistance;
	private float zDistance;
	
	//moving quickly warps the camera; increasing this value decreases warp delay
	private const float warpSpeed = 0.1f;
	private const float baseFOV = 65; //fov at 0 velocity

	//variables for hiding ramps
	private const int rampLayerMask = 1 << 8;
	private Dictionary<GameObject, GameObject> hiddenRamps;
	private const float transparency = 0.5f;

    // Use this for initialization
    void Start () {
        mouseSensitivity = SettingsManager.CameraSensitivity;
        player = GameObject.FindWithTag("Player");
        currentOffset = baseOffset;
		rotation = Vector3.zero;
        //transform.position = GameObject.Find("Ramp_Start").GetComponent<Transform>().position + Vector3.up;

        yAxis = Vector3.up;
        xAxis = Vector3.right;
        zAxis = Vector3.forward;

        //get camera positioning settings
        zDistance = SettingsManager.ForwardDistance;
        yDistance = SettingsManager.UpwardDistance;

        hiddenRamps = new Dictionary<GameObject, GameObject>();
    }
    
    // Follow camera
    void LateUpdate () {

		//Debug.DrawLine(player.transform.position,player.transform.position+xAxis,Color.red);
		//Debug.DrawLine(player.transform.position,player.transform.position+yAxis,Color.blue);
		//Debug.DrawLine(player.transform.position,player.transform.position+zAxis,Color.black);

		updateAxes();
    	updateRotation();
    	updatePosition();

    	processCollider();
    	
		//Debug.Log(Quaternion.LookRotation(-transform.forward).eulerAngles);	
    }

    //there is a ramp obstructing view
    void OnTriggerEnter(Collider other) {
    	GameObject ramp = other.gameObject;

		//create temporary transparent ramp
		GameObject transparentRamp = Instantiate(ramp, ramp.transform.parent);

		//so that it is ignored by everything; only its renderer is needed
		transparentRamp.GetComponent<Collider>().enabled = false; 

		//make it transparent
		//see https://answers.unity.com/questions/1004666/change-material-rendering-mode-in-runtime.html
		Material mat = transparentRamp.GetComponent<Renderer>().material;
		mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
		mat.SetColor("_Color", new Color(mat.color.r, mat.color.g, mat.color.b, transparency));

		//hide original ramp
		hiddenRamps.Add(ramp, transparentRamp);
		ramp.GetComponent<Renderer>().enabled = false; //make it invisible	
    }

    //the ramp is no longer obstructing view
    void OnTriggerExit(Collider other) {
    	GameObject ramp = other.gameObject;
    	GameObject transparentRamp = hiddenRamps[ramp];
    	hiddenRamps.Remove(ramp);

    	//destroy the temporary ramp
    	Destroy(transparentRamp.GetComponent<Renderer>().material);
    	Destroy(transparentRamp);

    	ramp.GetComponent<Renderer>().enabled = true;
    }

    private void updateRotation() {
		//deltatime is still required to freeze camera during pause
		rotation.x += mouseSensitivity*InputManager.getInput().yAxisMovement() * Time.deltaTime * 60;
		rotation.y += mouseSensitivity*InputManager.getInput().xAxisMovement() * Time.deltaTime * 60; 
		rotation.z = 0;
		
		//do not allow rotation past the limits
		if(rotation.x < upperLimit && rotation.x > -180) {
			rotation.x = upperLimit;
			
		} else if(rotation.x > lowerLimit && rotation.x < 180) {
			rotation.x = lowerLimit;
		} 
		
		if(rotation.y > 360 || rotation.y < -360) {
			rotation.y = 0;
		}
		
		Vector3 temp = Quaternion.AngleAxis(rotation.y, yAxis) * Quaternion.AngleAxis(rotation.x, xAxis) * zAxis;
		transform.rotation = Quaternion.LookRotation(temp, yAxis);
    }

    private void updatePosition() {
    	//FOV widens as velocity increases
    	float ballSpeed = Vector3.Dot(player.GetComponent<Rigidbody>().velocity, transform.forward);
		float prevFOV = GetComponent<Camera>().fieldOfView;
		float nextFOV = baseFOV - ballSpeed;
		GetComponent<Camera>().fieldOfView = Mathf.Lerp(prevFOV, nextFOV, warpSpeed * Time.deltaTime * 60);

		float nextOffset = baseOffset / (1 - ballSpeed * 0.01f);//camera gets closer as speed increases
		currentOffset = Mathf.Lerp(currentOffset, nextOffset, warpSpeed * Time.deltaTime * 60);

		transform.position = player.transform.position //transform.forward is always normalized
							+ (yAxis * yDistance - transform.forward * zDistance) //position the camera behind the ball 
							* currentOffset;
		
    }
	
	private void updateAxes() {

		Vector3 targetYAxis = -Physics.gravity.normalized;

		if(yAxis == targetYAxis) { //no change in axes registered
			return;

		} else if (Vector3.Angle(yAxis, targetYAxis) < 1) {
			yAxis = targetYAxis;

		} else { //shift the yAxis towards the gravity vector a little
			Quaternion rotation = Quaternion.FromToRotation(yAxis, targetYAxis);
			rotation = Quaternion.Lerp(Quaternion.identity, rotation, gravityChangeSpeed * Time.deltaTime * 60);
			yAxis = rotation * yAxis;
		}

		xAxis = Vector3.Cross(yAxis, transform.forward).normalized;
		while(xAxis.Equals(Vector3.zero)) {
			xAxis = Vector3.Cross(yAxis, Random.insideUnitSphere);
			Debug.Log("xAxis is 0");

			if(yAxis.Equals(Vector3.zero)) { //to avoid infinite loops
				Debug.Log("yAxis is 0");
				break;
			}
		}

		zAxis = Vector3.Cross(xAxis, yAxis).normalized;

		rotation = new Vector3(rotation.x, 0, 0);
	}

	//ensures that the collider always spans the length between the camera and the ball
    private void processCollider() {
		Transform collider = GetComponentInChildren<BoxCollider>().transform;
		collider.position = (transform.position + player.transform.position) / 2;
		collider.rotation = Quaternion.LookRotation(player.transform.position - transform.position);
		collider.localScale = new Vector3(collider.localScale.x, collider.localScale.y, 
										0.9f * (player.transform.position - transform.position).magnitude);
    }

	
	// returns the projection of the camera's current forward facing direction onto the x-z plane in the current coordinate basis
	public Vector3 forwardVector() {
		return Vector3.ProjectOnPlane(transform.forward, Physics.gravity);
	}
	
	// returns the projection of camera's current right facing direction onto the x-z plane in the current coordinate basis
	public Vector3 rightVector() {
		return Vector3.ProjectOnPlane(transform.right, Physics.gravity);
	}
	
	
}
