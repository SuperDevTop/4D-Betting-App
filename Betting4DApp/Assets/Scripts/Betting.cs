using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class Betting : MonoBehaviour
{
    [SerializeField] private InputField[] bettingNumberInput;
    [SerializeField] private InputField[] bettingBigInput;
    [SerializeField] private InputField[] bettingSmallInput;
    [SerializeField] private Toggle[] bettingCompanyM;
    [SerializeField] private Toggle[] bettingCompanyD;
    [SerializeField] private Toggle[] bettingCompanyT;
    [SerializeField] private Toggle[] bettingCompanyG;
    [SerializeField] private InputField[] bettingRoll;
    [SerializeField] private Text[] bettingTotal;

    public string absURL = "";
    string betURL;

    public class Result
    {
        public string result;
        public int rank;
        public int profit;       
    }

    private void Start()
    {
        betURL = absURL + "api/bet";
    }

    public void CreateBet()
    {
        StartCoroutine(CreateBetAction());
    }

    IEnumerator CreateBetAction()
    {
        for (int i = 0; i < MainUI.bettingCount; i++)
        {
            if (bettingNumberInput[i].text.Length == 4 && int.Parse(bettingTotal[i].text) > 0)
            {
                StartCoroutine(PostBet(betURL, bettingNumberInput[i].text, int.Parse(bettingBigInput[i].text), int.Parse(bettingSmallInput[i].text), GetCompanyCode(i), MainUI.userID));
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    public string GetCompanyCode(int index)
    {
        string code = "";

        if (bettingCompanyM[index].isOn)
        {
            code += "M";
        }            

        if (bettingCompanyD[index].isOn)
        {
            code += "D";
        }     

        if (bettingCompanyT[index].isOn)
        {
            code += "T";
        }      

        if (bettingCompanyG[index].isOn)
        {
            code += "G";
        }

        return code;
    }

    public void TextValueChanged()
    {
        for (int i = 0; i < MainUI.bettingCount; i++)
        {
            bettingTotal[i].text = "" + (int.Parse(bettingSmallInput[i].text) + int.Parse(bettingBigInput[i].text)) * GetCheckedNumber(i);
        }
    }

    public int GetCheckedNumber(int index)
    {
        int number = 0;

        if (bettingCompanyD[index].isOn)
        {
            number++;
        }

        if (bettingCompanyT[index].isOn)
        {
            number++;
        }

        if (bettingCompanyG[index].isOn)
        {
            number++;
        }

        if (bettingCompanyM[index].isOn)
        {
            number++;
        }

        return number;
    }

    public bool IsPossibile()
    {
        bool isPossible = false;
        
        for(int i = 0; i < MainUI.bettingCount; i++)
        {

        }

        return isPossible;
    }

    IEnumerator PostBet(string url, string number, int big, int small, string company, int id)
    {          
        WWWForm form = new WWWForm();
        form.AddField("number", number);
        form.AddField("big", big);
        form.AddField("small", small);
        form.AddField("company", company);
        form.AddField("id", id);

        UnityWebRequest uwr = UnityWebRequest.Post(url, form);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);

            // Recieve data from backend.
            Result loadData = JsonUtility.FromJson<Result>(uwr.downloadHandler.text);

            print("!!!Success");
        }
    }
}
