using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFileReader : MonoBehaviour
{
    public TextAsset txtFile;

    public List<string> theLines; 

    // Start is called before the first frame update
    void Start()
    {
        theLines = new List<string>(); 

        if (txtFile != null)
        {
            string[] splitText = txtFile.text.Split('\n');  

            foreach (string line in splitText)
            {
                theLines.Add(line); 
            }
        }
    } 
}
