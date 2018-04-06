using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayStory : MonoBehaviour {
	//scenario
	public TextAsset scenario;
	private string[] scenarioArray;
	private Dictionary<string, StoryScene> scene = new Dictionary<string, StoryScene>();
	//illustration
	public List<Sprite> spriteIllustration;
	private Dictionary<string, Sprite> illustration = new Dictionary<string, Sprite>();
	//other
	private string firstScene;
	private int currentCommandIndex;
	private string currentScene;
	private string[] tempLine;
	//animate
	private bool typingAnimation;
	public float textSpeed = 0.03f;
	public float transitionDelay = 1.1f;


	//other Gameobject;
	private Text dialogText;
	private Image illustrationImage;
	private Image fadePanel;
	//choice
	private string[] choiceDestination;
	private Text[] choiceText;
	private int currentChoicesCount;

	// Use this for initialization
	void Start () {

		//scenario
		scenarioArray = scenario.text.Split("\n"[0]);
		firstScene = scenarioArray [0].Split (":" [0]) [1].Trim();

		//scenario to Dictionary
		string tempSceneKey = firstScene;
		bool choiceOpen = false;
		foreach (string line in scenarioArray) {
			tempLine = line.Split(":"[0]);
			if (tempLine.Length > 1) { //Command
				if (tempLine [0].Trim () == "scene") {
					tempSceneKey = tempLine [1].Trim ();
					scene.Add (tempSceneKey, new StoryScene ());
					choiceOpen = false;
				} else if (tempLine [0].Trim () == "choices") {
					choiceOpen = true;
					scene[tempSceneKey].commands.Add(line);
				} else if (choiceOpen) { //choices
					Choice tempChoice = new Choice();
					tempChoice.text = tempLine [0].Trim ();
					tempChoice.destination = tempLine [1].Trim ();
					scene [tempSceneKey].choices.Add (tempChoice);
				} else { //other command
					scene[tempSceneKey].commands.Add(line);
				}
			} else if (line.Trim() != ""){ //text dialog
				scene[tempSceneKey].commands.Add(line);
			}
		}


		/* Debugging
		foreach (string x in scene["AwalMula"].commands) {
			print (x);
		}

		foreach (Choice x in scene["AwalMula"].choices) {
			print (x.text+"|"+x.destination);
		}*/


		//illustration
		foreach (Sprite sprite in spriteIllustration) {
			illustration.Add (sprite.name, sprite);
		}



		//temp initialize
		currentCommandIndex = -1; //-1 because currentCommandIndex++ run first
		currentScene = firstScene;


		//gameobject initialize
		dialogText = GameObject.Find("DialogText").GetComponent<Text>();
		illustrationImage = GameObject.Find("Illustration").GetComponent<Image>();
		fadePanel = GameObject.Find("FadePanel").GetComponent<Image>();

		//choice initialize
		int maxChoices = 6;
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
		fadePanel.CrossFadeAlpha(1.0f, 0.7f, false); //(alpha value, fade speed, not important)
		yield return new WaitForSeconds(transitionDelay);
		illustrationImage.sprite = sprite;
		fadePanel.CrossFadeAlpha(0.0f, 0.7f, false);
		yield return new WaitForSeconds(1.0f);
		clickDialog ();
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
						choiceDestination [1] = scene [currentScene].choices [1].destination;
						choiceText [1].text = scene [currentScene].choices [1].text;
						choiceText [1].transform.parent.gameObject.SetActive (true);
						//center bottom
						choiceDestination [4] = scene [currentScene].choices [4].destination;
						choiceText [4].text = scene [currentScene].choices [4].text;
						choiceText [4].transform.parent.gameObject.SetActive (true);
					}
					dialogText.transform.parent.gameObject.SetActive (false);
				} else if (tempLine [0].Trim () == "image") {
					StartCoroutine(IllustrationTransition(illustration [tempLine [1].Trim ()]));
				} else { //not command but contain :
					tempLine[0] = scene [currentScene].commands [currentCommandIndex]; //unsplit
					StartCoroutine(AnimateText(tempLine[0]));
				}
			} else if(tempLine [0].Trim () == "gameover") { //gameover
				dialogText.text = "GAMEOVER";
			} else { //dialog
				StartCoroutine(AnimateText(tempLine[0]));
			}

		}

	}

	public void clickChoice(int destinationIndex){
	}
}
