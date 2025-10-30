using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class DialogManager : MonoBehaviour
{
    public TMP_Text actorName;
    public TMP_Text messageText;
    public RectTransform backGroundBox;
    public bool isActive = false;

    Message[] currentMessages;
    Actor[] currentActors;
    int activeMessage = 0;

    public void OpenDialogue(Message[] messages, Actor[] actors)
    {
        currentMessages = messages;
        currentActors = actors;
        activeMessage = 0;
        DisplayMessage();
        backGroundBox.DOScale(new Vector3(1f, 1f, 1f), 0.5f).SetEase(Ease.InOutExpo);
    }

    public float typingSpeed = 0.03f;
    private Coroutine typingCoroutine;

    void DisplayMessage()
    {
        Message messageToDisplay = currentMessages[activeMessage];
        Actor actorToDisplay = currentActors[messageToDisplay.actorId];

        actorName.text = actorToDisplay.name;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(messageToDisplay.message));
    }

    private bool isTyping = false;

    IEnumerator TypeText(string message)
    {
        isTyping = true;
        messageText.text = "";
        foreach (char c in message)
        {
            messageText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }

    public void NextMessage()
    {
        activeMessage++;
        if (activeMessage < currentMessages.Length)
        {
            DisplayMessage();
        }
        else
        {
            backGroundBox.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InOutExpo);
        }
    }

    void Start()
    {
        backGroundBox.transform.localScale = Vector3.zero;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                SkipTyping();
            }
            else
            {
                NextMessage();
            }
        }
    }

    void SkipTyping()
    {
        StopCoroutine(typingCoroutine);
        Message messageToDisplay = currentMessages[activeMessage];
        messageText.text = messageToDisplay.message;
        isTyping = false;
    }
}
