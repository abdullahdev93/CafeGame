using System.Collections;
using UnityEngine;
using CHARACTERS;

public class InputPanelTesting : MonoBehaviour
{
    public InputPanel inputPanel;

    void Start()
    {
        StartCoroutine(Running());
    }

    IEnumerator Running()
    {
        Character Stella = CharacterManager.instance.CreateCharacter("Stella", revealAfterCreation: true);

        yield return Stella.Say("Hi! What's your name?");

        inputPanel.Show("What Is Your Name?");

        while (inputPanel.isWaitingOnUserInput)
            yield return null;

        string firstName = inputPanel.firstName;
        string lastName = inputPanel.lastName;
        string pronounSubject = inputPanel.pronounSubject;
        string pronounObject = inputPanel.pronounObject;

        yield return Stella.Say($"It's very nice to meet you, {firstName} {lastName}! I see you go by {pronounSubject}/{pronounObject} pronouns.");
        yield return Stella.Say($"I'll be sure to let others know to address you as {pronounSubject} or refer to you as {pronounObject}.");
    }
}
