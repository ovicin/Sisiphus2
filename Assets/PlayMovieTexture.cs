using UnityEngine;
using System.Collections;
using UnityOSC;

public class PlayMovieTexture : MonoBehaviour {

	MovieTexture movie;
	// Use this for initialization
	void Start () {

		movie = ((MovieTexture)GetComponent<Renderer>().material.mainTexture);
		movie.Play();
		//Debug.Log ("Movie Duration " + movie.duration);
		movie.loop = true;
	}
	
	// Update is called once per frame
	void Update () {


		/*
		Dictionary<string, ClientLog> clients = new Dictionary<string,ClientLog>();
		clients = OSCHandler.Instance.Clients;
		Dictionary<string, ServerLog> servers = new Dictionary<string, ServerLog>();
		servers = OSCHandler.Instance.Servers;
		*/
	}
}
