using UnityEngine;
using TMPro;
using System.Collections;

// Creating the slow typewritter effect for script to look natural
public class textAnimation : MonoBehaviour
{
    public TMP_Text textComponent;
    [TextArea] public string fullText;
    // the delay between each character showing up
    public float delay = 0.05f; // Higher is making it slower reading speed
    
    void Start()
    {
        //fallback in case tehre is no text on assigned grab the TMP text
        if (textComponent == null)
            textComponent = GetComponent<TMP_Text>();
        //the typewritter effect
        StartCoroutine(ShowTextSlowly());
    }

    IEnumerator ShowTextSlowly()
    {
        //TMP is made to calculate the information of the text (how many character it has)
        textComponent.text = fullText;
        textComponent.maxVisibleCharacters = 0;
        // Stores the characters in the TMP 
        int totalCharacters = textComponent.textInfo.characterCount;

        // hide everything to start with the typewritter effect
        textComponent.ForceMeshUpdate();
        totalCharacters = textComponent.textInfo.characterCount;

        int visibleCount = 0;
        // the actual typewritter effect
        while (visibleCount <= totalCharacters)
        {
            textComponent.maxVisibleCharacters = visibleCount;
            visibleCount++;
            yield return new WaitForSeconds(delay);
        }
    }
}