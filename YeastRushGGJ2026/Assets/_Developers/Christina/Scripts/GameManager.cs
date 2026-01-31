using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; // necessary for Events
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Events to assign to, triggers events in other scripts
    public event UnityAction PlayNextStep;
    public event UnityAction PlayPreviousStep;
    public event UnityAction GoToStep;
    public event UnityAction PlayStep;
    public event UnityAction PauseStep;
    public event UnityAction UpdateCurrScore;
    public event UnityAction UpdateCurrStep;
    public GameManager_Storyline_UI storylineS; // storyline script on the hand ui which stores the currstep in the game

    void Start()
    {
        
    }

    void Update()
    {
//        if (Input.GetKeyDown(KeyCode.Escape))
//            Application.Quit();

//        if(Input.GetKeyDown(KeyCode.R))
//            Restart();

        if(Input.GetKeyDown(KeyCode.Alpha1))
            PlayNextStepEvent(); // Controls.Player.keyboard.performed += PlayNextStepEvent();

    }

    // ACTIVATE EVENTS for example via menu buttons
    public void PlayNextStepEvent() // send a warning that the content will be changed
    {
        Debug.Log("PlayNextStepEvent");
        PlayNextStep?.Invoke(); // Pauses Video in GameManager if action is present
    }


    public void PlayStepsEvent() // send a warning that the content will be changed
    {
        Debug.Log("PlayEvent");
        PlayStep?.Invoke(); // Pauses Video in GameManager if action is present
    }

    public void PauseStepsEvent() // send a warning that the content will be changed
    {
        Debug.Log("PauseEvent");
        PauseStep?.Invoke(); // Pauses Video in GameManager if action is present
    }
    public void GoToStepEvent(int targetStep) // event that others can subscribe to
    {
        Debug.Log("GotoEvent " + targetStep);        
        GoToStep?.Invoke(); // invoke event if it is present
    }
/*
    public void PlayNextStepEvent() // event that others can subscribe to
    {
        Debug.Log("PlayNextEvent");
        if (currStep < content.Count - 2) // more steps to make
        {
            currStep++;
        }
        else // TUTORIAL ENDED > RESET
        {
            // do nothing
        }

        PlayNextStep?.Invoke(); // invoke event if it is present
    }*/
    
    public void Restart()
    {
        // Reload the current scene.
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }
}
