using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Linq;
using System;

public class Betting : MonoBehaviour
{
    [SerializeField] InputField[] bettingNumberInput;
    [SerializeField] InputField[] bettingBigInput;
    [SerializeField] InputField[] bettingSmallInput;
    [SerializeField] private Toggle[] bettingCompanyM;
    [SerializeField] private Toggle[] bettingCompanyD;
    [SerializeField] private Toggle[] bettingCompanyT;
    [SerializeField] private Toggle[] bettingCompanyG;
    [SerializeField] private Dropdown[] bettingRoll;
    [SerializeField] private Text[] bettingTotal;

    public string absURL = "";
    string betURL;

    public class Result
    {
        public int result;
        public int rank;
        public int profit;       
    }

    private void Start()
    {
        betURL = absURL + "api/bet";
    }

    public void CreateBet()
    {
        MainUI.instance.loadingUI.SetActive(true);
        StartCoroutine(CreateBetAction());
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
            if(bettingNumberInput[i].text.Length == 4 && (bettingBigInput[i].text != "" || bettingSmallInput[i].text != ""))
            {
                string big = bettingBigInput[i].text;
                string small = bettingSmallInput[i].text;

                if(big == "") big = "0";
                if(small == "") small = "0";

                if (bettingRoll[i].value == 0)
                {
                    bettingTotal[i].text = "" + (int.Parse(small) + int.Parse(big)) * GetCheckedNumber(i);
                }
                else if (bettingRoll[i].value == 1)
                {
                    bettingTotal[i].text = "" + (int.Parse(small) + int.Parse(big)) * GetCheckedNumber(i) * Permutations(4, CalculateNumbers(bettingNumberInput[i].text));
                }
                else
                {
                    bettingTotal[i].text = "" + (int.Parse(small) + int.Parse(big)) * GetCheckedNumber(i);
                }
            }
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

    // --------------------  Calculate betting amount according to roll --------------------
    public int Factorial(int x)
    {
        if (x <= 1)
            return 1;
        else
            return x * Factorial(x - 1);
    }

    public int Permutations(int a, int b)
    {
        if (a <= 1)
            return 1;

        return Factorial(a) / (Factorial(a - b + 1));
    }

    public int CalculateNumbers(string inputNumbers)
    {
        List<int> repeatedNums = new List<int>();
        int[] array = new int[4];  
        var dict = new Dictionary<int, int>();

        for (int i = 0; i < 4; i++)
        {
            array[i] = int.Parse(inputNumbers[i].ToString());
        }
        
        foreach (var value in array)
        {
            // When the key is not found, "count" will be initialized to 0
            dict.TryGetValue(value, out int count);
            dict[value] = count + 1;
        }

        foreach (var pair in dict)
        {
            repeatedNums.Add(pair.Value);
        }

        return repeatedNums.Count();
    }

    public bool IsPossibile()
    {
        bool isPossible = false;
        
        for(int i = 0; i < MainUI.bettingCount; i++)
        {

        }

        return isPossible;
    }

    //--------------------- Initialize betting parameters ------------------------
    public void InitializeBet()
    {
        for (int i = 0; i < MainUI.bettingCount; i++)
        {
            MainUI.instance.bettingList[i].SetActive(false);
            bettingRoll[i].value = 0;
            bettingNumberInput[i].text = "";
            bettingBigInput[i].text = "";
            bettingSmallInput[i].text = "";
            bettingCompanyM[i].isOn = false;
            bettingCompanyD[i].isOn = false;
            bettingCompanyT[i].isOn = false;
            bettingCompanyG[i].isOn = false;            
        }

        MainUI.instance.bettingBtns.transform.position = MainUI.instance.buttonsOriginal.transform.position;
        MainUI.instance.bettingContent.transform.position = MainUI.instance.bettingContentOriginal.transform.position;
        MainUI.instance.bettingList[0].SetActive(true);
        MainUI.bettingCount = 1;
    }

    IEnumerator CreateBetAction()
    {
        MainUI.instance.loadingUI.SetActive(true);

        for (int i = 0; i < MainUI.bettingCount; i++)
        {
            if (bettingNumberInput[i].text.Length == 4 && int.Parse(bettingTotal[i].text) > 0)
            {
                string roll = "";
                string small = bettingSmallInput[i].text;
                string big = bettingBigInput[i].text;

                if(bettingRoll[i].value == 0)
                {
                    roll = "straight";
                }
                else if(bettingRoll[i].value == 1)
                {
                    roll = "pao";
                }
                else
                {
                    roll = "i-box";
                }

                if(small == "") small = "0";
                if (big == "") big = "0";

                StartCoroutine(PostBet(betURL, bettingNumberInput[i].text, int.Parse(big), int.Parse(small), GetCompanyCode(i), MainUI.userID, roll, bettingTotal[i].text));
                yield return new WaitForSeconds(1f);
            }
        }

        //yield return new WaitForSeconds(2f);
        InitializeBet();
        MainUI.instance.loadingUI.SetActive(false);
    }

    IEnumerator PostBet(string url, string number, int big, int small, string company, int id, string roll, string total)
    {
        WWWForm form = new WWWForm();
        form.AddField("number", number);
        form.AddField("big", big);
        form.AddField("small", small);
        form.AddField("company", company);
        form.AddField("id", id);
        form.AddField("roll", roll);

        UnityWebRequest uwr = UnityWebRequest.Post(url, form);
        yield return uwr.SendWebRequest();
        MainUI.instance.loadingUI.SetActive(false);

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
            Result loadData = JsonUtility.FromJson<Result>(uwr.downloadHandler.text);

            if(loadData.result == 0)
            {
                StartCoroutine(DelayShowAlertText("No enough balance."));
            }
        }
    }

    IEnumerator DelayShowAlertText(string text)
    {
        MainUI.instance.alertText.gameObject.SetActive(true);
        MainUI.instance.alertText.text = text;
        yield return new WaitForSeconds(3f);
        MainUI.instance.alertText.gameObject.SetActive(false);
    }
}
