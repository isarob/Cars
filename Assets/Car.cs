﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour {

	public float speedo;
	public bool isGrounded;
	public WheelCollider[] wheels;
	[Tooltip("0: static\n1: powered\n2: steering\n3: powered + steering")]
	public int[] wheelProperties; //0: static, 1: powered, 2: steering, 3: powered + steering
	public float steerLimit;
	public float brakeTorque;
	public float boost;
	public float handbrakeSlide;
	public float stiffness;
	public float airControl;
	public GameObject COM;
	public AnimationCurve speed = AnimationCurve.Linear (0, 200, 200, 0);
	public AnimationCurve downforce = AnimationCurve.Linear (0, 0, 10, 10);
	private LineRenderer[] lr;
	public LineRenderer skid;
	public Color skidColour;
	private Vector3[] prevSkid;
	public GameObject[] brakeLights;
	public GameObject[] boostLights;

	public GameObject cam;
	private Vector3 prevPos = Vector3.zero;
	private Vector3 prevUp = Vector3.up;
	// Use this for initialization
	void Start (){
		prevSkid = new Vector3[wheels.Length];
		lr = new LineRenderer[wheels.Length];
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//cam.transform.localEulerAngles = Vector3.Scale(cam.transform.localEulerAngles,Vector3.up);
		var followCam = -Vector3.forward*8+Vector3.up*2;
		//print((transform.position-prevPos).magnitude);
		if (isGrounded) {
			prevUp = (transform.up*Time.deltaTime*10+prevUp).normalized;
			cam.transform.LookAt(COM.transform.position,prevUp);
			cam.transform.parent = this.transform;
			if  ((COM.transform.position-prevPos).magnitude < 0.3f) {
				followCam = -Vector3.forward*8+Vector3.up*2;
			}else{
				followCam = (Vector3.ProjectOnPlane(COM.transform.InverseTransformPoint(prevPos),Vector3.up).normalized*8 + Vector3.up * 2);
			}
			cam.transform.localPosition = Vector3.MoveTowards(cam.transform.localPosition,followCam-COM.transform.localPosition,6f*Time.deltaTime);
		}else{
			prevUp = (Vector3.up*Time.deltaTime+prevUp).normalized;
			cam.transform.LookAt(COM.transform.position,prevUp);
			cam.transform.parent = null;
			Debug.DrawLine(Vector3.ProjectOnPlane(prevPos,Vector3.up),Vector3.zero,Color.red);
			var fp = (COM.transform.position+((Vector3.Scale(prevPos-COM.transform.position,new Vector3(1,.3f,1))).normalized*8));
		
			var up = ((Vector3.up * 2));
			followCam = fp+up;
			cam.transform.position += COM.transform.position-prevPos;
			cam.transform.position = Vector3.MoveTowards(cam.transform.position,followCam-COM.transform.localPosition,8f*Time.deltaTime);
		}
		
		prevPos = COM.transform.position;
	}
	void Update () {
		isGrounded = false;
		var velocity = ((Vector3.Dot(GetComponent<Rigidbody>().velocity,transform.forward)*3.6f));
		speedo = velocity;
		GetComponent<Rigidbody>().centerOfMass = COM.transform.localPosition;
		//print (velocity);
		for(int i = 0; i < wheels.Length; i++){
			var wheel = wheels[i];
			var prop = wheelProperties [i];
			WheelHit wh;
			if (wheel.GetGroundHit (out wh)) {
				isGrounded = true;
				var whAbove = wh.point + new Vector3 (0, 0.05f, 0);
				Debug.DrawLine (wh.point,wh.point+wh.normal*10,Color.red);
				if (Mathf.Clamp01 ((Mathf.Abs ((wh.sidewaysSlip + (wh.forwardSlip) / 2)) - 0.3f) / 0.7f) != 0) {
					
					//new linerenderer
					if (lr [i] == null || lr [i].positionCount > 7) {
						lr [i] = Instantiate (skid);
						lr [i].positionCount = 2;
						lr [i].transform.position = whAbove;
						lr [i].transform.rotation = Quaternion.LookRotation(wh.normal,transform.up);
						lr [i].transform.parent = wh.collider.transform;


						if (prevSkid [i] != Vector3.zero) {
							lr [i].SetPosition (0, lr[i].transform.InverseTransformPoint(prevSkid [i]));
						} else {
							lr [i].SetPosition (0, Vector3.zero);
						}
						lr [i].SetPosition (1, lr[i].transform.InverseTransformPoint(whAbove));

						var gradient = new Gradient ();
						var colorKey = new GradientColorKey[2];
						var alphaKey = new GradientAlphaKey[2];
						colorKey [0].color = skidColour;
						colorKey [0].time = 0;
						colorKey [1].color = skidColour;
						colorKey [1].time = 1;
						alphaKey [0].alpha = Mathf.Clamp01 ((Mathf.Abs ((wh.sidewaysSlip + (wh.forwardSlip) / 2)) - 0.3f) / 0.7f);
						alphaKey [0].time = 0;
						alphaKey [1].alpha = Mathf.Clamp01 ((Mathf.Abs ((wh.sidewaysSlip + (wh.forwardSlip) / 2)) - 0.3f) / 0.7f);
						alphaKey [1].time = 1;
						gradient.SetKeys (colorKey, alphaKey);
						lr [i].colorGradient = gradient;
						lr [i].gameObject.SetActive (true);
					} else {
						var gradient = new Gradient ();
						var colorKey = new GradientColorKey[lr [i].positionCount + 1];
						var alphaKey = new GradientAlphaKey[lr [i].positionCount + 1];
						for (int j = 0; j < lr [i].positionCount; j++) {
							colorKey [j].time = j / (lr [i].positionCount);
							colorKey [j].color = skidColour;
							alphaKey [j].time = j / (lr [i].positionCount);
							if (j < lr [i].positionCount - 1) {
								alphaKey [j].alpha = lr [i].colorGradient.alphaKeys [j].alpha;
							} else {
								alphaKey [j].alpha = Mathf.Clamp01 ((Mathf.Abs ((wh.sidewaysSlip + (wh.forwardSlip) / 2)) - 0.3f) / 0.7f);
							}
						}
						gradient.SetKeys (colorKey, alphaKey);
						lr [i].colorGradient = gradient;
						lr [i].positionCount += 1;
						lr [i].SetPosition (lr [i].positionCount - 1, lr[i].transform.InverseTransformPoint(whAbove));
					}
					prevSkid [i] = whAbove;
				} else {
					lr [i] = null;
					prevSkid[i] = Vector3.zero;
				}
			} else {
				lr [i] = null;
				prevSkid[i] = Vector3.zero;
			}

			if (prop == 2 || prop == 3) {
				wheel.steerAngle = Input.GetAxis ("Horizontal") * steerLimit;
			}
			if (prop == 1 || prop == 3 || prop == -2 || prop == -3) {
				wheel.motorTorque = Input.GetAxis ("Vertical")*speed.Evaluate(Vector3.Dot(GetComponent<Rigidbody>().velocity,transform.forward));
			}
			if (prop == -2 || prop == -3) {
				wheel.steerAngle = Input.GetAxis ("Horizontal") * -steerLimit;
			}
			GetComponent<Rigidbody> ().AddForce (-transform.up*downforce.Evaluate(Mathf.Abs(velocity))*Mathf.Sign(velocity));
			if (!isGrounded) {
				GetComponent<Rigidbody> ().AddRelativeTorque (Input.GetAxis ("Vertical") * airControl * Vector3.right);
				GetComponent<Rigidbody> ().AddRelativeTorque (Input.GetAxis ("Horizontal") * airControl * Vector3.up);
				GetComponent<Rigidbody> ().AddRelativeTorque (Input.GetAxis ("Roll") * airControl * Vector3.forward);
			} else {
				
			}


			if (Input.GetKey (KeyCode.LeftShift)) {
				GetComponent<Rigidbody> ().AddForce (transform.forward * boost);
				foreach (GameObject light in boostLights) {
					light.GetComponent<Renderer> ().material.SetColor ("_EmissionColor", new Color(50/255, 255/255, 255/255));
				}
			} else {
				foreach(GameObject light in boostLights){
					light.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",Color.black);
				}
			}


			if (Input.GetKey (KeyCode.Space)) {

				/*
				if (wheel.motorTorque - brakeTorque > 0) {
					wheel.brakeTorque = 0;
				} else {
					
				}

				*/
				//SideFriction
				wheel.brakeTorque = brakeTorque;
				WheelFrictionCurve sfc = wheel.sidewaysFriction;
				sfc.stiffness = handbrakeSlide;
				wheel.sidewaysFriction = sfc;
				//FrontFriction
				WheelFrictionCurve ffc = wheel.forwardFriction;
				ffc.stiffness = handbrakeSlide;
				wheel.forwardFriction = ffc;

				foreach(GameObject light in brakeLights){
					light.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",Color.red);
				}
			} else {
				wheel.brakeTorque = 0;
				//SideFriction
				WheelFrictionCurve sfc = wheel.sidewaysFriction;
				sfc.stiffness = stiffness;
				wheel.sidewaysFriction = sfc;
				//FrontFriction
				WheelFrictionCurve ffc = wheel.forwardFriction;
				ffc.stiffness = 1.8f;
				wheel.forwardFriction = ffc;

				if (Input.GetAxis ("Vertical") < 0) {
					foreach(GameObject light in brakeLights){
						light.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",Color.red);
					}
				} else {
					foreach(GameObject light in brakeLights){
						light.GetComponent<Renderer> ().material.SetColor ("_EmissionColor",Color.black);
					}
				}
			}
		}

	

	}
}
