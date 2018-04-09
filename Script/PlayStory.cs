using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayStory : MonoBehaviour {
	//resource
	private Resources resources;
	//scenario
	private Dictionary<string, StoryScene> scene = new Dictionary<string, StoryScene>();
	//illustration
	private Dictionary<string, Sprite> illustration = new Dictionary<string, Sprite>();
	//Audio
	private Dictionary<string, AudioClip> BGM = new Dictionary<string, AudioClip>();
	private Dictionary<string, AudioClip> SE = new Dictionary<string, AudioClip>();
	private AudioSource BGMaudioSource;
	private AudioSource SEaudioSource;
	private string currentBGM;
	//other
	private string firstScene;
	private int currentCommandIndex;
	private string currentScene;
	private string[] tempLine;
	private List<string> inventory;
	//animate
	private bool typingAnimation;
	public float textSpeed = 0.03f;
	public float transitionDelay = 1.1f;
	public float gameoverDelay = 1.5f;


	//other Gameobject;
	private Text dialogText;
	private Image illustrationImage;
	private Image fadePanel;
	//menu
	private GameObject menu;
	private GameObject confirmExit;
	//save load
	private GameObject saveLoad;
	private GameObject confirmSaveLoad;
	private bool isSave;
	private GameObject[] slot;
	private int maxSlots;
	private int currentSlot;
	//choice
	private string[] choiceDestination;
	private Text[] choiceText;
	private int currentChoicesCount;
	private int maxChoices;

	// Use this for initialization
	void Start () {
		PlayerPrefs.DeleteAll ();
		//resource
		resources = GameObject.Find("Resources").GetComponent<Resources>();
		//scenario resources
		firstScene = resources.firstScene;
		scene = resources.scene;
		//illustration resources
		illustration = resources.illustration;
		//audio resources
		BGM = resources.BGM;
		SE = resources.SE;
		//audio source
		BGMaudioSource = this.GetComponent<AudioSource>();
		SEaudioSource = this.gameObject.AddComponent<AudioSource> ();


		//temp initialize
		currentCommandIndex = -1; //-1 because currentCommandIndex++ run first
		currentScene = firstScene;
		currentBGM = "none";
		inventory = new List<string>();


		//gameobject initialize
		dialogText = GameObject.Find("DialogText").GetComponent<Text>();
		illustrationImage = GameObject.Find("Illustration").GetComponent<Image>();
		fadePanel = GameObject.Find("FadePanel").GetComponent<Image>();

		//menu
		confirmExit = GameObject.Find ("ConfirmExit");
		confirmExit.SetActive (false);
		menu = GameObject.Find ("Menu");
		menu.SetActive (false);

		//Save load slot
		maxSlots = 1;
		slot = new GameObject[maxSlots];
		string tempSpriteName;
		string tempText;
		for (int i = 0; i < maxSlots; i++) {
			slot [i] = GameObject.Find ("Slot" + (i + 1));
			tempSpriteName = PlayerPrefs.GetString ("Slot" + (i + 1) + "Image", "none");
			tempText = PlayerPrefs.GetString ("Slot" + (i + 1) + "Date", "Slot a" + (i + 1));
			if (tempSpriteName!="none") {
				slot [i].transform.Find ("Image").GetComponent<Image> ().sprite = illustration [tempSpriteName];
			}
			slot [i].transform.Find ("Text").GetComponent<Text> ().text = tempText;
			//load text / date
		}

		//save load UI
		confirmSaveLoad = GameObject.Find ("ConfirmSaveLoad");
		confirmSaveLoad.SetActive (false);
		saveLoad = GameObject.Find ("SaveLoadUI");
		saveLoad.SetActive (false);
		isSave = true;


		//choice initialize
		maxChoices = 6;
		choiceDestination = new string[maxChoices];
		choiceText = new Text[maxChoices];
		for (int i = 0; i < maxChoices; i++) {
			choiceText[i] = GameObject.Find("Choice"+(i+1)+"Text").GetComponent<Text>();
			choiceText [i].transform.parent.gameObject.SetActive (false);
		}


		//start dialog
		clickDialog();
	}

	IEnumerator AnimateText(string text){
		typingAnimation = true;
		for (int i = 0; i < (text.Length+1); i++)
		{
			dialogText.text = text.Substring(0, i);
			yield return new WaitForSeconds(textSpeed);
		}
		typingAnimation = false;
	}

	public void SkipAnimateText(){
		StopAllCoroutines();
		dialogText.text = tempLine[0];
		typingAnimation = false;
	}

	IEnumerator IllustrationTransition(Sprite sprite){
		dialogText.transform.parent.gameObject.SetActive (false);
		fadePanel.CrossFadeAlpha(1.0f, 0.7f, false); //(alpha value, fade speed, not important)
		yield return new WaitForSeconds(transitionDelay);
		illustrationImage.sprite = sprite;
		fadePanel.CrossFadeAlpha(0.0f, 0.7f, false);
		yield return new WaitForSeconds(1.0f);
		dialogText.transform.parent.gameObject.SetActive (true);
		clickDialog ();
	}

	IEnumerator gameoverTransition(){
		dialogText.transform.parent.gameObject.SetActive (false);
		yield return new WaitForSeconds(gameoverDelay);
		fadePanel.CrossFadeAlpha(1.0f, 2.0f, false); //(alpha value, fade speed, not important)
		yield return new WaitForSeconds(2.5f);
		SceneManager.LoadScene (0);
	}
	
	public void clickDialog(){
		if (typingAnimation) {
			SkipAnimateText ();
		} else {
			currentCommandIndex++;
			tempLine = scene [currentScene].commands [currentCommandIndex].Split(":"[0]);
			if (tempLine.Length > 1) {
				if (tempLine [0].Trim () == "choices") {
					currentChoicesCount = scene [currentScene].choices.Count;
					if (currentChoicesCount > 2) {
						for (int i = 0; i < currentChoicesCount; i++) {
							choiceDestination [i] = scene [currentScene].choices [i].destination;
							choiceText [i].text = scene [currentScene].choices [i].text;
							choiceText [i].transform.parent.gameObject.SetActive (true);
						}
					} else if (currentChoicesCount == 2) {
						//center top
						choiceDestination [1] = scene [currentScene].choices [0].destination;
						choiceText [1].text = scene [currentScene].choices [0].text;
						choiceText [1].transform.parent.gameObject.SetActive (true);
						//center bottom
						choiceDestination [4] = scene [currentScene].choices [1].destination;
						choiceText [4].text = scene [currentScene].choices [1].text;
						choiceText [4].transform.parent.gameObject.SetActive (true);
					}
					dialogText.transform.parent.gameObject.SetActive (false);
				} else if (tempLine [0].Trim () == "image") {
					StartCoroutine(IllustrationTransition(illustration [tempLine [1].Trim ()]));
				} else if (tempLine [0].Trim () == "bgm") {
					currentBGM = tempLine [1].Trim ();
					if (tempLine [1].Trim () == "none") {
						BGMaudioSource.Stop ();
					} else {
						BGMaudioSource.clip = BGM [currentBGM];
						BGMaudioSource.Play ();
					}
					clickDialog ();
				} else if (tempLine [0].Trim () == "se") {
					SEaudioSource.PlayOneShot (SE[tempLine [1].Trim ()], 0.6f);
					clickDialog ();
				} else if (tempLine [0].Trim () == "jump") {
					currentCommandIndex = -1;
					currentScene = tempLine [1].Trim ();
					clickDialog ();
				} else if (tempLine [0].Trim () == "get") {
					inventory.Add (tempLine [1].Trim ());
					clickDialog ();
				} else if (tempLine [0].Trim () == "req") {
					string[] tempReq = tempLine [1].Split ("," [0]);
					bool pass = true;
					bool reqExist;
					for (int i = 0; i < tempReq.Length; i++) {
						reqExist = false;
						for (int j = 0; j < inventory.Count; j++) {
							if (tempReq[i].Trim() == inventory[j]) {
								reqExist = true;
							}
						}
						if (!reqExist) {
							pass = false;
						}
					}
					if (pass) {
						currentCommandIndex = -1;
						currentScene = tempLine [2].Trim ();
						clickDialog ();
					} else {
						currentCommandIndex = -1;
						currentScene = tempLine [3].Trim ();
						clickDialog ();
					}
				} else { //not command but contain :
					tempLine[0] = scene [currentScene].commands [currentCommandIndex]; //unsplit
					StartCoroutine(AnimateText(tempLine[0]));
				}
			} else if(tempLine [0].Trim () == "gameover") { //gameover
				StartCoroutine(gameoverTransition());
			} else { //dialog
				StartCoroutine(AnimateText(tempLine[0]));
			}

		}

	}

	public void clickChoice(int destinationIndex){
		currentCommandIndex = -1;
		currentScene = choiceDestination [destinationIndex];
		for (int i = 0; i < maxChoices; i++) {
			choiceText [i].transform.parent.gameObject.SetActive (false);
		}
		dialogText.transform.parent.gameObject.SetActive (true);

		clickDialog ();
	}


	public void clickOpenMenu(){
		menu.SetActive (true);
	}

	public void clickCloseMenu(){
		menu.SetActive (false);
	}

	public void clickExitMenu(){
		confirmExit.SetActive (true);
	}

	public void clickExitYes(){
		SceneManager.LoadScene (0);
	}

	public void clickExitNo(){
		confirmExit.SetActive(false);
	}

	public void clickSaveMenu(){
		isSave = true;
		saveLoad.SetActive(true);
	}

	public void clickLoadMenu(){
		isSave = false;
		saveLoad.SetActive(true);
	}

	public void clickCloseSaveLoad(){
		saveLoad.SetActive (false);
	}



	public void clickSlot(int slotNumber){
		if (isSave) { //save
			if (PlayerPrefs.GetString ("Slot" + slotNumber + "Date", "none") == "none") {
				save (slotNumber);
			} else {
				currentSlot = slotNumber;
				confirmSaveLoad.SetActive (true);
				confirmSaveLoad.transform.Find("Text").GetComponent<Text>().text = "Overwrite this save data?";
			}
		} else { //load
		}
	}

	private void save(int slotNumber){
		string tempDate = System.DateTime.Now.ToString ("yyyy/MM/dd hh:mm:ss");
		string tempSpriteName = illustrationImage.sprite.name;
		PlayerPrefs.SetString ("Slot" + slotNumber + "Date", tempDate);
		PlayerPrefs.SetString ("Slot" + slotNumber + "Image", tempSpriteName);

		slot [slotNumber-1].transform.Find ("Image").GetComponent<Image> ().sprite = illustration [tempSpriteName];
		slot [slotNumber-1].transform.Find ("Text").GetComponent<Text> ().text = tempDate;

		PlayerPrefs.SetString ("Slot" + slotNumber + "Scene", currentScene);
		PlayerPrefs.SetInt ("Slot" + slotNumber + "CommandIndex", currentCommandIndex-1);
		PlayerPrefs.SetString ("Slot" + slotNumber + "BGM", currentBGM);
		PlayerPrefs.SetString ("Slot" + slotNumber + "Inventory", string.Join (",", inventory.ToArray()));
	}

	private void load(int slotNumber){
	}

	public void clickConfirmSaveLoadYes(){
		if (isSave) { //save
			save (currentSlot);
			confirmSaveLoad.SetActive (false);
		} else { //load
		}

	}

	public void clickConfirmSaveLoadNo(){
		confirmSaveLoad.SetActive (false);
	}
}
