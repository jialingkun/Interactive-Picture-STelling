using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
	// Use this for initialization
	void Start () {
	}

	void Update(){
		if (Input.GetKey(KeyCode.Escape)){
			Application.Quit();
		}
	}

	public void clickStart(){
		GameObject.Find ("loading").GetComponent<Text> ().text = "LOADING... PLEASE WAIT";
		GameObject.Find ("Load").SetActive (false);
		this.gameObject.SetActive (false);
		SceneManager.LoadScene (1);
	}
}
