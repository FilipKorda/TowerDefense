using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonyDestroyLevel : MonoBehaviour
{
    public static DonyDestroyLevel Instance; 
 
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
