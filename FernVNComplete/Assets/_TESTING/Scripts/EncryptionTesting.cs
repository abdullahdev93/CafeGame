using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

public class EncryptionTesting : MonoBehaviour
{
    public DATA data;
    public string encryptedData = "";
    private const string key = "SECRETKEY";

    // Start is called before the first frame update
    void Start()
    {
        //data = new DATA();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Save();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            Load();
        }
    }

    private void Save()
    {
        string inputString = JsonUtility.ToJson(data);

        Debug.Log($"Original String: {inputString}");
        Debug.Log($"Key: {key}");

        byte[] inputBytes = Encoding.UTF8.GetBytes(inputString);
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);

        Debug.Log($"Input Bytes: {BitConverter.ToString(inputBytes)}");
        Debug.Log($"Key Bytes: {BitConverter.ToString(keyBytes)}");

        byte[] encryptedBytes = XOREncode(inputBytes, keyBytes);
        System.IO.File.WriteAllBytes("Assets/_Testing/encryptedData.txt", encryptedBytes);
    }

    private byte[] XOREncode(byte[] input, byte[] key)
    {
        byte[] output = new byte[input.Length];
        for (int i = 0; i < input.Length; i++)
        {
            output[i] = (byte)(input[i] ^ key[i % key.Length]);
        }
        return output;
    }

    private void Load()
    {
        byte[] encryptedBytes = System.IO.File.ReadAllBytes("Assets/_Testing/encryptedData.txt");
        byte[] keyBytes = Encoding.UTF8.GetBytes(key);

        Debug.Log($"Encrypted Bytes: {BitConverter.ToString(encryptedBytes)}");
        Debug.Log($"Encrypted String: {Encoding.UTF8.GetString(encryptedBytes)}");

        byte[] decryptedBytes = XOREncode(encryptedBytes, keyBytes);
        string decryptedString = Encoding.UTF8.GetString(decryptedBytes);

        Debug.Log($"Decrypted String: {decryptedString}");
    }

    [System.Serializable]
    public class DATA
    {
        public string model;
        public float cost;
        public int version;
        public bool reliable;
        public List<int> config;
    }
}
