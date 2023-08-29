using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Message
{
    static string GetMessage()
    {
        string strId = SystemInfo.deviceUniqueIdentifier;
        return strId;
    }

    public static bool CheckMessage(string name)
    {
#if UNITY_EDITOR
        FileInfo fileInfo = new FileInfo(name);
        if (fileInfo.Exists)
        {
            ArrayList arrayList = SRMessage(name);
            string message = GetMessage();
            if (message == arrayList[0].ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            StreamWriter sw;
            sw = fileInfo.CreateText();
            sw.WriteLine(GetMessage());
            sw.Close();
            return true;
        }
#endif
        return true;
    }

    static ArrayList SRMessage(string name)
    {
        StreamReader sr = null;
        try
        {
            sr = File.OpenText(name);
        }
        catch(Exception e)
        {
            return null;
        }
        string line;
        ArrayList arrayList = new ArrayList();
        while((line = sr.ReadLine()) != null)
        {
            arrayList.Add(line);
        }
        sr.Close();
        sr.Dispose();
        return arrayList;
    }
}
