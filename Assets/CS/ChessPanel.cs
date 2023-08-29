using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessPanel : MonoBehaviour
{
    public GameObject prefab;
    const int line = 15;

    //设置棋盘代码，点击棋盘的位置会显示棋子
    public void SetPiece(string name,string idxStr)
    {
        Transform t= transform.Find(name);
        foreach (Transform item in t)
        {
            item.gameObject.SetActive(false);
        }
        t.Find(idxStr).gameObject.SetActive(true);
        t.GetComponent<Button>().interactable = false;
    }

    //初始化
    public void InitPiece()
    {
        foreach (Transform t in transform) 
        {
            Destroy(t.gameObject);
        }
        for(int i = 0; i < line; i++)
        {
            for (int j = 0; j < line; j++)
            {
                GameObject p = Instantiate(prefab, transform);
                p.name = i + "_" + j;
                p.GetComponent<Button>().onClick.AddListener(delegate
                {
                    if (!Main.isStart)
                    {
                        return;
                    }
                    if (!Main.MayPlay())
                    {
                        return;
                    }
                    NetManager.Send("Play|" + p.name);
                });
            }
        }
    }

    void Update()
    {
        
    }
}
