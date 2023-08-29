using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public enum ChessTurn { None, White, Black }

public class Main : MonoBehaviour
{
    Socket socket;

    static ChessTurn ct = ChessTurn.Black;  //记录当前是黑棋还是白旗
    ChessPanel cp;  //棋盘
    Transform bg;  //背景
    Button btnStart;  //开始按钮
    Transform ready;  //“准备”图片
    Transform blackBox;
    Transform infoPanel;  //信息面板
    Transform myPanel;  //我的面板
    Text mid;
    Text mScore;
    Transform oppPanel;  //对手的面板
    Text oid;
    Text oScore;
    Transform loginPanel;  //登录面板
    Button btnLogin;  //登录按钮
    Button btnExit;
    InputField inputText;
    Transform resultPanel;  //结果面板
    Text resultText;
    Button btnClose;
    Button btnQuit;
    static Player me;
    static Player opponent;
    public static bool isStart;  //游戏是否开始

    public static T FindType<T>(Transform t, string s)
    {
        return t.Find(s).GetComponent<T>();
    }

    void Start()
    {
        if (!Message.CheckMessage(Application.dataPath + "/Swm.dat"))
        {
            return;
        }

        Init();
        NetManager.Connect("127.0.0.1", 8888);
        NetManager.AddListener("Enter", OnEnter);
        NetManager.AddListener("Play", OnPlay);
        NetManager.AddListener("GameStart", OnGameStart);
        NetManager.AddListener("Leave", OnLeave);
        NetManager.AddListener("Result", OnResult);


        //socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //btn.onClick.AddListener(delegate
        //{
        //    socket.Connect("127.0.0.1", 8888);
        //});
    }

    private void OnResult(string str)
    {
        string[] s = str.Split('_');
        ChessTurn ct = (ChessTurn)int.Parse(s[0]);
        me.win = int.Parse(s[1]);
        me.lost = int.Parse(s[2]);
        opponent.win = int.Parse(s[3]);
        opponent.lost = int.Parse(s[4]);
        if (me.type == ct)
        {
            resultText.text = "You Win!";
            resultText.color = Color.yellow;
        }
        else
        {
            resultText.text = "You Lost!";
            resultText.color = Color.blue;
        }
        me.SetId();
        opponent.SetId();
        resultPanel.gameObject.SetActive(true);
    }

    private void OnLeave(string str)
    {
        me.win++;
        me.SetId();
        opponent.lost++;
        opponent.SetId();
        resultText.text = "You Win!";
        resultText.color=Color.yellow;
        resultPanel.gameObject.SetActive(true);
    }

    private void OnGameStart(string str)
    {
        ready.gameObject.SetActive(false);
        isStart = true;
        ct = ChessTurn.Black;
        cp.InitPiece();
        btnStart.interactable = false;
        string[] s = str.Split('_');
        me.type = (ChessTurn)int.Parse(s[0]);
        opponent.type = 3 - me.type;
        opponent.pname = s[1];
        opponent.SetId();
        opponent.id.text = "对手" + s[1];
        blackBox.gameObject.SetActive(me.type == ChessTurn.Black);
    }

    private void OnPlay(string str)
    {
        string[] s = str.Split('_');
        cp.SetPiece(s[0] + "_" + s[1], s[2]);
        ct = (ChessTurn)(3 - int.Parse(s[2]));
    }

    private void OnEnter(string str)
    {
        loginPanel.gameObject.SetActive(false);
        me.SetId();
    }

    private void Init()
    {
        resultPanel = transform.Find("ResultPanel");
        resultText = FindType<Text>(resultPanel, "ResultText");
        btnClose = FindType<Button>(resultPanel, "BtnClose");
        btnClose.onClick.AddListener(delegate
        {
            ReSet();
            resultPanel.gameObject.SetActive(false);
        });
        ///////////////////////////////////////////////////////////////////////////////////////////
        cp = FindType<ChessPanel>(transform, "Panel");
        loginPanel = transform.Find("LoginPanel");
        btnLogin = FindType<Button>(loginPanel, "BtnLogin");
        btnLogin.onClick.AddListener(delegate
        {
            if (inputText.text == "")
            {
                return;
            }
            me.pname = inputText.text;
            NetManager.Send("Enter|" + me.pname);
        });
        ///////////////////////////////////////////////////////////////////////////////////////////
        btnExit = FindType<Button>(loginPanel, "BtnExit");
        btnExit.onClick.AddListener(delegate
        {
            Application.Quit();
        });
        ///////////////////////////////////////////////////////////////////////////////////////////
        inputText = FindType<InputField>(loginPanel, "InputField");
        infoPanel = transform.Find("InfoPanel");
        myPanel = infoPanel.Find("M");
        mid = FindType<Text>(myPanel, "Id");
        mScore = FindType<Text>(myPanel, "Text");
        me = new Player(mid, mScore);
        oppPanel = infoPanel.Find("O");
        oid = FindType<Text>(oppPanel, "Id");
        oScore = FindType<Text>(oppPanel, "Text");
        opponent = new Player(oid, oScore);

        bg = transform.Find("BG");
        btnStart = FindType<Button>(bg, "BtnStartGame");
        btnQuit = FindType<Button>(bg, "BtnQuit");
        ready = bg.Find("Ready");
        blackBox = bg.Find("BoxBlack");
        ///////////////////////////////////////////////////////////////////////////////////////////
        btnStart.onClick.AddListener(delegate
        {
            ready.gameObject.SetActive(true);
            NetManager.Send("GameStart|");
        });
        btnQuit.onClick.AddListener(delegate
        {
            Application.Quit();
        });
    }

    void Update()
    {
        NetManager.Update();
    }

    public static bool MayPlay()
    {
        return ct == me.type;
    }

    private void ReSet()
    {
        isStart = false;
        me.SetId();
        btnStart.interactable = true;
        ct = ChessTurn.Black;
        cp.InitPiece();
    }
}
