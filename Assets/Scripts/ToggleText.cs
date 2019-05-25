using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleText : MonoBehaviour
{
    public GameObject dialoguePanel;

    // Start is called before the first frame update
    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(dialoguePanel.activeSelf == true)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                dialoguePanel.SetActive(false);
            }
        }
        else if(dialoguePanel.activeSelf == false)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                dialoguePanel.SetActive(true);
            }
        }
    }
}
