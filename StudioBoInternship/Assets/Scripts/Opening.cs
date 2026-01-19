using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Opening : MonoBehaviour
{
    public NPCDialogue dialogueData;
    private Animator _animator;
    private DialogueController _dialogueUI;
    private int _dialogueIndex;
    private bool _isTyping, _isDialogueActive;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _dialogueUI = DialogueController.Instance;
        StartDialogue();
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (dialogueData == null || !_isDialogueActive) return;

            if (_isDialogueActive)
            {
                NextLine();
            }
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
        if (_dialogueIndex == 3) _animator.Play("Blackout");

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
        SceneManager.LoadScene("ExploreScene");
    }
}
