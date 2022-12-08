using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    //Manages dialogue for the game

    private List<DialogueInteractable.Sentence> sentences;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public Canvas textBoxC;
    public GameObject textBox;
    public GameObject weakChoiceButton;
    //public GameObject strongChoiceButton;

    GameObject[] buttons;
    TextMeshProUGUI[] buttonsText;

    int currentSentence;

    [SerializeField]
    private Image blinkIcon;

    //List<string> lastingImpacts;

    [SerializeField]
    private Player player;
    [SerializeField]
    private ThirdPersonMovement playerM;

    private bool canMoveOn;
    private bool waitingForPlayerToContinue;

    int nextSentence;

    bool isPressingButton;

    [HideInInspector]
    public bool dialogueRunning;

    Button currentButton;

    void Start()
    {
        //blinkIcon.enabled = false;

        buttons = new GameObject[1];
        buttonsText = new TextMeshProUGUI[1];

        canMoveOn = false;
        waitingForPlayerToContinue = false;

        nextSentence = 0;
        
        isPressingButton = false;

        dialogueRunning = false;
    }

    private void Update() 
    {
        if(canMoveOn && waitingForPlayerToContinue && !isPressingButton)
        {
            isPressingButton = true;
            waitingForPlayerToContinue = false;
            if(nextSentence < sentences.Count)
            DisplayNextSentence(nextSentence);

            else EndDialogue();
        }    

        if((Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return)) && canMoveOn)
        isPressingButton = false;
    }

    public void StartDialogue(DialogueInteractable.Dialogue dialogue)
    {
        if(!dialogueRunning)
        {
            dialogueRunning = true;
            player.isReadingDialogue = true;
        
            sentences = new List<DialogueInteractable.Sentence>();
            playerM.canMove = false;

            textBoxC.enabled = true;
            
            sentences.Clear();
            foreach(GameObject button in buttons) if(button != null) Destroy(button);

            foreach(DialogueInteractable.Sentence sentence in dialogue.sentences)
            {
                sentences.Add(sentence);
            }

            DisplayNextSentence(0);
        }
    }

    public void DisplayNextSentence(int sentenceID)
    {
        currentSentence = sentenceID;

        if(sentenceID >= sentences.Count)
        {
            EndDialogue();
        }
        else
        {
            foreach(GameObject button in buttons) if(button != null) Destroy(button);

            DialogueInteractable.Sentence sentence = sentences[sentenceID];

            if(sentence.weakChoice.Length == 1)
            {
                nextSentence = sentence.weakChoice[0].nextTextID;
            }

            nameText.text = sentence.speaker;
            StopAllCoroutines();
            StartCoroutine(TypeSentence(sentence));
        }
    }

    public void GrantWeakChoice(int numberOfChoices, DialogueInteractable.WeakChoice[] weakChoice)
    {
        foreach(GameObject button in buttons) if(button != null) Destroy(button);
        buttons = new GameObject[numberOfChoices];
        buttonsText = new TextMeshProUGUI[numberOfChoices];
        for(int i = 0; i < numberOfChoices; i++)
        {
            buttons[i] = Instantiate(weakChoiceButton) as GameObject;
            buttons[i].transform.SetParent(textBox.transform, false);
            buttons[i].transform.position = new Vector3(buttons[i].transform.position.x, buttons[i].transform.position.y + 30 * i, buttons[i].transform.position.z);
            Button button = buttons[i].GetComponent<Button>();
            buttonsText[i] = buttons[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            buttonsText[i].text = weakChoice[i].choiceText;

            int weakChoiceID = i;
            if(weakChoice[weakChoiceID].nextTextID != 0) 
            {
                button.onClick.AddListener(delegate{DisplayNextSentence(weakChoice[weakChoiceID].nextTextID);});
            }
            // else if(sentences[currentSentence].gameImpact.Length > 0)
            // {
            //     foreach(Impact impact in sentences[currentSentence].gameImpact)
            //     {
            //         if(lastingImpacts.Contains(impact.impact)) button.onClick.AddListener(delegate{DisplayNextSentence(impact.nextTextID);});
            //     }
            // }
        }

        if(buttons.Length > 0 && buttons[0] != null)
        EventSystem.current.SetSelectedGameObject(buttons[0]);
    }

    private void EndDialogue()
    {
        playerM.canMove = true;

        dialogueRunning = false;
        textBoxC.enabled = false;
        EventSystem.current.SetSelectedGameObject(null);
        player.isReadingDialogue = false;
    }

    IEnumerator TypeSentence(DialogueInteractable.Sentence sentence)
    {
        dialogueText.text = "";
        foreach (char character in sentence.text.ToCharArray())
        {
            dialogueText.text += character;
            yield return new WaitForSeconds(.025f);
        }

        if(sentence.weakChoice != null && sentence.weakChoice.Length > 0)
        GrantWeakChoice(sentence.weakChoice.Length, sentence.weakChoice);
        

        canMoveOn = true;

        if(canMoveOn && (sentence.weakChoice == null || sentence.weakChoice.Length <= 1))
        StartCoroutine(BlinkIcon());

        StopCoroutine(TypeSentence(sentence));
    }

    IEnumerator BlinkIcon()
    {
        waitingForPlayerToContinue = true;
        while(waitingForPlayerToContinue) 
        {
            //blinkIcon.enabled = true;
            yield return new WaitForSeconds(.5f);
            //blinkIcon.enabled = false;
            yield return new WaitForSeconds(.3f);
        }
        waitingForPlayerToContinue = false;

        StopCoroutine(BlinkIcon());
    }
}