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
    [SerializeField] private GameObject signupUI;
    [SerializeField] private GameObject profileUI;
    [SerializeField] private GameObject homeUI;
    [SerializeField] private GameObject bettingUI;
    [SerializeField] private GameObject movingUI;
    public GameObject loadingUI;
    [SerializeField] private GameObject yourbetsTab;
    [SerializeField] private GameObject betTab;
    [SerializeField] private GameObject ticketsTab;
    [SerializeField] private GameObject backToBtn;
    [SerializeField] private InputField phoneNumberInput;
    [SerializeField] private InputField passwordInput;
    [SerializeField] private InputField phoneNumberSignup;
    [SerializeField] private InputField passwordSignup;
    [SerializeField] private InputField passwordCSignup;    
    public Text alertText;    

    [SerializeField] private Text[] betResult;
    [SerializeField] private Dropdown selectCompany;

    [Header("---------- Betting Page ----------")]
    public GameObject[] bettingList;    
    public GameObject bettingBtns;

    [Header("---------- Profile Page ----------")]
    [SerializeField] private Text profilePhonenumber;
    [SerializeField] private InputField profileName;
    public Text balance;

    [Header("---------- Your Bets ----------")]
    [SerializeField] private GameObject yourBetsList;
    [SerializeField] private GameObject betLists;
    public GameObject buttonsOriginal;
    public GameObject bettingContent;
    public GameObject bettingContentOriginal;

    [SerializeField] private Text ticketDate;
    [SerializeField] private Text ticketTime;
    [SerializeField] private Text ticketPhone;
    [SerializeField] private Text ticketUserId;
    [SerializeField] private Text ticketNT;
    [SerializeField] private Text ticketNo;
    [SerializeField] private GameObject[] ticketSet;
    [SerializeField] private GameObject ticketExtra;

    [SerializeField] private GameObject ticketClone;
    [SerializeField] private GameObject ticketTop;
    [SerializeField] private GameObject ticketMain;
    [SerializeField] private GameObject ticketBottom;

    [Header("---------- Move Points ----------")]
    [SerializeField] private Text balanceMove;
    [SerializeField] private InputField phoneNumberF;
    [SerializeField] private InputField phoneNumberB;
    [SerializeField] private InputField amountSend;
    [SerializeField] private GameObject confirmDialog;

    [Header("---------- Extra ----------")]
    [SerializeField] private Text timeType;
    [SerializeField] private Text timeLeft;
    [SerializeField] private Text timeServer;
    [SerializeField] private Button bettingBtn;

    public static int bettingCount;
    public static int userID;
    public static MainUI instance;
    public string absURL = "";
    string signupURL;
    string signinURL;
    string betHistoryURL;
    string betResultURL;
    string getTicketURL;
    string getTimeURL;
    string getBalanceURL;
    string postMovePointsURL;

    bool isBettingTime;

    public class Result
    {
        public string result;
        public int id;
        public int balance;
        public string[] histories;
        public string[] ranknumbers;
        public string[] ticket;
        public string from;
        public string to;
        public int amount;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        loginUI.SetActive(true);
        isBettingTime = false;

        signupURL = absURL + "api/signup";
        signinURL = absURL + "api/login";
        betHistoryURL = absURL + "api/getBetHistory";
        betResultURL = absURL + "api/getRankNumbers";
        getTicketURL = absURL + "api/getTicket";
        getTimeURL = absURL + "api/getTime";
        getBalanceURL = absURL + "api/getPointBalance";
        postMovePointsURL = absURL + "api/movePoint";

        bettingCount = 1;
        phoneNumberInput.text = PlayerPrefs.GetString("PHONE_NUMBER");
        profileName.text = PlayerPrefs.GetString("USER_NAME");        
    }

    private void Update()
    {
        mainUI.transform.localScale = new Vector3(Screen.width / 1170f, Screen.height / 2532f, 1f);
    }

    #region UI Functionalities

    // -------------------------- Sign Up/In ----------------------------------

    public void Signup()
    {
        if (phoneNumberSignup.text.Length != 10)
        {
            StartCoroutine(DelayShowAlertText("Invalid phone number."));
        }
        else if (passwordSignup.text != passwordCSignup.text)
        {
            StartCoroutine(DelayShowAlertText("Passwords do not match."));
        }
        else
        {
            loadingUI.SetActive(false);
            StartCoroutine(PostSignIn(signupURL, phoneNumberSignup.text, passwordSignup.text));
        }
    }

    public void Login()
    {
        if (phoneNumberInput.text != "" && passwordInput.text != "")
        {
            // Request "sign in" Api.
            loadingUI.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(PostSignIn(signinURL, phoneNumberInput.text, passwordInput.text));
        }
        else
        {
            StartCoroutine(DelayShowAlertText("Please fill all fields."));
        }
    }

    public void UpdateProfile()
    {
        PlayerPrefs.SetString("USER_NAME", profileName.text);
    }

    public void SelectCompany()
    {
        loadingUI.SetActive(true);

        if (selectCompany.value == 0)
        {
            StopAllCoroutines();
            StartCoroutine(GetBetResult(betResultURL, "m"));
        }
        else if(selectCompany.value == 1)
        {
            StopAllCoroutines();
            StartCoroutine(GetBetResult(betResultURL, "d"));
        }
        else if (selectCompany.value == 2)
        {
            StopAllCoroutines();
            StartCoroutine(GetBetResult(betResultURL, "t"));
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(GetBetResult(betResultURL, "g"));
        }
    }

    // -------------------------- Betting UI ----------------------------------
    public void PlusBtnClick()
    {
        if(bettingCount < 10)
        {
            bettingCount++;
            bettingList[bettingCount - 1].SetActive(true);
            bettingBtns.transform.position = new Vector3(bettingBtns.transform.position.x, (bettingBtns.transform.position.y) - 275f / 2532f * Screen.height, 0);
            
            if(bettingCount >= 3)
            {
                bettingContent.transform.position = new Vector3(bettingContent.transform.position.x, bettingContent.transform.position.y + 275f / 2532f * Screen.height, 0);
            }
        }
    }

    public void MinusBtnClick()
    {
        if(bettingCount > 1)
        {            
            bettingList[bettingCount - 1].SetActive(false);

            //if(bettingCount < 3)
            //{
            //    bettingBtns.transform.position = new Vector3(bettingBtns.transform.position.x, (bettingBtns.transform.position.y) + 275f / 2532f * Screen.height, 0);
            //}

            bettingBtns.transform.position = new Vector3(bettingBtns.transform.position.x, (bettingBtns.transform.position.y) + 275f / 2532f * Screen.height, 0);
            bettingContent.transform.position = new Vector3(bettingContent.transform.position.x, bettingContent.transform.position.y - 275f / 2532f * Screen.height, 0);
            bettingCount--;
        }
    } 

    public void YourBetsTabClick()
    {
        yourbetsTab.SetActive(true);
        betTab.SetActive(false);
        ticketsTab.SetActive(false);
        loadingUI.SetActive(true);

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
        loadingUI.SetActive(true);
        GameObject[] ticketData = GameObject.FindGameObjectsWithTag("Ticket");

        for(int i = 0; i < ticketData.Length; i++)
        {
            GameObject.Destroy(ticketData[i]);
        }

        StartCoroutine(GetTicketData(getTicketURL, userID));
    }

    // ---------------------------- Move points -----------------------------
    public void SendPointsBtnClick()
    {
        if(phoneNumberF.text != "" && phoneNumberB.text != "" && amountSend.text != "")
        {
            if(int.Parse(amountSend.text) <= int.Parse(balanceMove.text))
            {
                confirmDialog.SetActive(true);
            }
            else
            {
                StartCoroutine(DelayShowAlertText("Insufficient Amount."));
            }                        
        }
        else
        {
            StartCoroutine(DelayShowAlertText("Please input all fields."));
        }
    }
    
    public void AcceptToSend()
    {
        StartCoroutine(PostMoveAmount(postMovePointsURL, PlayerPrefs.GetString("PHONE_NUMBER"), phoneNumberB.text, int.Parse(amountSend.text)));
    }

    // -------------------------- Side Menu ---------------------------

    public void SettingsBtnClick()
    {
        sidebarUI.SetActive(true);
        backToBtn.SetActive(true);
        loadingUI.SetActive(true);

        StartCoroutine(GetServerTime(getTimeURL));
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
        StartCoroutine(GetBalanceData(getBalanceURL, userID));
    }

    public void BettingBtnClick()
    {
        if (isBettingTime)
        {
            HideAllUI();
            bettingUI.SetActive(true);
            BetTabClick();
        }
        else
        {
            StartCoroutine(DelayShowAlertText("Please wait for betting time"));
        }                
    }

    public void HistoryBtnClick()
    {
        HideAllUI();
        bettingUI.SetActive(true);
        YourBetsTabClick();
    }

    public void MovePointsBtnClick()
    {
        HideAllUI();
        movingUI.SetActive(true);
        phoneNumberF.text = "60";
        StartCoroutine(GetBalanceData(getBalanceURL, userID));
    }

    public void LogoutBtnClick()
    {
        HideAllUI();
        loginUI.SetActive(true);
    }

    public void HideAllUI()
    {
        sidebarUI.SetActive(false);
        backToBtn.SetActive(false);
        profileUI.SetActive(false);
        homeUI.SetActive(false);
        bettingUI.SetActive(false);
        movingUI.SetActive(false);
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
        loadingUI.SetActive(false);

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
                    PlayerPrefs.SetString("PHONE_NUMBER", phoneNumberInput.text);
                    profilePhonenumber.text = "+60" + phoneNumberInput.text;

                    StopAllCoroutines();
                    StartCoroutine(GetBetResult(betResultURL, "m"));                    
                }
                else if (string.Equals(result, "2")) // User doesn't existed
                {
                    StartCoroutine(DelayShowAlertText("Not registered."));
                }
                else // Enter wrong password.
                {
                    StartCoroutine(DelayShowAlertText("Wrong password."));
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
                    phoneNumberInput.text = phoneNumberSignup.text;
                    signupUI.SetActive(false);
                    loginUI.SetActive(true);                    
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
        loadingUI.SetActive(false);

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
                cloneList.transform.GetChild(6).gameObject.GetComponent<Text>().text = data.SelectToken("histories[" + i + "].roll").ToString();
                cloneList.transform.GetChild(7).gameObject.GetComponent<Text>().text = data.SelectToken("histories[" + i + "].total").ToString();
                cloneList.transform.GetChild(8).gameObject.GetComponent<Text>().text = data.SelectToken("histories[" + i + "].ticketno").ToString();
            }
        }
    }

    IEnumerator GetServerTime(string url)
    {
        UnityWebRequest uwr = UnityWebRequest.Get(url);
        yield return uwr.SendWebRequest();
        loadingUI.SetActive(false);

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
            var data = (JObject)JsonConvert.DeserializeObject(uwr.downloadHandler.text);

            int currentTime = int.Parse(data.SelectToken("hour").ToString());
            string serverTime = data.SelectToken("time").ToString().Split(" ")[1];

            if (currentTime < 18 && currentTime >= 9)
            {
                timeType.text = "Ending: 18:00";
                timeLeft.text = (18 - currentTime).ToString();
                isBettingTime = true;
            }
            else
            {
                timeType.text = "Starting: 09:00";
                timeLeft.text = ((33 - currentTime) % 24).ToString();
                isBettingTime = false;
            }
                                    
            timeServer.text = serverTime.Split(":")[0] + ":" + serverTime.Split(":")[1];
        }
    }

    IEnumerator GetBetResult(string url, string companyStr)
    {
        WWWForm form = new WWWForm();
        form.AddField("company", companyStr);

        UnityWebRequest uwr = UnityWebRequest.Post(url, form);
        yield return uwr.SendWebRequest();
        loadingUI.SetActive(false);

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
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

    IEnumerator GetTicketData(string url, int id)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);

        UnityWebRequest uwr = UnityWebRequest.Post(url, form);
        yield return uwr.SendWebRequest();
        loadingUI.SetActive(false);

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
            var data = (JObject)JsonConvert.DeserializeObject(uwr.downloadHandler.text);
            int betListCount = JsonUtility.FromJson<Result>(uwr.downloadHandler.text).histories.Length;
            List<int> setCount = new List<int>();
            int cloneCount = 0;
            int ticketCount = 0;
            int nt = 0;
            string currentTicketNum = "";

            for(int i = 0; i < betListCount; i++)
            {
                if (currentTicketNum != data.SelectToken("histories[" + i + "].ticketno").ToString())
                {
                    currentTicketNum = data.SelectToken("histories[" + i + "].ticketno").ToString();
                    setCount.Add(1);
                }
                else
                {
                    setCount[setCount.Count - 1]++;
                }
            }

            for(int i = 0; i < setCount.Count; i++)
            {
                nt = 0;  
                
                // ----------------- Top ---------------------
                GameObject cloneTop = Instantiate(ticketTop, ticketTop.transform.position - new Vector3(0, (2000f + cloneCount * 250f) / 2532f * Screen.height, 0), transform.rotation);
                cloneTop.transform.localScale = new Vector3(Screen.width / 1170f, Screen.height / 2532f, 1f);
                cloneTop.transform.SetParent(ticketClone.transform);
                cloneTop.tag = "Ticket";

                cloneTop.transform.GetChild(0).gameObject.GetComponent<Text>().text = data.SelectToken("histories[" + ticketCount + "].created_at").ToString().Split(" ")[0];
                cloneTop.transform.GetChild(1).gameObject.GetComponent<Text>().text = data.SelectToken("histories[" + ticketCount + "].created_at").ToString().Split(" ")[1];
                cloneTop.transform.GetChild(2).gameObject.GetComponent<Text>().text = "+60" + PlayerPrefs.GetString("PHONE_NUMBER");
                cloneTop.transform.GetChild(3).gameObject.GetComponent<Text>().text = "+60" + PlayerPrefs.GetString("PHONE_NUMBER");
                cloneTop.transform.GetChild(4).gameObject.name = "share:" + data.SelectToken("histories[" + ticketCount + "].ticketno").ToString();

                cloneCount++;

                // -------------------- Main -------------------
                for(int j = 0; j < setCount[i]; j++)
                {
                    GameObject cloneMain = Instantiate(ticketMain, ticketMain.transform.position - new Vector3(0, (2000f + cloneCount * 250f) / 2532f * Screen.height, 0), transform.rotation);
                    cloneMain.transform.localScale = new Vector3(Screen.width / 1170f, Screen.height / 2532f, 1f);
                    cloneMain.transform.SetParent(ticketClone.transform);
                    cloneMain.tag = "Ticket";

                    cloneMain.transform.GetChild(0).gameObject.GetComponent<Text>().text = data.SelectToken("histories[" + ticketCount + "].company").ToString();
                    cloneMain.transform.GetChild(1).gameObject.GetComponent<Text>().text = data.SelectToken("histories[" + ticketCount + "].number").ToString();
                    cloneMain.transform.GetChild(2).gameObject.GetComponent<Text>().text = "B " + data.SelectToken("histories[" + ticketCount + "].big").ToString();
                    cloneMain.transform.GetChild(3).gameObject.GetComponent<Text>().text = "S " + data.SelectToken("histories[" + ticketCount + "].small").ToString();
                    cloneMain.transform.GetChild(4).gameObject.GetComponent<Text>().text = data.SelectToken("histories[" + ticketCount + "].roll").ToString();
                    cloneMain.transform.GetChild(5).gameObject.GetComponent<Text>().text = data.SelectToken("histories[" + ticketCount + "].total").ToString();

                    nt += int.Parse(data.SelectToken("histories[" + ticketCount + "].total").ToString());
                    cloneCount++;
                    ticketCount++;
                }

                // ------------------ Bottom ---------------------
                GameObject cloneBottom = Instantiate(ticketBottom, ticketBottom.transform.position - new Vector3(0, (2000f + cloneCount * 250f) / 2532f * Screen.height, 0), transform.rotation);
                cloneBottom.transform.localScale = new Vector3(Screen.width / 1170f, Screen.height / 2532f, 1f);
                cloneBottom.transform.SetParent(ticketClone.transform);
                cloneBottom.tag = "Ticket";

                cloneBottom.transform.GetChild(0).gameObject.GetComponent<Text>().text = nt.ToString();
                cloneBottom.transform.GetChild(1).gameObject.GetComponent<Text>().text = data.SelectToken("histories[" + (ticketCount - 1) + "].ticketno").ToString();
                cloneCount++;
            }
        }
    }

    IEnumerator GetBalanceData(string url, int id)
    {
        WWWForm form = new WWWForm();
        form.AddField("user_id", id);
        loadingUI.SetActive(true);

        UnityWebRequest uwr = UnityWebRequest.Post(url, form);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
            Result loadData = JsonUtility.FromJson<Result>(uwr.downloadHandler.text);
            loadingUI.SetActive(false);

            balance.text = loadData.balance.ToString();
            balanceMove.text = loadData.balance.ToString();
        }
    }

    IEnumerator PostMoveAmount(string url, string from, string to, int amount)
    {
        WWWForm form = new WWWForm();
        form.AddField("from", from);
        form.AddField("to", to);
        form.AddField("amount", amount);
        loadingUI.SetActive(true);

        UnityWebRequest uwr = UnityWebRequest.Post(url, form);
        yield return uwr.SendWebRequest();
        loadingUI.SetActive(false);

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
            Result loadData = JsonUtility.FromJson<Result>(uwr.downloadHandler.text);
            
            if(loadData.result == "2")
            {
                phoneNumberB.text = "";
                amountSend.text = "";
                StartCoroutine(GetBalanceData(getBalanceURL, userID));
            }
            else
            {                
                StartCoroutine(DelayShowAlertText("Invalid user"));                
            }
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
