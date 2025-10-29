using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingScreenTips : MonoBehaviour
{
    public static LoadingScreenTips instance { get; private set; } 
    public TextAsset gameTips;
    public TMP_Text tipOnScreen;
    public GameObject loadingScreen;
    public TMP_Text continueText;

    public float loadTimer; 

    private bool loadScreenActive => DialogueSystem.instance.loadingShow;

    //public void Start()
    //{ 
        //instance = this; 
    //}

    public void Awake()
    {
        tipOnScreen.text = GetTextTip();

        loadTimer = (Random.Range(5, 10));

        instance = this; 
    }

    public string GetTextTip()
    {
        string[] tips = gameTips.text.Split('\n');

        string s = tips[Random.Range(0, tips.Length)];

        return s;
    }

    void Update()
    { 
        if (loadScreenActive)
        {
            loadTimer -= Time.deltaTime; 

            if (loadTimer <= 0)
            {
                continueText.text = "Continue"; 

                if (Input.GetKey(KeyCode.Mouse0))
                { 
                    loadingScreen.SetActive(false);
                }

                loadTimer = 0;
            }
        } 
    }
}
