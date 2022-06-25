using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store_Upgrade_towers : MonoBehaviour
{

    [HideInInspector]
	public Sotre_blueprint sotre_blueprint;
    [HideInInspector]
	public bool isUpgraded = false;


   public void UpgradeTurret_DMG ()
	{
		if (PlayerStats.solidCash < sotre_blueprint.upgradeCost)
		{
			Debug.Log("Not enough cash to upgrade that!");
			return;
		}

		PlayerStats.solidCash -= sotre_blueprint.upgradeCost;


        isUpgraded = true;

		Debug.Log("Turret upgraded!");
    }

}
