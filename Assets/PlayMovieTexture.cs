using UnityEngine;
using System.Collections;
using UnityOSC;
using SystemDefs;

public class PlayMovieTexture : MonoBehaviour {
	public  SystemStates PlayInState;

	bool play;
	MovieTexture movie;
	// Use this for initialization
	void Start () {

		/* get one material from the material array */
		Material[] mat = GetComponent<Renderer> ().materials;
		/* Apply the material to the renderer */
//GetComponent<Renderer> ().material.mainTexture = Resources.Load("mountaintext_new_with_blink.mov") as Texture;
	//	MovieTexture movTexture;
	//	movTexture = Resources.Load ("new mountain export/mountaintexture/mountaintext_new_with_blink.mov") as MovieTexture;
	//	GetComponent<Renderer> ().material.mainTexture = movTexture;
		/* Play the movie texture */

		movie = ((MovieTexture)GetComponent<Renderer>().material.mainTexture);
		movie.Play();
		//Debug.Log ("Movie Duration " + movie.duration);
		movie.loop = true;


	}
	
	// Update is called once per frame
	void Update () {
		if (CurrentState.Instance.GetCurrentState () == PlayInState) {
			GetComponent<Renderer> ().enabled = true;
			play = true;
			movie.Play();
		} else {
			GetComponent<Renderer> ().enabled = false;
			play = false;
			movie.Stop();
		}
		/*
		if (Input.GetMouseButtonDown (0)) 
		if (play) {
			GetComponent<Renderer> ().enabled = false;
			play = false;
			movie.Stop();
		} else {
			GetComponent<Renderer> ().enabled = true;
			play = true;
			movie.Play();
		}
		*/
		/*
		Dictionary<string, ClientLog> clients = new Dictionary<string,ClientLog>();
		clients = OSCHandler.Instance.Clients;
		Dictionary<string, ServerLog> servers = new Dictionary<string, ServerLog>();
		servers = OSCHandler.Instance.Servers;
		*/
	}
}
