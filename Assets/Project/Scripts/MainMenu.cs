using UnityEngine;
using System.Collections;

public class Main_menu : MonoBehaviour 
{

	public string startLevel;

	public string levelSelect;

	public void NewGame()
	{
		Application.LoadLevel (startLevel);
	}

	public void LevelSelect()
	{
		Application.LoadLevel (levelSelect);
	}

	public void QuitGame()
	{
		Debug.Log ("Game Exited");
		Application.Quit();
	}

}
