using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryScene{
	public List<string> commands = new List<string>();
	public List<Choice> choices = new List<Choice>();
}
