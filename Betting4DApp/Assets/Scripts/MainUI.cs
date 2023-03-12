using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Rendering;
using System;

public class MainUI : MonoBehaviour
{       
    [Header("---------- Main UI ----------")]
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject sidebarUI;
    [SerializeField] private GameObject loginUI;
    [SerializeField] private GameObject profileUI;
    [SerializeField] private GameObject homeUI;
    [SerializeField] private GameObject bettingUI;
    [SerializeField] private GameObject yourbetsTab;
    [SerializeField] private GameObject betTab;
    [SerializeField] private GameObject ticketsTab;
    [SerializeField] private GameObject backToBtn;
    [SerializeField] private InputField phoneNumberInput;
    [SerializeField] private InputField passwordInput;
    [SerializeField] private Text alertText;

    [SerializeField] private Text[] betResult;

    [Header("---------- Betting Page ----------")]
    [SerializeField] private GameObject[] bettingList;    
    [SerializeField] private GameObject bettingBtns;

    [Header("---------- Your Bets ----------")]
    [SerializeField] private GameObject yourBetsList;
    [SerializeField] private GameObject betLists;

    [SerializeField] private Text ticketDate;
    [SerializeField] private Text ticketTime;
    [SerializeField] private GameObject ticketSet;
    [SerializeField] private Text ticketNT;
    [SerializeField] private Text ticketNo;

    public static int bettingCount;
    public static int userID;
    public static MainUI instance;
    public string absURL = "";
    string signupURL;
    string signinURL;
    string betHistoryURL;
    string betResultURL;
    string getTicketURL;

    public class Result
    {
        public string result;
        public int id;
        public string[] histories;
        public string[] ranknumbers;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        signupURL = absURL + "api/signup";
        signinURL = absURL + "api/login";
        betHistoryURL = absURL + "api/getBetHistory";
        betResultURL = absURL + "api/getRankNumbers";
        getTicketURL = absURL + "api/getTicket";
        bettingCount = 1;
    }

    private void Update()
    {
        mainUI.transform.localScale = new Vector3(Screen.width / 1170f, Screen.height / 2532f, 1f);
    }

    #region UI Functionalities

    // -------------------------- Sign Up/In ----------------------------------

    public void Signup()
    {

    }

    public void Login()
    {
        if (phoneNumberInput.text != "" && passwordInput.text != "")
        {
            // Request "sign in" Api.
            StopAllCoroutines();
            StartCoroutine(PostSignIn(signinURL, phoneNumberInput.text.ToString(), passwordInput.text.ToString()));
        }
        else
        {
            StartCoroutine(DelayShowAlertText("Please fill all fields."));
        }
    }

    // -------------------------- Betting UI ----------------------------------
    public void PlusBtnClick()
    {
        if(bettingCount < 5)
        {
            bettingCount++;
            bettingList[bettingCount - 1].SetActive(true);
            bettingBtns.transform.position = new Vector3(bettingBtns.transform.position.x, (bettingBtns.transform.position.y) - 275f / 2532f * Screen.height, 0);
        }
    }

    public void MinusBtnClick()
    {
        if(bettingCount > 1)
        {            
            bettingList[bettingCount - 1].SetActive(false);
            bettingBtns.transform.position = new Vector3(bettingBtns.transform.position.x, (bettingBtns.transform.position.y) + 275f / 2532f * Screen.height, 0);
            bettingCount--;
        }
    } 

    public void YourBetsTabClick()
    {
        yourbetsTab.SetActive(true);
        betTab.SetActive(false);
        ticketsTab.SetActive(false);

        GameObject[] yourBets = GameObject.FindGameObjectsWithTag("YourBet");

        for(int i = 0; i < yourBets.Length; i++)
        {
            GameObject.Destroy(yourBets[i]);
        }

        StopAllCoroutines();
        StartCoroutine(PostBetHistory(betHistoryURL, userID));
    }

    public void BetTabClick()
    {
        yourbetsTab.SetActive(false);
        betTab.SetActive(true);
        ticketsTab.SetActive(false);
    }

    public void TicketsTabClick()
    {
        yourbetsTab.SetActive(false);
        betTab.SetActive(false);
        ticketsTab.SetActive(true);

        StartCoroutine(GetTicket(getTicketURL, userID));
    }

    // -------------------------- Side Menu ---------------------------

    public void SettingsBtnClick()
    {
        sidebarUI.SetActive(true);
        backToBtn.SetActive(true);
    }

    public void HomeBtnClick()
    {
        HideAllUI();
        homeUI.SetActive(true);
    }

    public void ProfileBtnClick()
    {
        HideAllUI();
        profileUI.SetActive(true);
    }

    public void BettingBtnClick()
    {
        HideAllUI();
        bettingUI.SetActive(true);
        BetTabClick();
    }

    public void HistoryBtnClick()
    {
        HideAllUI();
        bettingUI.SetActive(true);
        YourBetsTabClick();
    }

    public void AdminActionsBtnClick()
    {

    }

    public void LogoutBtnClick()
    {

    }

    public void HideAllUI()
    {
        sidebarUI.SetActive(false);
        backToBtn.SetActive(false);
        profileUI.SetActive(false);
        homeUI.SetActive(false);
        bettingUI.SetActive(false);
    }

    #endregion

    #region API Integration
    IEnumerator PostSignIn(string url, string phoneNumber, string password)
    {
        // Send user phone number and password.
        WWWForm form = new WWWForm();
        form.AddField("phoneNumber", phoneNumber);
        form.AddField("password", password);

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
            string result = loadData.result;            

            if (url == signinURL)
            {
                if (string.Equals(result, "1")) // Sign in successfully.
                {
                    loginUI.SetActive(false);
                    homeUI.SetActive(true);
                    userID = loadData.id;

                    StopAllCoroutines();
                    StartCoroutine(GetBetResult(betResultURL));
                }
                else if (string.Equals(result, "2")) // User doesn't existed
                {
                    StartCoroutine(DelayShowAlertText("Not registered"));
                }
                else // Enter wrong password.
                {
                    StartCoroutine(DelayShowAlertText("Wrong password"));
                }

            }
            else if (url == signupURL)
            {
                if (string.Equals(result, "1")) // User already exists
                {
                    StartCoroutine(DelayShowAlertText("Already Exists"));
                }
                else // Sign up succesfully.
                {                    
                }
            }
        }
    }

    IEnumerator PostBetHistory(string url, int id)
    {
        WWWForm form = new WWWForm();
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
            var data = (JObject)JsonConvert.DeserializeObject(uwr.downloadHandler.text);
            int betListCount = JsonUtility.FromJson<Result>(uwr.downloadHandler.text).histories.Length;
            
            for(int i = 0; i < betListCount; i++)
            {
                GameObject cloneList = Instantiate(yourBetsList, yourBetsList.transform.position - new Vector3(0, (1500f + 150f * i) / 2532f * Screen.height, 0), transform.rotation);
                cloneList.transform.localScale = new Vector3(Screen.width / 1170f, Screen.height / 2532f, 1f);
                cloneList.transform.SetParent(betLists.transform);
                cloneList.tag = "YourBet";

                cloneList.transform.GetChild(0).gameObject.GetComponent<Text>().text = data.SelectToken("histories[" + i + "].created_at").ToString().Split(" ")[0];
                cloneList.transform.GetChild(1).gameObject.GetComponent<Text>().text = data.SelectToken("histories[" + i + "].created_at").ToString().Split(" ")[1];
                cloneList.transform.GetChild(2).gameObject.GetComponent<Text>().text = data.SelectToken("histories[" + i + "].company").ToString();
                cloneList.transform.GetChild(3).gameObject.GetComponent<Text>().text = data.SelectToken("histories[" + i + "].number").ToString();
                cloneList.transform.GetChild(4).gameObject.GetComponent<Text>().text = data.SelectToken("histories[" + i + "].big").ToString();
                cloneList.transform.GetChild(5).gameObject.GetComponent<Text>().text = data.SelectToken("histories[" + i + "].small").ToString();
                cloneList.transform.GetChild(7).gameObject.GetComponent<Text>().text = data.SelectToken("histories[" + i + "].total").ToString();
                cloneList.transform.GetChild(8).gameObject.GetComponent<Text>().text = data.SelectToken("histories[" + i + "].ticketno").ToString();
            }
        }
    }

    IEnumerator GetBetResult(string url)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        yield return uwr.SendWebRequest();

        if(uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error while sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);

            Result loadData = JsonUtility.FromJson<Result>(uwr.downloadHandler.text);

            for(int i = 0; i < loadData.ranknumbers.Length; i++)
            {
                betResult[i].text = loadData.ranknumbers[i].ToString();
            }           
        }
    }

    IEnumerator GetTicket(string url, int id)
    {
        WWWForm form = new WWWForm();
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
            var data = (JObject)JsonConvert.DeserializeObject(uwr.downloadHandler.text);

            ticketDate.text = data.SelectToken("ticket.created_at").ToString().Split(" ")[0];
            ticketTime.text = data.SelectToken("ticket.created_at").ToString().Split(" ")[1];

            ticketSet.transform.GetChild(0).gameObject.GetComponent<Text>().text = data.SelectToken("ticket.company").ToString();
            ticketSet.transform.GetChild(1).gameObject.GetComponent<Text>().text = data.SelectToken("ticket.number").ToString();
            ticketSet.transform.GetChild(2).gameObject.GetComponent<Text>().text = "B" + data.SelectToken("ticket.big").ToString();
            ticketSet.transform.GetChild(3).gameObject.GetComponent<Text>().text = "S" + data.SelectToken("ticket.small").ToString();
            ticketSet.transform.GetChild(5).gameObject.GetComponent<Text>().text = data.SelectToken("ticket.total").ToString();

            ticketNo.text = data.SelectToken("ticket.ticketno").ToString();
            ticketNT.text = data.SelectToken("ticket.total").ToString();
        }
    }

    IEnumerator DelayShowAlertText(string text)
    {
        alertText.gameObject.SetActive(true);
        alertText.text = text;
        yield return new WaitForSeconds(2f);
        alertText.gameObject.SetActive(false);
    }

    #endregion
}
