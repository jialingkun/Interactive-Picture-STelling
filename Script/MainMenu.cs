using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
	//resource
	private Resources resources;
	//save load
	private GameObject saveLoad;
	private GameObject confirmSaveLoad;
	private GameObject[] slot;
	private int maxSlots;
	private int currentSlot;

	// Use this for initialization
	void Start () {
		//resource
		resources = GameObject.Find("Resources").GetComponent<Resources>();
		//Save load slot
		maxSlots = 6;
		slot = new GameObject[maxSlots];
		string tempSpriteName;
		string tempText;
		for (int i = 0; i < maxSlots; i++) {
			slot [i] = GameObject.Find ("Slot" + (i + 1));
			tempSpriteName = PlayerPrefs.GetString ("Slot" + (i + 1) + "Image", "none");
			tempText = PlayerPrefs.GetString ("Slot" + (i + 1) + "Date", "Slot " + (i + 1));
			if (tempSpriteName == "none") {
				slot [i].transform.Find ("Image").GetComponent<Image> ().sprite = resources.emptySlot;
			} else {
				slot [i].transform.Find ("Image").GetComponent<Image> ().sprite = resources.illustration [tempSpriteName];
			}
			slot [i].transform.Find ("Text").GetComponent<Text> ().text = tempText;
		}

		//save load UI
		confirmSaveLoad = GameObject.Find ("ConfirmSaveLoad");
		confirmSaveLoad.SetActive (false);
		saveLoad = GameObject.Find ("SaveLoadUI");
		saveLoad.SetActive (false);
	}

	void Update(){
		if (Input.GetKey(KeyCode.Escape)){
			Application.Quit();
		}
	}

	public void clickStart(){
		//PlayerPrefs.DeleteAll ();
		GameObject.Find ("loading").GetComponent<Text> ().text = "LOADING... PLEASE WAIT";
		GameObject.Find ("Load").SetActive (false);
		this.gameObject.SetActive (false);

		//initialize load
		PlayerPrefs.SetString("LoadScene", resources.firstScene); 
		PlayerPrefs.SetInt("LoadCommandIndex", -1); //-1 because currentCommandIndex++ run first
		PlayerPrefs.SetString("LoadInventory","");
		PlayerPrefs.SetString("LoadBGM", "none");
		PlayerPrefs.SetString("LoadImage", "none");


		SceneManager.LoadScene (1);
	}

	public void clickLoad(){
		saveLoad.SetActive(true);
	}

	public void clickCloseSaveLoad(){
		saveLoad.SetActive (false);
	}



	public void clickSlot(int slotNumber){
		if (PlayerPrefs.GetString ("Slot" + slotNumber + "Date", "none") != "none"){
			currentSlot = slotNumber;
			confirmSaveLoad.SetActive (true);
			confirmSaveLoad.transform.Find("Text").GetComponent<Text>().text = "Load this save data?";
		}
	}

	private void load(int slotNumber){
		PlayerPrefs.SetString("LoadScene", PlayerPrefs.GetString ("Slot" + slotNumber + "Scene")); 
		PlayerPrefs.SetInt("LoadCommandIndex", PlayerPrefs.GetInt ("Slot" + slotNumber + "CommandIndex"));
		PlayerPrefs.SetString("LoadInventory",PlayerPrefs.GetString ("Slot" + slotNumber + "Inventory"));
		PlayerPrefs.SetString("LoadBGM",PlayerPrefs.GetString ("Slot" + slotNumber + "BGM"));
		PlayerPrefs.SetString("LoadImage",PlayerPrefs.GetString ("Slot" + slotNumber + "Image"));

		SceneManager.LoadScene (1);
	}

	public void clickConfirmSaveLoadYes(){
		load(currentSlot);
	}

	public void clickConfirmSaveLoadNo(){
		confirmSaveLoad.SetActive (false);
	}
}
