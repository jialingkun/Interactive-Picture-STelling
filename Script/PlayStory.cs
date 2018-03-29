using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	// Use this for initialization
	void Start () {

		//scenario
		scenarioArray = scenario.text.Split("\n"[0]);
		firstScene = scenarioArray [0].Split (":" [0]) [1].Trim();

		//scenario to Dictionary
		string[] tempLine;
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
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
