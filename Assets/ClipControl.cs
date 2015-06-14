using UnityEngine;
using System.Collections;
using SystemDefs;

public class ClipControl : MonoBehaviour {

	enum enAccelState{
		INIT,
		ACCEL1,
		ACCEL2
	};

	/*
	enum enRPSControlStates{
		STANDBY,
		CLIMBING,
		FALLING
	};
	enRPSControlStates RPSState;
	*/
	enAccelState state; 
	float oldxAxisAccel = 0;
	float currentThreshold = 0;
	float xAxisThreshold = 30;
	float yAxisThreshold = 70;
	float minPlaybackTime = 0.05f;
	float maxPlaybackTime = 0.97f;
	float PlaybackTimeDivider = 4;
	float currentMinPlaybackTime = 0;
	float currentMaxPlaybackTime = 0;
	float xAxisAccel = 0;
	float yAxisAccel = 0;
	float playbackTime = 0;


	bool play;
	bool manual;
	// Use this for initialization
	void Start () {
		play = true;
		manual = false;

		Application.runInBackground = true;

		OSCHandler.Instance.Init();
		currentMinPlaybackTime = minPlaybackTime;
		currentMaxPlaybackTime = maxPlaybackTime / PlaybackTimeDivider;

		state = enAccelState.INIT;

		//RPSState = enRPSControlStates.CLIMBING;
		CurrentState.Instance.SetCurrentState (SystemStates.STANDBY);
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
		if (Input.GetKey(KeyCode.Alpha1)){
			Debug.Log ("STANDBY");
			manual = true;
		    CurrentState.Instance.SetCurrentState(SystemStates.STANDBY);
		}
		if (Input.GetKey(KeyCode.Alpha2)){
			Debug.Log ("CLIMBING");
			manual = true;
			CurrentState.Instance.SetCurrentState(SystemStates.CLIMBING);
		}
		if (Input.GetKey(KeyCode.Alpha3)){
			Debug.Log ("FALLING");
			manual = true;
			CurrentState.Instance.SetCurrentState(SystemStates.FALLING);
		}

		if(!manual)
			updateRPS ();


	}

	private void updateRPS(){

		switch (CurrentState.Instance.GetCurrentState()) {
		case SystemStates.STANDBY:
			Debug.Log ("STANDBY");
			GetComponent<Animator> ().speed = 0;
			//GetComponent<Animator> ().playbackTime = 0;
			Debug.Log (GetComponent<Animator> ().GetCurrentAnimatorStateInfo(0).length);


			if (OSCHandler.Instance.getRPS () > 1) {
				//switch to climbing if there is some movement
				CurrentState.Instance.SetCurrentState(SystemStates.CLIMBING);
			} 
			break;
		case SystemStates.CLIMBING:
			Debug.Log ("CLIMBING");
			if (playbackTime < maxPlaybackTime)
				RPS_Control();
			else
				//RPSState = enRPSControlStates.FALLING;
				CurrentState.Instance.SetCurrentState(SystemStates.FALLING);
			break;
		case SystemStates.FALLING:
			Debug.Log ("FALLING");
			if (playbackTime > minPlaybackTime) 
				MoveBackward();
			else 
				//RPSState = enRPSControlStates.CLIMBING;
				//if we reached the begining switch to STANDBY
				CurrentState.Instance.SetCurrentState(SystemStates.STANDBY);
			break;
		}
	}
	private void RPS_Control(){
		if (OSCHandler.Instance.getRPS () > 1) {
			//move FW
			MoveForward();
		} else {
			//move backwards
			//MoveBackward();

			/* change to falling */
			CurrentState.Instance.SetCurrentState(SystemStates.FALLING);
		}
	}
	private void MoveForward(){
		if (playbackTime < maxPlaybackTime)
			GetComponent<Animator> ().speed = 3;
		else
			GetComponent<Animator> ().speed = 0;
	}
	private void MoveBackward(){
		if (playbackTime > minPlaybackTime) 
			GetComponent<Animator> ().speed = -10;
		else
			GetComponent<Animator> ().speed = 0;
	}

	private void SetSpeedSimple(){
		if ((xAxisAccel >= 50) && (playbackTime > minPlaybackTime)) {
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
