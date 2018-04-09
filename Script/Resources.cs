using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resources : MonoBehaviour {
	//scenario
	public TextAsset scenario;
	private string[] scenarioArray;
	public Dictionary<string, StoryScene> scene = new Dictionary<string, StoryScene>();
	private string[] tempLine;
	[HideInInspector]
	public string firstScene;
	//illustration
	public List<Sprite> spriteIllustration;
	public Dictionary<string, Sprite> illustration = new Dictionary<string, Sprite>();
	//Audio
	public List<AudioClip> BGMClip;
	public Dictionary<string, AudioClip> BGM = new Dictionary<string, AudioClip>();
	public List<AudioClip> SEClip;
	public Dictionary<string, AudioClip> SE = new Dictionary<string, AudioClip>();

	void Awake(){
		DontDestroyOnLoad (this.gameObject);
		if (FindObjectsOfType(GetType()).Length > 1)
		{
			Destroy(gameObject);
		}
	}



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


		//illustration to dictionary
		foreach (Sprite sprite in spriteIllustration) {
			illustration.Add (sprite.name, sprite);
		}

		//audio to dictionary
		foreach (AudioClip clip in BGMClip) {
			BGM.Add (clip.name, clip);
		}
		foreach (AudioClip clip in SEClip) {
			SE.Add (clip.name, clip);
		}
	}
}
