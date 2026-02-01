using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class GameManager_Storyline_UI : MonoBehaviour
{
    public int currStep = 0;
    public UiMode currUiMode; 
    public int points = 0;
    public int maximumPoints = 20;
    public TMP_Text pointDisplay;
    public TMP_Text playTime;
    public int timeGameEnds = 100;   
    [HideInInspector] public float startTime = 0;    
    [HideInInspector] public float currentTime = 0;
    private AudioSource audioS;

    public enum UiMode // narrative / edu / tech are only spawned when a new step is selected
    {
        narrative,
        educational,
        technical,
        statistics // stats are treated separatedly and are never destroyed
    }

    public List<Step> storyline = new List<Step>();
    public List<Button> uiButtons = new List<Button>();
    public TMP_Text stepName; 
    public TMP_Text stepInstruction; 
    public GameObject contentTarget; // new content will be spawned here;
    public GameObject statContent;  // container for the statcontent
    private GameManager gameS;
    public bool gameFinished = false;
    public GameObject gameFinishedDefeatedContainer; // stores a pop up info when game ends badly
    public GameObject rewardInfo;

    void Start()
    {
        GameObject go = GameObject.FindWithTag("GameManager");
        if (go != null)
        { 
            gameS = go.GetComponent<GameManager>();
        }
        if (gameS != null)
        {
            gameS.storylineS = this.GetComponent<GameManager_Storyline_UI>(); // reference this script in the manager
            gameS.PlayNextStep += NextStep; // assign the next step display to the event in the manager
            //gameS.UpdateCurrScore += IncreasePoins;
        }
        startTime = Time.realtimeSinceStartup;
        audioS = this.gameObject.AddComponent<AudioSource>();
    }
    void OnAwake()
    {
        rewardInfo.SetActive(false);
        rewardInfo.SetActive(true);
    }

    void Update()
    {
        if(currStep < storyline.Count - 1)
        {
            if(points >= storyline[currStep+1].pointsToReachNextLevel)
            NextStep();
        }
        if(currentTime > timeGameEnds)
        {
            gameFinished = true;
            // Display interaction
            gameFinishedDefeatedContainer.SetActive(true);
        }
        if(!gameFinished)
        {
           currentTime = Time.realtimeSinceStartup-startTime;
           playTime.text = (Mathf.RoundToInt(timeGameEnds-currentTime)).ToString();//Mathf.Floor(currentTime / 60 ).ToString("00") + " : " + Mathf.FloorToInt(currentTime%60).ToString("00");
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
            NextStep(); // Controls.Player.keyboard.performed += PlayNextStepEvent();
        if(Input.GetKeyDown(KeyCode.Alpha2))
           IncreasePoints(); // Controls.Player.keyboard.performed += PlayNextStepEvent();
        
    }
    public void IncreasePoints(int value = 1) //int value)
    {
        points+= value;
        Debug.Log("increase points");
        pointDisplay.text = points.ToString();
        rewardInfo.SetActive(false);
        rewardInfo.SetActive(true);

        Debug.Log("-------UI: increase point successfully");
    }
    public void IncreaseTime(int value = 1)
    {
        timeGameEnds += value;
    }

    public void NextStep()
    {
        if (currStep < storyline.Count - 1) // there is more content to display
        {
            // Reset content of the last step
            /*
            if(currStep >= 0)
            {
                if(storyline[currStep].showObject != null)
                storyline[currStep].showObject.SetActive(false);
                uiButtons[(int)currUiMode].GetComponent<Image>().color = uiButtons[(int)currUiMode].colors.normalColor;
            }   */ 

            currStep++; // increase the counter 
           //currUiMode = storyline[currStep].activeMode; // save the newly selected mode
            //display the content of the new step
            if(storyline[currStep].showObject != null)
                storyline[currStep].showObject.SetActive(true);
           // stepName.text = storyline[currStep].name;
            stepInstruction.text =  storyline[currStep].instruction;
            audioS.clip = storyline[currStep].audioInstruction;
            audioS.Play();
            
            //SetUIMode(currUiMode); //storyline[currStep].activeMode); // removed for the game jam
            //if(storyline[currStep].narContent != null)
             //       contentTarget.GetComponent<SpriteRenderer>().sprite = storyline[currStep].narContent;
        }    
    }
/*
    public void SetUIModeWithController(int selection)
    {
        switch(selection)
        {
            case 10:
                SetUIMode(UiMode.narrative);
            break;
            case 1:
                SetUIMode(UiMode.educational);
            break;
            case 2:
                SetUIMode(UiMode.technical);
            break;
            case 3:
                SetUIMode(UiMode.statistics);
            break;
        }
    }    


    private void SetUIMode(UiMode newMode) // switch the ui menu to another UIMode and spawn new content // accessed with controller in the ui menu or when a new chapter starts
    {
        // Reset everything before displaying new content
        statContent.SetActive(false);
        contentTarget.SetActive(false);
        uiButtons[(int)currUiMode].GetComponent<Image>().color = uiButtons[(int)currUiMode].colors.highlightedColor;

        switch(newMode) // display new selection
        {
            case UiMode.narrative:
                if(storyline[currStep].narContent != null)
                    contentTarget.GetComponent<SpriteRenderer>().sprite = storyline[currStep].narContent;
                contentTarget.SetActive(true);
            break;
            
            case UiMode.educational:
                if(storyline[currStep].eduContent != null)
                    contentTarget.GetComponent<SpriteRenderer>().sprite = storyline[currStep].eduContent;
                contentTarget.SetActive(true);
            break; 

            case UiMode.technical:
                if(storyline[currStep].tecContent != null)
                    contentTarget.GetComponent<SpriteRenderer>().sprite = storyline[currStep].tecContent;
                contentTarget.SetActive(true);
            break; 

            case UiMode.statistics: // display the statistics
                statContent.SetActive(true);
            break; 
            

        }
    }*/

    [System.Serializable]
    public class Step
    {
        public string name;
        //public UiMode activeMode; // select the mode the ui should display
        public AudioClip audioInstruction;
        public string instruction = " ";
        public GameObject showObject; // add object which should be activated here
        public int pointsToReachNextLevel = 0;
        /*public Sprite narContent = null; // display narrative image description
        public Sprite eduContent = null; // display educational image description
        public Sprite tecContent = null; // display technical image description
        

        // public bool autoplay;*/
    }
    [System.Serializable]
    public class UIModeContainer
    {
        public string name; 
        public GameObject container;
    }
}