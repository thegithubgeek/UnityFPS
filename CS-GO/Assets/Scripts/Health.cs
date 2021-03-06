﻿using UnityEngine;
using UnityEngine.Networking;
public class Health : Photon.MonoBehaviour 
{
	public const int maxHealth = 100;
	public int currentHealth = maxHealth;
	public Texture t;
	public Texture cross;
	public GameObject[] spawn;
	public string team;
	private Log log;
	void Start(){
		log = FindObjectOfType<Log> ();
	}

	[PunRPC]
	public void TakeDamage(int amount, string name)
	{
		currentHealth -= amount;
		if (currentHealth <= 0)
		{
			currentHealth = maxHealth;
			photonView.RPC("RpcRespawn", PhotonTargets.AllBuffered, null);
			photonView.RPC ("add", PhotonTargets.AllBuffered, photonView.name + " was killed by " + name);
			photonView.RPC ("removeFlag", PhotonTargets.AllBuffered);
		}
	}
	public void OnGUI(){
		if(!photonView.isMine){
			return;
		}
		GUI.Box (new Rect(Screen.width/2 - 5, Screen.height/2 - 5, 10, 10), cross);
		GUI.Box (new Rect(0, 0, 200, 20), t);
		GUI.Box (new Rect(0, 0, currentHealth/maxHealth * 200, 20), t);
		GUI.Box (new Rect(0, 20, 200, 20), "Health: " + currentHealth);
		GUI.Box (new Rect(0, 40, 200, 20), "Team: " + team);
	}
	[PunRPC]
	public void RpcRespawn()
	{
		if(photonView.isMine){
			transform.position = spawn[Random.Range(0, spawn.Length)].transform.position;
		}
	}
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			stream.SendNext (currentHealth);
		} else if(stream.isReading && photonView.isMine){
			currentHealth = (int)stream.ReceiveNext ();
		}
	}
	[PunRPC]
	public void RedColor()
	{
		transform.FindChild ("MazeLowMan").FindChild("LowMan").GetComponent<Renderer> ().material.color = Color.red;
	}
	[PunRPC]
	public void BlueColor()
	{
		transform.FindChild ("MazeLowMan").FindChild("LowMan").GetComponent<Renderer> ().material.color = Color.blue;
	}
	[PunRPC]
	void add(string s) {
		log.log = log.log + "\n" + s;
	}
}