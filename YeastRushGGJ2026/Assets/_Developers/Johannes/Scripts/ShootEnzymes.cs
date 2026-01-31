using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.InputSystem;


public class ShootEnzymes : MonoBehaviour
{
    // 2. These variables are to hold the Action references
    List<InputAction> results = new List<InputAction>();


    private GameObject leftController;
    private GameObject rightController; 
    
    [SerializeField]
    private GameObject enzymePrefab;

    private float lastShotTime = 0f;
    private float shootCooldown = 0.25f;
    private int leftControllerIndex = -1;
    private int rightControllerIndex = -1;
    private int lastControllerIndex = 0;

    private void Start()
    {

        foreach (var map in InputSystem.actions.actionMaps)
        {
            var action = map.FindAction("Activate Value");
            if (action != null)
                results.Add(action);
        }
    }

    void Update()
    {
        if (leftController == null || rightController == null)
        {
            // Find controllers in the scene - warning: ids can change depending on load order
            leftController = GameObject.Find("Left Controller");
            
            if (leftController != null)
            {
                // Debug.Log("We found the Left Controller.");
                leftControllerIndex = lastControllerIndex;
                Debug.Log(leftControllerIndex.ToString() + " <--- Left Controller");
                lastControllerIndex++;
            }
            rightController = GameObject.Find("Right Controller");
            if (rightController != null)
            {
                rightControllerIndex = lastControllerIndex;
                Debug.Log(rightControllerIndex.ToString() + " <--- Right Controller");
                lastControllerIndex++;
            }
        }
        else
        for (int i = 0; i < results.Count; i++)
        {
            var activateAction = results[i];
            float moveValue = activateAction.ReadValue<float>();

            if (moveValue > 0.5f && lastShotTime + shootCooldown < Time.time)
            {
                // Instantiate enzyme at correct controller position and rotation
                if (i == rightControllerIndex) // Assuming first action is for right controller    
                {            
                    GameObject enzyme = Instantiate(enzymePrefab, rightController.transform.position, rightController.transform.rotation);
                    enzyme.GetComponent<Rigidbody>().linearVelocity = rightController.transform.forward * 10f; // Adjust speed as needed
                    lastShotTime = Time.time;
                }
                else if (i == leftControllerIndex) // Assuming second action is for left controller
                    {
                        GameObject enzyme = Instantiate(enzymePrefab, leftController.transform.position, leftController.transform.rotation);
                        enzyme.GetComponent<Rigidbody>().linearVelocity = leftController.transform.forward * 10f; // Adjust speed as needed
                        lastShotTime = Time.time;
                    }
            }

            // your movement code here
//            Debug.Log("Activate Value: " + moveValue);
        }        

    }
}