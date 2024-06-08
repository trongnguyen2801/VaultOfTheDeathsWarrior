using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public enum  dataName
    {
        PlayerSex,
        StartScreen,
    }

    private Dictionary<dataName, string> _dataType = new Dictionary<dataName, string>()
    {
        {dataName.PlayerSex,"PlayerSex"},
        {dataName.StartScreen,"StartScreen"},
    };

    public static DataManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public void SaveData(dataName _name, string data)
    {
        PlayerPrefs.SetString(_dataType[_name],data);
        PlayerPrefs.Save();
    }
    
    public void SaveData(dataName _name, int data)
    {
        PlayerPrefs.SetInt(_dataType[_name],data);
        PlayerPrefs.Save();
    }
    
    public void SaveData(dataName _name, float data)
    {
        PlayerPrefs.SetFloat(_dataType[_name],data);
        PlayerPrefs.Save();
    }
    
    public int LoadDataInt(dataName _name)
    {
        int val = 0;
        if (PlayerPrefs.HasKey(_dataType[_name]))
        {
            val = PlayerPrefs.GetInt(_dataType[_name]);
        }
        return val;
    }
}
