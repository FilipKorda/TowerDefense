using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EXPManager : MonoBehaviour
{
    public Text currentexptext, targetEXPtext, levelText;
    public int currentexp, targetEXP, level;

    public static EXPManager instance;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else        
            Destroy(gameObject);       
    }

    void Start() 
    {
        currentexptext.text = currentexp.ToString();
        targetEXPtext.text = targetEXP.ToString();
        levelText.text = level.ToString();
    }

    public void AddEXP(int exp)
    {
        currentexp += exp;

        if(currentexp >=targetEXP)
        {
            currentexp = currentexp - targetEXP;
            level++;
            levelText.text = level.ToString();
        }

        currentexptext.text = currentexp.ToString();
    }
}
