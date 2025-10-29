using TMPro;
using UnityEngine;

public class CafeContinuePrompt : MonoBehaviour
{
    public static CafeContinuePrompt Instance;

    [SerializeField] private Animator anim;                  // your pulsing/arrow Animator
    [SerializeField] private TextMeshProUGUI tmpro;          // the Tutorial Text TMP
    [SerializeField] private bool continuePromptOnLastCharacter = true;

    private RectTransform root;

    private void Awake()
    {
        if (Instance == null) Instance = this; else { Destroy(gameObject); return; }
        root = GetComponent<RectTransform>();
        Hide();
    }

    /// <summary>Call once from TutorialManager after you know the TMP to follow.</summary>
    public void AttachTo(TextMeshProUGUI target)
    {
        tmpro = target;
        if (tmpro != null)
        {
            // parent under the text so localPosition works in the same space
            root.SetParent(tmpro.transform, false);
        }
    }

    public void Show()
    {
        if (tmpro == null) return;

        // Do not show if there is no text yet
        if (string.IsNullOrEmpty(tmpro.text))
        {
            Hide();
            return;
        }

        tmpro.ForceMeshUpdate();

        if (continuePromptOnLastCharacter && tmpro.textInfo.characterCount > 0)
        {
            var lastIndex = tmpro.textInfo.characterCount - 1;
            var ch = tmpro.textInfo.characterInfo[lastIndex];

            // If the last char is invisible (newline, etc.), try to find the last visible one
            for (int i = lastIndex; i >= 0 && !ch.isVisible; i--)
                ch = tmpro.textInfo.characterInfo[i];

            // Position just to the right of the last character’s bottom-right
            Vector3 targetPos = ch.bottomRight;
            float charWidth = ch.pointSize * 0.5f;
            targetPos = new Vector3(targetPos.x + charWidth, targetPos.y, 0);

            root.localPosition = targetPos;
        }

        anim.gameObject.SetActive(true);
    }

    public void Hide()
    {
        if (anim != null) anim.gameObject.SetActive(false);
    }
}
