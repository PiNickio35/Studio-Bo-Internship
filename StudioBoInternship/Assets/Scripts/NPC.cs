using System;
using System.Collections;
using Interfaces;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialogue dialogueData;
    private DialogueController _dialogueUI;
    private int _dialogueIndex;
    private bool _isTyping, _isDialogueActive;

    private void Start()
    {
        _dialogueUI = DialogueController.Instance;
    }

    public void Interact()
    {
        if (dialogueData == null || (PauseController.IsGamePaused && !_isDialogueActive)) return;

        if (_isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }

    public bool CanInteract()
    {
        return !_isDialogueActive;
    }

    private void StartDialogue()
    {
        _isDialogueActive = true;
        _dialogueIndex = 0;
        
        _dialogueUI.SetNPCInfo(dialogueData.npcName, dialogueData.npcPortrait);
        
        _dialogueUI.ShowDialogueUI(true);
        PauseController.SetPause(true);
        
        StartCoroutine(TypeLine());
    }

    private void NextLine()
    {
        if (_isTyping)
        {
            StopAllCoroutines();
            _dialogueUI.SetDialogueText(dialogueData.dialogueLines[_dialogueIndex]);
            _isTyping = false;
        }
        else if (++_dialogueIndex < dialogueData.dialogueLines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();    
        }
    }

    private IEnumerator TypeLine()
    {
        _isTyping = true;
        _dialogueUI.SetDialogueText("");

        foreach (char letter in dialogueData.dialogueLines[_dialogueIndex])
        {
            _dialogueUI.SetDialogueText(_dialogueUI.dialogueText.text += letter);
            SoundEffectManager.PlayVoice(dialogueData.voiceSound, dialogueData.voicePitch);
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        _isTyping = false;

        if (dialogueData.autoProgressLines.Length > _dialogueIndex && dialogueData.autoProgressLines[_dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    public void EndDialogue()
    {
        StopAllCoroutines();
        _isDialogueActive = false;
        _dialogueUI.SetDialogueText("");
        _dialogueUI.ShowDialogueUI(false);
        PauseController.SetPause(false);
    }
}
