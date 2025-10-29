using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareController : MonoBehaviour
{
    private SpriteRenderer squareSprite;

    public Sprite theDefaultImage;
    public Sprite pressedImage;

    //public GameObject OKEffect;
    //public GameObject goodEffect;
    //public GameObject perfectEffect;
    //public GameObject missEffect; 

    public KeyCode keyToPress;

    // Start is called before the first frame update
    void Start()
    {
        squareSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(keyToPress))
        {
            squareSprite.sprite = pressedImage;
        }

        if (Input.GetKeyUp(keyToPress))
        {
            squareSprite.sprite = theDefaultImage;
        }
    }
} 