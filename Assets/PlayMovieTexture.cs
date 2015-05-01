using UnityEngine;
using System.Collections;
using UnityOSC;

public class PlayMovieTexture : MonoBehaviour {

	// Use this for initialization
	void Start () {
		((MovieTexture)GetComponent<Renderer>().material.mainTexture).Play();

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
