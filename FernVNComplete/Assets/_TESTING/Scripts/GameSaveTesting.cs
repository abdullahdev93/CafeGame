using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VISUALNOVEL;

namespace TESTING
{
    public class GameSaveTesting : MonoBehaviour
    {
        public VNGameSave save;

        // Start is called before the first frame update
        void Awake()
        {
            VNGameSave.activeFile = new VNGameSave();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                VNGameSave.activeFile.Save();
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                try
                {
                    save = VNGameSave.Load($"{FilePaths.gameSaves}1{VNGameSave.FILE_TYPE}", activateOnLoad: true);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Do something because we found an error. {e.ToString()}");
                }
            }
        }
    }
}