﻿using UnityEngine;
using System.Collections;

public class ClipControl : MonoBehaviour {

	enum enAccelState{
		INIT,
		ACCEL1,
		ACCEL2
	};

	enum enRPSControlStates{
		CLIMBING,
		FALLING
	};
	enRPSControlStates RPSState;
	enAccelState state; 
	float oldxAxisAccel = 0;
	float currentThreshold = 0;
	float xAxisThreshold = 30;
	float yAxisThreshold = 70;
	float minPlaybackTime = 0.02f;
	float maxPlaybackTime = 0.98f;
	float PlaybackTimeDivider = 4;
	float currentMinPlaybackTime = 0;
	float currentMaxPlaybackTime = 0;
	float xAxisAccel = 0;
	float yAxisAccel = 0;
	float playbackTime = 0;


	bool play;
	// Use this for initialization
	void Start () {
		play = true;

		OSCHandler.Instance.Init();
		currentMinPlaybackTime = minPlaybackTime;
		currentMaxPlaybackTime = maxPlaybackTime / PlaybackTimeDivider;

		state = enAccelState.INIT;

		RPSState = enRPSControlStates.CLIMBING;
	}
	
	// Update is called once per frame
	void Update () {

		/* update the OSC handler */
		OSCHandler.Instance.UpdateLogs();

		/* update the accell variables */
		xAxisAccel = OSCHandler.Instance.GetX ();
		yAxisAccel = OSCHandler.Instance.GetY ();

		/* get current playback time */
		Animator animator = GetComponent<Animator> ();
		AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo (0);
		playbackTime = currentState.normalizedTime % 1;

		//Debug.Log (playbackTime + " " + xAxisAccel);

		updateRPS ();


	}

	private void updateRPS(){
		switch (RPSState) {
		case enRPSControlStates.CLIMBING:
			if (playbackTime < 0.97)
				RPS_Control();
			else
				RPSState = enRPSControlStates.FALLING;
			break;
		case enRPSControlStates.FALLING:
			if (playbackTime > 0.02) 
				MoveBackward();
			else 
				RPSState = enRPSControlStates.CLIMBING;
			break;
		}
	}
	private void RPS_Control(){
		if (OSCHandler.Instance.getRPS () > 3) {
			//move FW
			MoveForward();
		} else {
			//move backwards
			MoveBackward();
		}
	}
	private void MoveForward(){
		if (playbackTime < 0.97)
			GetComponent<Animator> ().speed = 3;
		else
			GetComponent<Animator> ().speed = 0;
	}
	private void MoveBackward(){
		if (playbackTime > 0.02) 
			GetComponent<Animator> ().speed = -3;
		else
			GetComponent<Animator> ().speed = 0;
	}

	private void SetSpeedSimple(){
		if ((xAxisAccel >= 50) && (playbackTime > 0.02)) {
			GetComponent<Animator> ().speed = -3;
		} else if ((xAxisAccel < 50) && (playbackTime < 0.99)) {
			GetComponent<Animator> ().speed = 3;
		} else {
			GetComponent<Animator> ().speed = 0;
		}
	}

	private void MouseControledSpeed(){

		if (Input.GetMouseButtonDown(0)) {
			Debug.Log ("Mouse Pressed");
			if (play) {
				GetComponent<Animator> ().speed = -3;
				play = false;
			} else {
				GetComponent<Animator> ().speed = 3;
				play = true;
			}
		}

	}

	private void Set2AxisPlaybackDividerSpeed(){

		Debug.Log ("Current State " + state);
		Debug.Log("TimeInterval " + currentMinPlaybackTime + "-" + currentMaxPlaybackTime);
		//Debug.Log ("CurPlaybakTime " + playbackTime);
		//Debug.Log ("Accx " + xAxisAccel + " ThresX " + xAxisThreshold);
		//Debug.Log ("AccY " + yAxisAccel + " ThresY " + yAxisThreshold);

		if (playbackTime < 0 )
			GetComponent<Animator> ().Play("Take 001", 0, 0.02f);
	
		switch (state) {
		case enAccelState.INIT:
			currentMinPlaybackTime = minPlaybackTime;
			currentMaxPlaybackTime = maxPlaybackTime / PlaybackTimeDivider;
			state = enAccelState.ACCEL2;
			break;
		case enAccelState.ACCEL1:
			if ((xAxisAccel < 50)){
				state = enAccelState.ACCEL2;

			}
			else{

				if((playbackTime >= currentMinPlaybackTime) &&
				   (playbackTime < currentMaxPlaybackTime)){
					//move backward
					GetComponent<Animator> ().speed = -3;
				}
				else if (playbackTime >= currentMaxPlaybackTime){
					/* update the playback thresholds */
					currentMinPlaybackTime = currentMaxPlaybackTime;
					currentMaxPlaybackTime = maxPlaybackTime/PlaybackTimeDivider;
					PlaybackTimeDivider--;
					if (PlaybackTimeDivider == 0){
						/* start a new game switch to the INIT state */
						state = enAccelState.INIT;
					}
					else{
						/* switch to the ACCEL2 state */
						state = enAccelState.ACCEL2;
					}
				}
			}
			break;
		case enAccelState.ACCEL2:
			if (yAxisAccel > 50){
				state = enAccelState.ACCEL1;

			}
			else{

				if((playbackTime >= currentMinPlaybackTime) &&
				   (playbackTime < currentMaxPlaybackTime)){
					//move forward
					GetComponent<Animator> ().speed = 3;
				}
				else if (playbackTime >= currentMaxPlaybackTime){
					/* update the playback thresholds */
					currentMinPlaybackTime = currentMaxPlaybackTime;
					currentMaxPlaybackTime = maxPlaybackTime/PlaybackTimeDivider;
					PlaybackTimeDivider--;
					if (PlaybackTimeDivider == 0){
						/* start a new game switch to the INIT state */
						state = enAccelState.INIT;
					}
					else{
						/* switch to the ACCEL2 state */
						state = enAccelState.ACCEL1;
					}
				}
			}
			break;
		}

	}

}
