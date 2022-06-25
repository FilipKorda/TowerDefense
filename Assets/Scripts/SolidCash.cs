using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SolidCash : MonoBehaviour
{
	public Text solidcashText;

	
	void Update () 
	{
		solidcashText.text = "$" + PlayerStats.solidCash.ToString();
	}
}