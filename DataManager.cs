using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    public int gameMode;
    public int difficult;
    public string nickName;

    public string[] gameModeTexts = { "덧셈", "뺄셈", "곱셈", "나눗셈" };
    public string[] GTexts = { "+", "-", "X", "%" };
    public string[] DTexts = { "입문", "초급", "중급", "고급"};
    public int[,] numRangeSet
        = new int[16,2] { { 1, 10 }, { 6, 50 }, { 11, 100 }, { 51, 1000 }, 
                          { 1, 10 }, { 6, 50 }, { -50, 50 }, { -500, 500 },
                          { 1, 10 }, { 1, 20 }, { 1, 100 }, { 1, 1000 },
                          { 1, 10 }, { 6, 50 }, { 11, 100 }, { 51, 1000 } };
    public int[,] starLimitSet
        = new int[4, 4] { { 8, 12, 16, 22 }, { 6, 10, 14, 20 }, { 4, 8, 12, 18 }, { 3, 6, 9, 15 } };
    public int[,] errorRangeSet
        = new int[4, 2] { { -3, 5 }, { -5, 8 }, { -8, 12 }, { -50, 50 } };

    public int count;
    public int correct;
    public int maxScore;

    public string[][] playerDatas;
    public bool isOkay = false;

    private void Awake()
    {
        if (!instance) 
            instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public string SetDTitle()
    {
        //"<난이도 : 입문>\r\n각 항의 수 범위 : 1 ~ 9\r\n클리어 조건 : 8/12/16/22\r\n최고기록 : " + (PlayerPrefs.HasKey("P0C") ? PlayerPrefs.GetInt("P0C") : "없음");
        return "<난이도 : " + DTexts[difficult] + ">\r\n각 항의 수 범위 : " + numRangeSet[4 * gameMode + difficult, 0] + " ~ " + (numRangeSet[4 * gameMode + difficult, 1] - 1)
            + "\r\n클리어 조건 : " + starLimitSet[difficult, 0] + "/" + starLimitSet[difficult, 1] + "/" + starLimitSet[difficult, 2] + "/" + starLimitSet[difficult, 3]
            + "\r\n최고기록 : " + GetPlayData();
    }

    private string GetPlayData()
    {
        string playerPrefebsName = gameMode + "BS" + difficult; //BestScore
        if (PlayerPrefs.HasKey(playerPrefebsName))
            return PlayerPrefs.GetInt(playerPrefebsName).ToString();
        else return "없음";
    }

    public string SetGTitle()
    {
        return "<게임 방법>\r\n30초 내에 최대한 많은 " + gameModeTexts[gameMode] + " 문제를 맞추세요!";
    }

    public void GetData()
    {
        string fileName = gameMode + "R" + difficult;
        string filePath = Path.Combine(Application.persistentDataPath, fileName + ".txt");
        if (!File.Exists(filePath)) { //파일 없으면 생성하고 결과 추가
            //디렉토리 생성
            string directoryName = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryName)) {
                Directory.CreateDirectory(directoryName);
            }
        }
        makeData(filePath); //이번판 결과도 파일에 저장

        //.txt파일을 .csv처럼 나누고 그것을 배열로 저장
        List<string[]> playerData = new List<string[]>();
        using (StreamReader reader = new StreamReader(filePath)) {
            string content = reader.ReadToEnd();
            string[] playerDatas = content.Split(new char[] { '\n' });
            for (int i = 0; i < playerDatas.Length; i++) {
                playerData.Add(playerDatas[i].Split(new char[] { ',' }));
            }
        }
        playerDatas = playerData.ToArray();
        setRanking();
        //문제 : playerDatas 가 {{닉넴, 맞춘개수, 전체개수}. {닉넴, 맞춘개수, 전체개수}} 형태인데
        //맞춘개수를 기준으로 playerDatas를 정렬하고 싶은데 어떻게 할 수 있을까요?
    }

    public void makeData(string filePath)
    {
        if (isOkay) {
            using (StreamWriter writer = new StreamWriter(filePath))
                writer.WriteLine(nickName + "," + correct + "," + count);
        }
    }

    public void setRanking()
    {

    }
}
