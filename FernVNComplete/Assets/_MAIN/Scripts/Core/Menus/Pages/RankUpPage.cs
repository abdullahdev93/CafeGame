using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using COMMANDS;

public class RankUpPage : MonoBehaviour
{
    public static RankUpPage instance { get; private set; } 

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI friendNameText;
    [SerializeField] private TextMeshProUGUI friendRankText;
    [SerializeField] private TextMeshProUGUI friendFlavorText;
    [SerializeField] private Image friendPicture;
    [SerializeField] private Image friendSilhouette;

    [Header("Character Visuals")]
    [SerializeField] private Sprite meiPicture;
    [SerializeField] private Sprite meiSilhouette;

    [SerializeField] private Sprite alexPicture;
    [SerializeField] private Sprite alexSilhouette;

    [SerializeField] private Sprite ninaPicture;
    [SerializeField] private Sprite ninaSilhouette;

    [SerializeField] private Sprite simonPicture;
    [SerializeField] private Sprite simonSilhouette;

    [Header("Rank Up Backgrounds")]
    //[SerializeField] private Image backgroundImage;
    [SerializeField] private RawImage backgroundImage;

    [SerializeField] private Texture2D meiBackground;
    [SerializeField] private Texture2D alexBackground;
    [SerializeField] private Texture2D ninaBackground;
    [SerializeField] private Texture2D simonBackground; 


    public Button OKButton;
    public GameObject rankUpPage;

    private CanvasGroup canvasGroup;
    private bool isFading = false;

    public void Awake()
    {
        instance = this; 
    }

    private Dictionary<string, string[]> flavorTextByCharacter = new Dictionary<string, string[]>
{
    {
        "Mei", new string[]
        {
            "\"Don't take this the wrong way… but you're not like most people I’ve met here.\"",
            "\"I guess it’s kinda nice having someone who gets the whole quiet corner vibe.\"",
            "\"You know, I used to read just to escape people. But with you… it’s different.\"",
            "\"You remembered my favorite tea. That’s… really sweet of you.\"",
            "\"Sometimes I feel like I'm just a ghost in this town. But when I'm with you, I feel real.\"",
            "\"I had a dream last night, and you were in it. We were just… laughing.\"",
            "\"You're one of the only people I don’t feel the need to hide around.\"",
            "\"I’m glad we met, even if I’d never say that out loud… wait, crap, I just did.\"",
            "\"You don’t just hear me. You *see* me. That scares me… but it also feels amazing.\"",
            "\"Whatever happens next… I’m not running anymore. Not from you.\""
        }
    },
    {
        "Alex", new string[]
        {
            "Alex: \"You’re lucky I let you see the ‘me’ behind the camera. Not many get that.\"",
            "Alex: \"Okay, don’t freak out—but I kinda look forward to our chats.\"",
            "Alex: \"You're actually kind of refreshing, y’know? No filters. Just real talk.\"",
            "Alex: \"It’s weird. I feel more like *myself* around you than when I’m performing.\"",
            "Alex: \"You’d tell me if I was being fake, right? I need that honesty in my life.\"",
            "Alex: \"I don't think I’d survive another con weekend without someone like you to vent to.\"",
            "Alex: \"Sometimes I wonder if people like me for me—or the persona. With you, I don’t have to guess.\"",
            "Alex: \"You helped me realize I don’t have to perform for affection.\"",
            "Alex: \"Thanks for showing me there’s more to me than just being someone’s fantasy.\"",
            "Alex: \"Hey, don’t ever disappear on me, okay? I don’t want to lose what we have.\""
        }
    },
    {
        "Nina", new string[]
        {
            "Nina: (to herself) \"Why do I keep noticing their eyes every time they look at me?\"",
            "Nina: \"If you're trying to charm me, you're going to have to do better than that. …Actually, don’t. That worked.\"",
            "Nina: \"You're persistent, I’ll give you that. No one else stuck around this long.\"",
            "Nina: (muttering) \"Why does my heart race when you compliment my art?\"",
            "Nina: \"Don’t flatter me. …Ugh, fine. Maybe I like it a little.\"",
            "Nina: (softly) \"Sometimes I wish I could pause time—just to stay in moments like this.\"",
            "Nina: \"I’m not used to people seeing past the paint and alcohol. But you… you do.\"",
            "Nina: \"I messed up a lot. But I don’t want to mess this up.\"",
            "Nina: \"You’re the only one who makes me feel like maybe I’m worth something again.\"",
            "Nina: (to herself) \"If they ever leave… I don’t know what I’d do.\""
        }
    },
    {
        "Simon", new string[]
        {
            "Simon: \"O-oh, h-hi. I… didn’t expect to see you today. But I’m g-glad you’re here.\"",
            "Simon: \"You didn’t laugh at my stutter. Most people… they do.\"",
            "Simon: \"I wrote a line in the script last night. It reminded me of you.\"",
            "Simon: \"You make me feel brave. Not all the time—but more than before.\"",
            "Simon: \"You… you stayed. Even when I got awkward. That means more than you know.\"",
            "Simon: \"When I’m with you, it’s like I don’t need to pretend I’m someone else.\"",
            "Simon: \"I used to be scared of sharing the real me. Now I’m scared of *not* sharing it with you.\"",
            "Simon: \"Every time we talk, I feel like I’m rewriting my story—and you’re in every chapter.\"",
            "Simon: \"If my life were a play… you’d be the one who saved the final act.\"",
            "Simon: \"No masks. No scripts. Just… me and you. And I wouldn’t have it any other way.\""
        }
    }
}; 

    private void Start()
    {
        canvasGroup = rankUpPage.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            Debug.LogError("CanvasGroup component is missing on RankUpPage GameObject.");
            return;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void ShowRankUp(FriendshipStat friendStat)
    {
        if (friendStat == null)
        {
            Debug.LogError("FriendStat is null in ShowRankUp.");
            return;
        }

        if (friendStat.HasPendingRankUp())
        {
            friendStat.ApplyPendingRankUp();
            StatsMenu.instance.UpdateFriendshipBars(); // Optional: if you want to update the Bond UI bar
            StatsMenu.instance.UpdateAllFriendshipRankTexts();
        } 

        string capitalizedName = friendStat.name.ToUpperInvariant();
        friendNameText.text = capitalizedName;
        friendRankText.text = friendStat.Level == 10 ? "MAX" : $"{friendStat.Level}"; 
        friendFlavorText.text = GetFlavorText(friendStat.name);

        SetFriendVisuals(friendStat.name); // <-- Set correct image/silhouette 

        StopAllCoroutines(); // Stop any fade-out still running
        rankUpPage.SetActive(true);
        StartCoroutine(FadeInRankUpPage()); 
    }

    public void ShowRankUp(string friendName)
    {
        FriendshipStat stat = FriendshipStats.instance.GetFriendshipStat(friendName);
        if (stat == null)
        {
            Debug.LogError($"No FriendshipStat found for {friendName}");
            return;
        }

        ShowRankUp(stat);
    } 

    private void SetFriendVisuals(string friendName)
    {
        switch (friendName)
        {
            case "Mei":
                friendPicture.sprite = meiPicture;
                friendSilhouette.sprite = meiSilhouette;
                backgroundImage.texture = meiBackground;
                break;
            case "Alex":
                friendPicture.sprite = alexPicture;
                friendSilhouette.sprite = alexSilhouette;
                backgroundImage.texture = alexBackground;
                break;
            case "Nina":
                friendPicture.sprite = ninaPicture;
                friendSilhouette.sprite = ninaSilhouette;
                backgroundImage.texture = ninaBackground;
                break;
            case "Simon":
                friendPicture.sprite = simonPicture;
                friendSilhouette.sprite = simonSilhouette;
                backgroundImage.texture = simonBackground;
                break;
            default:
                friendPicture.sprite = null;
                friendSilhouette.sprite = null;
                backgroundImage.texture = null;
                Debug.LogWarning($"No visuals assigned for: {friendName}");
                break;
        }

        friendPicture.gameObject.SetActive(true);
        friendSilhouette.gameObject.SetActive(true);
        backgroundImage.gameObject.SetActive(true);
    } 

    private string GetFlavorText(string friendName)
    {
        string key = friendName;
        if (!flavorTextByCharacter.ContainsKey(key))
            return $"I FEEL MY CONNECTION WITH {friendName.ToUpperInvariant()} HAS DEEPENED.";

        FriendshipStat stat = FriendshipStats.instance.GetFriendshipStat(friendName);
        int rank = Mathf.Clamp(stat.Level, 1, 10); // Rank 01 to Rank 10
        return flavorTextByCharacter[key][rank - 1];
    }

    public void ExitRankUpPage()
    {
        if (!isFading)
        {
            StartCoroutine(FadeOutRankUpPage());
        }

        var cmdGeneral = new CMD_DatabaseExtension_General();

        if (friendNameText.text == "Mei" || friendNameText.text == "MEI")
        {
            if (FriendshipStats.instance.FriendA.Level >= 1 && FriendshipStats.instance.FriendA.Level <= 10)
            {
                string fileName = $"MeiHangOut{FriendshipStats.instance.FriendA.Level}PARTTWO";
                cmdGeneral.LoadDialogueFile(fileName);
            }
        }

        if (friendNameText.text == "Alex" || friendNameText.text == "ALEX")
        {
            if (FriendshipStats.instance.FriendB.Level >= 1 && FriendshipStats.instance.FriendB.Level <= 10)
            {
                string fileName = $"AlexHangOut{FriendshipStats.instance.FriendB.Level}PARTTWO";
                cmdGeneral.LoadDialogueFile(fileName);
            }
        }

        if (friendNameText.text == "Nina" || friendNameText.text == "NINA")
        {
            if (FriendshipStats.instance.FriendC.Level >= 1 && FriendshipStats.instance.FriendC.Level <= 10)
            {
                string fileName = $"NinaHangOut{FriendshipStats.instance.FriendC.Level}PARTTWO";
                cmdGeneral.LoadDialogueFile(fileName);
            }
        }

        if (friendNameText.text == "Simon" || friendNameText.text == "SIMON")
        {
            if (FriendshipStats.instance.FriendD.Level >= 1 && FriendshipStats.instance.FriendD.Level <= 10)
            {
                string fileName = $"SimonHangOut{FriendshipStats.instance.FriendD.Level}PARTTWO";
                cmdGeneral.LoadDialogueFile(fileName);
            }
        }

        if (friendNameText.text == "Friend E" || friendNameText.text == "FRIEND E")
        {
            if (FriendshipStats.instance.FriendE.Level >= 1 && FriendshipStats.instance.FriendE.Level <= 10)
            {
                string fileName = $"FriendEHangOut{FriendshipStats.instance.FriendE.Level}PARTTWO";
                cmdGeneral.LoadDialogueFile(fileName);
            }
        }

        if (friendNameText.text == "Friend F" || friendNameText.text == "FRIEND F")
        {
            if (FriendshipStats.instance.FriendF.Level >= 1 && FriendshipStats.instance.FriendF.Level <= 10)
            {
                string fileName = $"FriendFHangOut{FriendshipStats.instance.FriendF.Level}PARTTWO";
                cmdGeneral.LoadDialogueFile(fileName);
            }
        }

        if (friendNameText.text == "Friend G" || friendNameText.text == "FRIEND G")
        {
            if (FriendshipStats.instance.FriendG.Level >= 1 && FriendshipStats.instance.FriendG.Level <= 10)
            {
                string fileName = $"FriendGHangOut{FriendshipStats.instance.FriendG.Level}PARTTWO";
                cmdGeneral.LoadDialogueFile(fileName);
            }
        }

        if (friendNameText.text == "Friend H" || friendNameText.text == "FRIEND H")
        {
            if (FriendshipStats.instance.FriendH.Level >= 1 && FriendshipStats.instance.FriendH.Level <= 10)
            {
                string fileName = $"FriendHHangOut{FriendshipStats.instance.FriendH.Level}PARTTWO";
                cmdGeneral.LoadDialogueFile(fileName);
            }
        }

        if (friendNameText.text == "Friend I" || friendNameText.text == "FRIEND I")
        {
            if (FriendshipStats.instance.FriendI.Level >= 1 && FriendshipStats.instance.FriendI.Level <= 10)
            {
                string fileName = $"FriendIHangOut{FriendshipStats.instance.FriendI.Level}PARTTWO";
                cmdGeneral.LoadDialogueFile(fileName);
            }
        }

        if (friendNameText.text == "Friend J" || friendNameText.text == "FRIEND J")
        {
            if (FriendshipStats.instance.FriendJ.Level >= 1 && FriendshipStats.instance.FriendJ.Level <= 10)
            {
                string fileName = $"FriendJHangOut{FriendshipStats.instance.FriendJ.Level}PARTTWO";
                cmdGeneral.LoadDialogueFile(fileName);
            }
        }
    }

    public IEnumerator FadeInRankUpPage()
    {
        isFading = true;

        float duration = 0.5f;
        float elapsed = 0f;
        canvasGroup.alpha = 0f;

        // Enable interactivity
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
        isFading = false;
    }

    public IEnumerator FadeOutRankUpPage()
    {
        isFading = true;

        float duration = 0.5f;
        float elapsed = 0f;

        // Disable interactivity immediately to avoid button spam
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        isFading = false;
    }
}
