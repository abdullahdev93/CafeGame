using DIALOGUE;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_General : CMD_DatabaseExtension
    {
        private static readonly string[] PARAM_SPEED = new string[] { "-s", "-spd" };
        private static readonly string[] PARAM_IMMEDIATE = new string[] { "-i", "-immediate" };
        private static readonly string[] PARAM_FILEPATH = new string[] { "-f", "-file", "-filepath" };
        private static readonly string[] PARAM_ENQUEUE = new string[] { "-e", "-enqueue" };
        private static readonly string[] PARAM_STAT = new string[] { "-st", "-stat" };
        private static readonly string[] PARAM_MONEY = new string[] { "-mo", "-money" };
        private static readonly string[] PARAM_MONEYUP = new string[] { "-mu", "-moneyup" };
        private static readonly string[] PARAM_MONEYDOWN = new string[] { "-md", "-moneydown" }; 
        private static readonly string[] PARAM_ESTAT = new string[] { "-est", "-estat" };
        private static readonly string[] PARAM_FRIEND = new string[] { "-fr", "-friend" };
        private static readonly string[] PARAM_LOVE = new string[] { "-lo", "-love" };
        private static readonly string[] PARAM_STATREQUIREMENT = new string[] { "-sr", "-statrequirement" };
        private static readonly string[] PARAM_PHONE = new string[] { "-ph", "-phone" };
        private static readonly string[] PARAM_UITip = new string[] { "-ut", "-uitip" };
        private static readonly string[] PARAM_SEASON = new string[] { "-se", "-season" };
        private static readonly string[] PARAM_WEEK = new string[] { "-wk", "-week" };
        private static readonly string[] PARAM_TEMP = new string[] { "-te", "-temperature" }; 
        private static readonly string[] PARAM_DAY = new string[] { "-dy", "-day" }; 
        private static readonly string[] PARAM_FCAL = new string[] { "-fc", "-fakecalendar" };
        private static readonly string[] PARAM_SKIP = new string[] { "-sk", "-skip" };
        private static readonly string[] PARAM_CUTSCENE = new string[] { "-cs", "-cutscene" };
        private static readonly string[] PARAM_CHARACTERNAME = new string[] { "-cn", "-charactername" };
        private static readonly string[] PARAM_RANKVALUE = new string[] { "-rv", "-rankvalue" };
        private static readonly string[] PARAM_MEET= new string[] { "-m", "-meet" };
        //private static readonly string[] PARAM_FKCH = new string[] { "-fh", "-fakehidden" }; 
        //private static readonly string[] PARAM_LIKE = new string[] { "-li", "-like" };
        //private static readonly string[] PARAM_STRAIGHT = new string[] { "-str", "-straight" };
        //private static readonly string[] PARAM_AWKWARD = new string[] { "-aw", "-awkward" }; 

        //private float waiting = LoadingScreenTips.instance.loadTimer; 

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("wait", new Func<string, IEnumerator>(Wait));

            //Dialogue System Controls
            database.AddCommand("showui", new Func<string[], IEnumerator>(ShowDialogueSystem));
            database.AddCommand("hideui", new Func<string[], IEnumerator>(HideDialogueSystem));
            
            //Dialogue Box Controls
            database.AddCommand("showdb", new Func<string[], IEnumerator>(ShowDialogueBox));
            database.AddCommand("hidedb", new Func<string[], IEnumerator>(HideDialogueBox));
            database.AddCommand("DisableInteractionBeforeCutscene", new Func<string, IEnumerator>(DisableInteractionBeforeCutscene));

            database.AddCommand("UIHide", new Action(UIHide));
            database.AddCommand("UIShow", new Action(UIShow)); 

            database.AddCommand("load", new Action<string[]>(LoadNewDialogueFile));

            database.AddCommand("loadscene", new Action<string>(LoadNewScene)); 

            database.AddCommand("loadingScreen", new Action(ShowLoadingScreen));

            database.AddCommand("CharacterResponse", new Action<string[]>(PlayCharacterResponse));

            database.AddCommand("StatRequirement", new Action<string[]>(PlayStatRequirement)); 

            database.AddCommand("AdvanceDay", new Action(AdvanceDay));

            database.AddCommand("FakeAdvanceDay", new Action<string[]>(FakeAdvanceDay));

            database.AddCommand("DisplayFakeDate", new Action(DisplayFakeCalendarDate)); 

            database.AddCommand("HideIconsIntroduction", new Action(HideIconsIntroduction));

            database.AddCommand("ShowAllIcons", new Action(ShowAllIcons)); 

            database.AddCommand("DisplayUITip", new Action<string[]>(DisplayUITip));
            database.AddCommand("HideUITip", new Action(HideUITip));
            database.AddCommand("FakeCalendarHide", new Action(FakeCalendarHidden)); 

            database.AddCommand("MeiHangOutTwoComplete", new Action(MeiHangOutTwoComplete));
            database.AddCommand("MeiHangOutThreeComplete", new Action(MeiHangOutThreeComplete));
            database.AddCommand("MeiHangOutFourComplete", new Action(MeiHangOutFourComplete));
            database.AddCommand("MeiHangOutFiveComplete", new Action(MeiHangOutFiveComplete));
            database.AddCommand("MeiHangOutSixComplete", new Action(MeiHangOutSixComplete));
            database.AddCommand("MeiHangOutSevenComplete", new Action(MeiHangOutSevenComplete));
            database.AddCommand("MeiHangOutEightComplete", new Action(MeiHangOutEightComplete));
            database.AddCommand("MeiHangOutNineComplete", new Action(MeiHangOutNineComplete));
            database.AddCommand("MeiHangOutTenComplete", new Action(MeiHangOutTenComplete));

            database.AddCommand("AlexHangOutTwoComplete", new Action(AlexHangOutTwoComplete));
            database.AddCommand("AlexHangOutThreeComplete", new Action(AlexHangOutThreeComplete));
            database.AddCommand("AlexHangOutFourComplete", new Action(AlexHangOutFourComplete));
            database.AddCommand("AlexHangOutFiveComplete", new Action(AlexHangOutFiveComplete));
            database.AddCommand("AlexHangOutSixComplete", new Action(AlexHangOutSixComplete));
            database.AddCommand("AlexHangOutSevenComplete", new Action(AlexHangOutSevenComplete));
            database.AddCommand("AlexHangOutEightComplete", new Action(AlexHangOutEightComplete));
            database.AddCommand("AlexHangOutNineComplete", new Action(AlexHangOutNineComplete));
            database.AddCommand("AlexHangOutTenComplete", new Action(AlexHangOutTenComplete));

            database.AddCommand("NinaHangOutTwoComplete", new Action(NinaHangOutTwoComplete));
            database.AddCommand("NinaHangOutThreeComplete", new Action(NinaHangOutThreeComplete));
            database.AddCommand("NinaHangOutFourComplete", new Action(NinaHangOutFourComplete));
            database.AddCommand("NinaHangOutFiveComplete", new Action(NinaHangOutFiveComplete));
            database.AddCommand("NinaHangOutSixComplete", new Action(NinaHangOutSixComplete));
            database.AddCommand("NinaHangOutSevenComplete", new Action(NinaHangOutSevenComplete));
            database.AddCommand("NinaHangOutEightComplete", new Action(NinaHangOutEightComplete));
            database.AddCommand("NinaHangOutNineComplete", new Action(NinaHangOutNineComplete));
            database.AddCommand("NinaHangOutTenComplete", new Action(NinaHangOutTenComplete));

            database.AddCommand("SimonHangOutTwoComplete", new Action(SimonHangOutTwoComplete));
            database.AddCommand("SimonHangOutThreeComplete", new Action(SimonHangOutThreeComplete));
            database.AddCommand("SimonHangOutFourComplete", new Action(SimonHangOutFourComplete));
            database.AddCommand("SimonHangOutFiveComplete", new Action(SimonHangOutFiveComplete));
            database.AddCommand("SimonHangOutSixComplete", new Action(SimonHangOutSixComplete));
            database.AddCommand("SimonHangOutSevenComplete", new Action(SimonHangOutSevenComplete));
            database.AddCommand("SimonHangOutEightComplete", new Action(SimonHangOutEightComplete));
            database.AddCommand("SimonHangOutNineComplete", new Action(SimonHangOutNineComplete));
            database.AddCommand("SimonHangOutTenComplete", new Action(SimonHangOutTenComplete)); 
            //database.AddCommand("LikedResponse", new Action<string[]>(PlayLikeResponse));
            //database.AddCommand("StraightResponse", new Action<string[]>(PlayStraightFaceResponse));
            //database.AddCommand("AwkwardResponse", new Action<string[]>(PlayAwkwardResponse));  

            // Add new commands for increasing stats 
            database.AddCommand("IncreaseCharisma", new Action<string[]>(IncreaseCharisma));
            database.AddCommand("IncreaseCreativity", new Action<string[]>(IncreaseCreativity));
            database.AddCommand("IncreaseEmpathy", new Action<string[]>(IncreaseEmpathy));
            database.AddCommand("IncreaseHumor", new Action<string[]>(IncreaseHumor));
            database.AddCommand("IncreaseEndurance", new Action<string[]>(IncreaseEndurance));
            database.AddCommand("IncreasePatience", new Action<string[]>(IncreasePatience));
            database.AddCommand("IncreaseIntelligence", new Action<string[]>(IncreaseIntelligence));
            database.AddCommand("IncreaseCourage", new Action<string[]>(IncreaseCourage));
            database.AddCommand("IncreaseKindness", new Action<string[]>(IncreaseKindness));
            database.AddCommand("IncreaseConfidence", new Action<string[]>(IncreaseConfidence));
            database.AddCommand("IncreaseKarma", new Action<string[]>(IncreaseKarma)); 
            database.AddCommand("DecreaseKarma", new Action<string[]>(DecreaseKarma));
            database.AddCommand("IncreaseAppreciationGrowth", new Action<string[]>(IncreaseAppreciationGrowth));
            database.AddCommand("IncreaseFriendshipA", new Action<string[]>(IncreaseFriendshipA));
            database.AddCommand("IncreaseFriendshipB", new Action<string[]>(IncreaseFriendshipB));
            database.AddCommand("IncreaseFriendshipC", new Action<string[]>(IncreaseFriendshipC));
            database.AddCommand("IncreaseFriendshipD", new Action<string[]>(IncreaseFriendshipD));
            database.AddCommand("IncreaseFriendshipE", new Action<string[]>(IncreaseFriendshipE));
            database.AddCommand("IncreaseFriendshipF", new Action<string[]>(IncreaseFriendshipF));
            database.AddCommand("IncreaseFriendshipG", new Action<string[]>(IncreaseFriendshipG));
            database.AddCommand("IncreaseFriendshipH", new Action<string[]>(IncreaseFriendshipH));
            database.AddCommand("IncreaseFriendshipI", new Action<string[]>(IncreaseFriendshipI));
            database.AddCommand("IncreaseFriendshipJ", new Action<string[]>(IncreaseFriendshipJ));
            database.AddCommand("IncreaseMoney", new Action<string[]>(IncreaseMoney));
            database.AddCommand("DecreaseMoney", new Action<string[]>(DecreaseMoney));
            database.AddCommand("MoneyUpAnim", new Action<string[]>(IncreaseMoneyAnim));
            database.AddCommand("MoneyDownAnim", new Action<string[]>(DecreaseMoneyAnim));
            database.AddCommand("PhoneEnabled", new Action<string[]>(EnablePhone));
            database.AddCommand("FakeCalendarInfo", new Action<string[]>(FakeCalendarInformation));
            database.AddCommand("DisableSkip", new Action(DisableSkipOption));
            database.AddCommand("EnableSkip", new Action(EnableSkipOption));
            database.AddCommand("ShowPhone", new Action(ShowPhoneUI)); 
            database.AddCommand("HidePhone", new Action(HidePhoneUI)); 
            database.AddCommand("ResetFile", new Action(ResetFile));
            database.AddCommand("ShowMC", new Action(ShowMC));
            database.AddCommand("HideMC", new Action(HideMC));
            database.AddCommand("RandomKarmaEvent", new Action(RandomKarmaEvent));
            database.AddCommand("ShowFriendInvites", new Action(ShowFriendInvites)); 
            database.AddCommand("AutoSave", new Action(AutoSaveFile));
            database.AddCommand("PlayCutscene", new Func<string[], IEnumerator>(PlayCutscene));
            database.AddCommand("ShowColorPickerName", new Action(ShowColorPickerName));
            //database.AddCommand("RankUpCharacter", new Action<string[]>(RankUpCharacter));
            database.AddCommand("DisplayRankUpPage", new Action<string[]>(DisplayRankUpPage)); 
            database.AddCommand("FirstTimeMeetingMei", new Action<string[]>(FirstTimeMeetingMei));
            database.AddCommand("FirstTimeMeetingAlex", new Action<string[]>(FirstTimeMeetingAlex));
            database.AddCommand("FirstTimeMeetingNina", new Action<string[]>(FirstTimeMeetingNina));
            database.AddCommand("FirstTimeMeetingSimon", new Action<string[]>(FirstTimeMeetingSimon)); 
            //database.AddCommand("FadeInGame", new Action(FadeInGame));
            //database.AddCommand("FadeOutGame", new Action(FadeOutGame)); 
            //database.AddCommand("ShowBarInvites", new Action(ShowBarInvites));
            /*database.AddCommand("IncreaseCharismaByOne", new Action(IncreaseCharismaByOne));
            database.AddCommand("IncreaseCharismaByTwo", new Action(IncreaseCharismaByTwo));
            database.AddCommand("IncreaseCharismaByThree", new Action(IncreaseCharismaByThree)); 
            database.AddCommand("IncreaseFriendAByOne", new Action(IncreaseFriendAByOne)); 
            database.AddCommand("IncreaseFriendAByTwo", new Action(IncreaseFriendAByTwo));
            database.AddCommand("IncreaseFriendAByThree", new Action(IncreaseFriendAByThree));8*/
            //database.AddCommand("DisplayCafeText", new Action(CafeText)); 

            //database.AddCommand("WaitingOnLoad", new Func<IEnumerator>(WaitingOnLoad)); 
        }

        private static void LoadNewDialogueFile(string[] data)
        { 
            string fileName = string.Empty;
            bool enqueue = false;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_FILEPATH, out fileName);
            parameters.TryGetValue(PARAM_ENQUEUE, out enqueue, defaultValue: false);

            string filePath = FilePaths.GetPathToResource(FilePaths.resources_dialogueFiles, fileName);
            TextAsset file = Resources.Load<TextAsset>(filePath);

            if (file == null)
            {
                Debug.LogWarning($"File '{filePath}' could not be loaded from dialogue files. Please ensure it exists within the '{FilePaths.resources_dialogueFiles}' resources folder.");
                return;
            }

            List<string> lines = FileManager.ReadTextAsset(file, includeBlankLines: true);
            Conversation newConversation = new Conversation(lines);

            if (enqueue)
                DialogueSystem.instance.conversationManager.Enqueue(newConversation);
            else
                DialogueSystem.instance.conversationManager.StartConversation(newConversation); 

            //DialogueSystem.instance.ShowLoading(); 
        }

        public void LoadDialogueFile(string fileName, bool enqueue = false)
        {
            string filePath = FilePaths.GetPathToResource(FilePaths.resources_dialogueFiles, fileName);
            TextAsset file = Resources.Load<TextAsset>(filePath);

            if (file == null)
            {
                Debug.LogWarning($"File '{filePath}' could not be loaded from dialogue files. Please ensure it exists within the '{FilePaths.resources_dialogueFiles}' resources folder.");
                return;
            }

            List<string> lines = FileManager.ReadTextAsset(file, includeBlankLines: true);
            Conversation newConversation = new Conversation(lines);

            if (enqueue)
                DialogueSystem.instance.conversationManager.Enqueue(newConversation);
            else
                DialogueSystem.instance.conversationManager.StartConversation(newConversation);
        } 

        private static IEnumerator Wait(string data)
        {
            if (float.TryParse(data, out float time))
            {
                yield return new WaitForSeconds(time);
            }
        }

        //private static IEnumerator WaitingOnLoad()
        //{
            //float waiting = LoadingScreenTips.instance.loadTimer;
            //yield return new WaitForSeconds(waiting); 
        //}

        private static IEnumerator ShowDialogueBox(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            yield return DialogueSystem.instance.dialogueContainer.Show(speed, immediate);
        }

        private static IEnumerator HideDialogueBox(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            yield return DialogueSystem.instance.dialogueContainer.Hide(speed, immediate);
        }

        private static IEnumerator ShowDialogueSystem(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            yield return DialogueSystem.instance.Show(speed, immediate);
        }

        private static IEnumerator HideDialogueSystem(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            yield return DialogueSystem.instance.Hide(speed, immediate);
        }

        private static void UIHide()
        {
            // Start fading out all UI elements
            CalendarSystem.Instance.StartCoroutine(FadeOutUIElement(CalendarSystem.Instance.seasonDayText.gameObject, 1f));
            CalendarSystem.Instance.StartCoroutine(FadeOutUIElement(CalendarSystem.Instance.timeOfDayText.gameObject, 1f));
            VNMenuManager.instance.StartCoroutine(FadeOutUIElement(VNMenuManager.instance.earningsText.gameObject, 1f));
            VNMenuManager.instance.StartCoroutine(FadeOutUIElement(VNMenuManager.instance.weatherImage.gameObject, 1f));
            CalendarSystem.Instance.StartCoroutine(FadeOutUIElement(CalendarSystem.Instance.calendarTemplate.gameObject, 1f)); 
            ChoicePanel.instance.StartCoroutine(FadeOutUIElement(ChoicePanel.instance.theSkipButton.gameObject, 1f));
            ChoicePanel.instance.StartCoroutine(FadeOutUIElement(ChoicePanel.instance.mockSkipButton.gameObject, 1f));
            VNMenuManager.instance.StartCoroutine(FadeOutUIElement(VNMenuManager.instance.PhoneIcon.gameObject, 1f));
            VNMenuManager.instance.StartCoroutine(FadeOutUIElement(VNMenuManager.instance.autoButton.gameObject, 1f));
            VNMenuManager.instance.StartCoroutine(FadeOutUIElement(VNMenuManager.instance.logButton.gameObject, 1f));
            VNMenuManager.instance.StartCoroutine(FadeOutUIElement(VNMenuManager.instance.speakerIcon.gameObject, 1f));
            VNMenuManager.instance.StartCoroutine(FadeOutUIElement(VNMenuManager.instance.earningsText.gameObject, 1f));
            DirectMessageManager.Instance.StartCoroutine(FadeOutUIElement(DirectMessageManager.Instance.notificationsCounterText.gameObject, 1f));
        }

        private static IEnumerator FadeOutUIElement(GameObject uiElement, float duration)
        {
            CanvasGroup canvasGroup = uiElement.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                // Add CanvasGroup if it doesn't exist
                canvasGroup = uiElement.AddComponent<CanvasGroup>();
            }

            float startAlpha = canvasGroup.alpha;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, elapsedTime / duration);
                yield return null;
            }

            canvasGroup.alpha = 0;
            uiElement.SetActive(false);  // Hide the element after fading out
        }

        private static void UIShow()
        {
            // Start fading in all UI elements
            CalendarSystem.Instance.StartCoroutine(FadeInUIElement(CalendarSystem.Instance.seasonDayText.gameObject, 1f));
            CalendarSystem.Instance.StartCoroutine(FadeInUIElement(CalendarSystem.Instance.timeOfDayText.gameObject, 1f));
            VNMenuManager.instance.StartCoroutine(FadeInUIElement(VNMenuManager.instance.earningsText.gameObject, 1f)); 
            VNMenuManager.instance.StartCoroutine(FadeInUIElement(VNMenuManager.instance.weatherImage.gameObject, 1f));
            CalendarSystem.Instance.StartCoroutine(FadeInUIElement(CalendarSystem.Instance.calendarTemplate.gameObject, 1f)); 
            ChoicePanel.instance.StartCoroutine(FadeInUIElement(ChoicePanel.instance.theSkipButton.gameObject, 1f));
            ChoicePanel.instance.StartCoroutine(FadeInUIElement(ChoicePanel.instance.mockSkipButton.gameObject, 1f));
            VNMenuManager.instance.StartCoroutine(FadeInUIElement(VNMenuManager.instance.PhoneIcon.gameObject, 1f));
            VNMenuManager.instance.StartCoroutine(FadeInUIElement(VNMenuManager.instance.autoButton.gameObject, 1f));
            VNMenuManager.instance.StartCoroutine(FadeInUIElement(VNMenuManager.instance.logButton.gameObject, 1f));
            VNMenuManager.instance.StartCoroutine(FadeInUIElement(VNMenuManager.instance.speakerIcon.gameObject, 1f));
            VNMenuManager.instance.StartCoroutine(FadeInUIElement(VNMenuManager.instance.earningsText.gameObject, 1f));
            DirectMessageManager.Instance.StartCoroutine(FadeInUIElement(DirectMessageManager.Instance.notificationsCounterText.gameObject, 1f));  
        }

        private static IEnumerator FadeInUIElement(GameObject uiElement, float duration)
        {
            // Ensure the UI element is active before fading in
            uiElement.SetActive(true);

            CanvasGroup canvasGroup = uiElement.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                // Add CanvasGroup if it doesn't exist
                canvasGroup = uiElement.AddComponent<CanvasGroup>();
            }

            float startAlpha = canvasGroup.alpha;
            float elapsedTime = 0f;

            // Start from alpha 0 if it's not already 0
            canvasGroup.alpha = 0;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);  // Fade in from 0 to 1
                yield return null;
            }

            canvasGroup.alpha = 1;  // Ensure alpha is fully 1 after fading in
        }


        private static void AdvanceDay()
        {
            CalendarSystem.Instance.AdvanceActivityOrDay(); 
        }

        private static void FakeAdvanceDay(string[] data)
        {
            float moveDistance;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_FCAL, out moveDistance); 

            FakeCalendarSlide.Instance.ShowPanelAndMove(moveDistance); 
        } 
        
        private static void DisplayFakeCalendarDate()
        {
            FakeCalendarSlide.Instance.DisplayFakeDate(); 
        }

        private static void HideIconsIntroduction()
        {
            CalendarSystem.Instance.calendarTemplate.SetActive(false);
            CalendarSystem.Instance.calendarMockTemplate.SetActive(true); 
            VNMenuManager.instance.PhoneIcon.gameObject.SetActive(false);
            VNMenuManager.instance.earningsText.gameObject.SetActive(false);
            DirectMessageManager.Instance.notificationsCounterText.gameObject.SetActive(false); 
        }

        private static void ShowAllIcons() 
        {
            CalendarSystem.Instance.calendarTemplate.SetActive(true);
            CalendarSystem.Instance.calendarMockTemplate.SetActive(false); 
            VNMenuManager.instance.PhoneIcon.gameObject.SetActive(true);
            VNMenuManager.instance.earningsText.gameObject.SetActive(true); 
        } 

        private static void FakeCalendarInformation(string[] data) 
        {
            string fakeSeasonDay;
            string fakeTimeOfDay;
            string fakeWeekDay;
            string fakeTemperature; 

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_SEASON, out fakeSeasonDay);
            parameters.TryGetValue(PARAM_DAY, out fakeTimeOfDay); 
            parameters.TryGetValue(PARAM_WEEK, out fakeWeekDay);
            parameters.TryGetValue(PARAM_TEMP, out fakeTemperature); 

            FakeCalendarSlide.Instance.FakeCalendarInfo(fakeSeasonDay, fakeTimeOfDay, fakeWeekDay, fakeTemperature); 

        } 
        
        private static void FakeCalendarHidden()
        {
            VNMenuManager.instance.fakeCalendarPanel.SetActive(false);   
        }

        private static IEnumerator DisableInteractionBeforeCutscene(string data)
        {
            if (float.TryParse(data, out float time))
            {
                DialogueSystem.instance.continueButton.interactable = false;
                VNMenuManager.instance.PhoneIcon.interactable = false;
                VNMenuManager.instance.autoButton.interactable = false;
                VNMenuManager.instance.logButton.interactable = false;
                yield return new WaitForSeconds(time); 
                DialogueSystem.instance.continueButton.interactable = true;
                VNMenuManager.instance.PhoneIcon.interactable = true;
                VNMenuManager.instance.autoButton.interactable = true;
                VNMenuManager.instance.logButton.interactable = true; 
            } 
        } 



        private static void DisableSkipOption() 
        {
            ChoicePanel.instance.skipDisabled = true;

            //AutoReader.reader.Disable();

            //ChoicePanel.instance.mockSkipButton.interactable = false;
            //ChoicePanel.instance.theSkipButton.interactable = false;

            //mockSkipButton.interactable = false;

            //ChoicePanel.instance.theSkipButton.gameObject.SetActive(false);
            //ChoicePanel.instance.mockSkipButton.gameObject.SetActive(true);

            //skipReader.Disable();

            //ChoicePanel.instance.fastForwardEffect.SetActive(false); 
        } 

        private static void EnableSkipOption()
        {
            ChoicePanel.instance.skipDisabled = false;

            ChoicePanel.instance.theSkipButton.GetComponent<CanvasGroup>().interactable = true;  

            ChoicePanel.instance.StartCoroutine(FadeInUIElement(ChoicePanel.instance.theSkipButton.gameObject, 1f));
            ChoicePanel.instance.StartCoroutine(FadeInUIElement(ChoicePanel.instance.mockSkipButton.gameObject, 1f)); 

            //AutoReader.reader.Disable();

            //ChoicePanel.instance.mockSkipButton.interactable = true;
            //ChoicePanel.instance.theSkipButton.interactable = true;

            //mockSkipButton.interactable = false;

            //ChoicePanel.instance.theSkipButton.gameObject.SetActive(true);
            //ChoicePanel.instance.mockSkipButton.gameObject.SetActive(false);

            //ChoicePanel.instance.StartCoroutine(FadeInUIElement(ChoicePanel.instance.theSkipButton.gameObject, 1f));
            //ChoicePanel.instance.StartCoroutine(FadeInUIElement(ChoicePanel.instance.mockSkipButton.gameObject, 1f)); 

            //skipReader.Disable();

            ChoicePanel.instance.fastForwardEffect.SetActive(false);
        } 

        private static void ShowPhoneUI()
        {
            VNMenuManager.instance.StartCoroutine(FadeInUIElement(VNMenuManager.instance.PhoneIcon.gameObject, 1f));

        }

        private static void HidePhoneUI()
        {
            VNMenuManager.instance.StartCoroutine(FadeOutUIElement(VNMenuManager.instance.PhoneIcon.gameObject, 1f));

        } 

        private static void ResetFile()
        {
            PlayerPrefs.DeleteKey("StartingFile"); 
            PlayerPrefs.Save();
        } 

        private static void ShowMC()
        {
            VNMenuManager.instance.PlayMCAnimation(); 
        } 

        private static void HideMC()
        {
            VNMenuManager.instance.HideMC(); 
        }

        /*// ... [previous content above remains unchanged]

        private static void RandomKarmaEvent()
        {
            string previousFile = PlayerPrefs.GetString("StartingFile", "");
            Debug.Log($"Previous File Loaded: {previousFile}");

            string currentLocation = "Unknown";

            //Debug.Log($"Current Location Loaded: {currentLocation}"); 

            if (previousFile.Contains("ParkFile"))
                currentLocation = "Park";
            else if (previousFile.Contains("BarFile"))
                currentLocation = "Bar";
            else if (previousFile.Contains("ConvienceStoreFile")) 
                currentLocation = "ConvienceStore";
            else if (previousFile.Contains("LibraryFile"))
                currentLocation = "Library";

            List<string> possibleEvents = new List<string>();

            switch (currentLocation)
            {
                case "ConvienceStore": 
                    possibleEvents.Add("ConvienceStoreRandomKarmaEvent");
                    possibleEvents.Add("ConvienceStoreRandomKarmaEventTwo");
                    possibleEvents.Add("ConvienceStoreRandomKarmaEventThree"); 
                    break;
                case "Park":
                    possibleEvents.Add("ParkRandomKarmaEvent");
                    possibleEvents.Add("ParkRandomKarmaEventTwo");
                    possibleEvents.Add("ParkRandomKarmaEventThree");
                    break;
                case "BarF":
                    possibleEvents.Add("BarRandomKarmaEvent");
                    possibleEvents.Add("BarRandomKarmaEventTwo");
                    break;
                case "Library":
                    possibleEvents.Add("LibraryRandomKarmaEvent");
                    possibleEvents.Add("LibraryRandomKarmaEventTwo");
                    break;
                default:
                    Debug.Log("No karma events defined for current location file: " + previousFile);
                    break;
            }

            Debug.Log($"Current Location: {currentLocation}");
            Debug.Log($"Total Karma Events Available: {possibleEvents.Count}");
            foreach (var e in possibleEvents)
            {
                Debug.Log($"Possible Event: {e}");
            }

            int roll = UnityEngine.Random.Range(1, 11);
            Debug.Log($"Random Karma Event Roll: {roll}");

            if (roll == 1 || roll == 5 || roll == 10)
            {
                if (possibleEvents.Count > 0)
                {
                    string selectedEvent = possibleEvents[UnityEngine.Random.Range(0, possibleEvents.Count)];
                    PlayerPrefs.SetString("StartingFile", selectedEvent);
                    PlayerPrefs.Save();
                    Debug.Log($"Karma Encounter triggered: {selectedEvent}");
                    SceneManager.LoadScene("VisualNovel");
                }
                else
                {
                    Debug.Log("No events to trigger. Returning to TownMap.");
                    SceneManager.LoadScene("TownMap");
                }
            }
            else
            {
                Debug.Log("No Karma Encounter this time.");
                SceneManager.LoadScene("TownMap");
            }
        }

        // ... [rest of the file continues unchanged] */

        // ... [previous content above remains unchanged]

        private static void RandomKarmaEvent()
        {
            string previousFile = PlayerPrefs.GetString("StartingFile", "");
            Debug.Log($"Previous File Loaded: {previousFile}");

            string currentLocation = previousFile;

            CalendarSystem.Weather currentWeather = CalendarSystem.Instance.GetCurrentWeather();
            string currentActivityPhase = CalendarSystem.Instance.activityPhases[CalendarSystem.Instance.activityCounter]; 

            Debug.Log($"Current Location Loaded: {currentLocation}");

            Debug.Log($"Current Weather Loaded: {currentWeather}");

            Debug.Log($"Current Activity Phase: {currentActivityPhase}"); 

            List<string> possibleEvents = new List<string>();

            switch (currentLocation)
            {
                case "ConvienceStoreFile": 
                    if (currentWeather == CalendarSystem.Weather.Sunny)
                    {
                        if (currentActivityPhase == "Morning" || currentActivityPhase == "Afternoon" || currentActivityPhase == "Evening")
                        {
                            possibleEvents.Add("ConvienceStoreRandomKarmaEvent");
                            possibleEvents.Add("ConvienceStoreRandomKarmaEventTwo");
                            possibleEvents.Add("ConvienceStoreRandomKarmaEventThree"); 
                        } 
                    } 
                    break;
                case "ParkFile":
                    if (currentWeather == CalendarSystem.Weather.Sunny)
                    {
                        if (currentActivityPhase == "Morning" || currentActivityPhase == "Afternoon" || currentActivityPhase == "Evening")
                        {
                            possibleEvents.Add("ParkRandomKarmaEvent");
                            possibleEvents.Add("ParkRandomKarmaEventTwo");
                            possibleEvents.Add("ParkRandomKarmaEventThree");
                        } 
                    } 
                    break;
                case "BarFile":
                    if (currentWeather == CalendarSystem.Weather.Sunny)
                    {
                        if (currentActivityPhase == "Morning" || currentActivityPhase == "Afternoon" || currentActivityPhase == "Evening")
                        {
                            possibleEvents.Add("BarRandomKarmaEvent");
                            possibleEvents.Add("BarRandomKarmaEventTwo");
                        } 
                    }
                    break;
                case "LibraryFile":
                    if (currentWeather == CalendarSystem.Weather.Sunny)
                    {
                        if (currentActivityPhase == "Morning" || currentActivityPhase == "Afternoon" || currentActivityPhase == "Evening")
                        {
                            possibleEvents.Add("LibraryRandomKarmaEvent");
                            possibleEvents.Add("LibraryRandomKarmaEventTwo"); 
                        } 
                    }
                    break;
                default:
                    Debug.Log("No karma events defined for current location file: " + previousFile);
                    break;
            }

            Debug.Log($"Current Location: {currentLocation}");
            Debug.Log($"Total Karma Events Available: {possibleEvents.Count}");
            foreach (var e in possibleEvents)
            {
                Debug.Log($"Possible Event: {e}");
            }

            int roll = UnityEngine.Random.Range(1, 11);
            Debug.Log($"Random Karma Event Roll: {roll}");

            if (roll == 1 || roll == 5 || roll == 10)
            {
                if (possibleEvents.Count > 0)
                {
                    string selectedEvent = possibleEvents[UnityEngine.Random.Range(0, possibleEvents.Count)];
                    PlayerPrefs.SetString("StartingFile", selectedEvent);
                    PlayerPrefs.Save();
                    Debug.Log($"Karma Encounter triggered: {selectedEvent}");
                    SceneManager.LoadScene("VisualNovel");
                }
                else
                {
                    Debug.Log("No events to trigger. Returning to TownMap.");
                    SceneManager.LoadScene("TownMap");
                }
            }
            else
            {
                Debug.Log("No Karma Encounter this time.");
                SceneManager.LoadScene("TownMap");
            }
        }

        // ... [rest of the file continues unchanged] 

        private static void ShowFriendInvites()
        {
            ChoicePanel.instance.ShowInviteMenu(); 
        } 

        private static void AutoSaveFile()
        {
            AutoSaveManager.instance.TriggerAutoSave(); 
        }

        /*private static void PlayCutscene(string[] data)
        {
            string cutsceneName;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_CUTSCENE, out cutsceneName);

            
            CutsceneController.Instance.PlayCutscene(cutsceneName); 
        }*/

        private static IEnumerator PlayCutscene(string[] data) 
        { 
            string cutsceneName; 

            var parameters = ConvertDataToParameters(data);

            //PlayCutscene(-cs "Fantasy Landscape") 
            //wait(67.0)

            parameters.TryGetValue(PARAM_CUTSCENE, out cutsceneName);

            yield return new WaitForSeconds(1f);

            yield return CutsceneController.Instance.PlayCutscene(cutsceneName); 
        } 

        private static void ShowColorPickerName()
        { 
            VNMenuManager.instance.colorPicker.gameObject.SetActive(true);      
        } 

        private static void FirstTimeMeetingMei(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            bool firstMeeting;

            parameters.TryGetValue(PARAM_MEET, out firstMeeting, defaultValue: false);

            if (firstMeeting)
                VNMenuManager.instance.meiFirstTime = true;
            else
                VNMenuManager.instance.meiFirstTime = false; 
        }

        private static void FirstTimeMeetingAlex(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            bool firstMeeting;

            parameters.TryGetValue(PARAM_MEET, out firstMeeting, defaultValue: false);

            if (firstMeeting)
                VNMenuManager.instance.alexFirstTime = true;
            else
                VNMenuManager.instance.alexFirstTime = false;
        }

        private static void FirstTimeMeetingNina(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            bool firstMeeting;

            parameters.TryGetValue(PARAM_MEET, out firstMeeting, defaultValue: false);

            if (firstMeeting)
                VNMenuManager.instance.ninaFirstTime = true;
            else
                VNMenuManager.instance.ninaFirstTime = false;
        }

        private static void FirstTimeMeetingSimon(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            bool firstMeeting;

            parameters.TryGetValue(PARAM_MEET, out firstMeeting, defaultValue: false);

            if (firstMeeting)
                VNMenuManager.instance.simonFirstTime = true; 
            else
                VNMenuManager.instance.simonFirstTime = false;
        }

        //private static void FadeInGame()
        //{
        //VNMenuManager.instance.FadeInPanel(); 
        //}

        //private static void ShowBarInvites()
        //{
        //ChoicePanel.instance.ShowBarInvitesMenu(); 
        //}

        private static void MeiHangOutTwoComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedMeiHangOutTwo = true;
            PlayerPrefs.SetInt("AlreadyAcceptedMeiHangOutTwo", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void MeiHangOutThreeComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedMeiHangOutThree = true; 
            PlayerPrefs.SetInt("AlreadyAcceptedMeiHangOutThree", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void MeiHangOutFourComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedMeiHangOutFour = true; 
            PlayerPrefs.SetInt("AlreadyAcceptedMeiHangOutFour", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void MeiHangOutFiveComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedMeiHangOutFive = true;
            PlayerPrefs.SetInt("AlreadyAcceptedMeiHangOutFive", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void MeiHangOutSixComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedMeiHangOutSix = true;
            PlayerPrefs.SetInt("AlreadyAcceptedMeiHangOutSix", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void MeiHangOutSevenComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedMeiHangOutSeven = true;
            PlayerPrefs.SetInt("AlreadyAcceptedMeiHangOutSeven", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void MeiHangOutEightComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedMeiHangOutEight = true;
            PlayerPrefs.SetInt("AlreadyAcceptedMeiHangOutEight", 1); // 1 represents true
            PlayerPrefs.Save();
        }
        
        private static void MeiHangOutNineComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedMeiHangOutNine = true;
            PlayerPrefs.SetInt("AlreadyAcceptedMeiHangOutNine", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void MeiHangOutTenComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedMeiHangOutTen = true;  
            PlayerPrefs.SetInt("AlreadyAcceptedMeiHangOutTen", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void AlexHangOutTwoComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedAlexHangOutTwo = true;
            PlayerPrefs.SetInt("AlreadyAcceptedAlexHangOutTwo", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void AlexHangOutThreeComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedAlexHangOutThree = true;
            PlayerPrefs.SetInt("AlreadyAcceptedAlexHangOutThree", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void AlexHangOutFourComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedAlexHangOutFour = true;
            PlayerPrefs.SetInt("AlreadyAcceptedAlexHangOutFour", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void AlexHangOutFiveComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedAlexHangOutFive = true;
            PlayerPrefs.SetInt("AlreadyAcceptedAlexHangOutFive", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void AlexHangOutSixComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedAlexHangOutSix = true;
            PlayerPrefs.SetInt("AlreadyAcceptedAlexHangOutSix", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void AlexHangOutSevenComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedAlexHangOutSeven = true;
            PlayerPrefs.SetInt("AlreadyAcceptedAlexHangOutSeven", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void AlexHangOutEightComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedAlexHangOutEight = true;
            PlayerPrefs.SetInt("AlreadyAcceptedAlexHangOutEight", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void AlexHangOutNineComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedAlexHangOutNine = true;
            PlayerPrefs.SetInt("AlreadyAcceptedAlexHangOutNine", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void AlexHangOutTenComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedAlexHangOutTen = true;
            PlayerPrefs.SetInt("AlreadyAcceptedAlexHangOutTen", 1); // 1 represents true 
            PlayerPrefs.Save();
        }

        private static void NinaHangOutTwoComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedNinaHangOutTwo = true;
            PlayerPrefs.SetInt("AlreadyAcceptedNinaHangOutTwo", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void NinaHangOutThreeComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedNinaHangOutThree = true;
            PlayerPrefs.SetInt("AlreadyAcceptedNinaHangOutThree", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void NinaHangOutFourComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedNinaHangOutFour = true;
            PlayerPrefs.SetInt("AlreadyAcceptedNinaHangOutFour", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void NinaHangOutFiveComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedNinaHangOutFive = true;
            PlayerPrefs.SetInt("AlreadyAcceptedNinaHangOutFive", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void NinaHangOutSixComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedNinaHangOutSix = true;
            PlayerPrefs.SetInt("AlreadyAcceptedNinaHangOutSix", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void NinaHangOutSevenComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedNinaHangOutSeven = true;
            PlayerPrefs.SetInt("AlreadyAcceptedNinaHangOutSeven", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void NinaHangOutEightComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedNinaHangOutEight = true;
            PlayerPrefs.SetInt("AlreadyAcceptedNinaHangOutEight", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void NinaHangOutNineComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedNinaHangOutNine = true;
            PlayerPrefs.SetInt("AlreadyAcceptedNinaHangOutNine", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void NinaHangOutTenComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedNinaHangOutTen = true;
            PlayerPrefs.SetInt("AlreadyAcceptedNinaHangOutTen", 1); // 1 represents true 
            PlayerPrefs.Save();
        }

        private static void SimonHangOutTwoComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedSimonHangOutTwo = true;
            PlayerPrefs.SetInt("AlreadyAcceptedSimonHangOutTwo", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void SimonHangOutThreeComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedSimonHangOutThree = true;
            PlayerPrefs.SetInt("AlreadyAcceptedSimonHangOutThree", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void SimonHangOutFourComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedSimonHangOutFour = true;
            PlayerPrefs.SetInt("AlreadyAcceptedSimonHangOutFour", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void SimonHangOutFiveComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedSimonHangOutFive = true;
            PlayerPrefs.SetInt("AlreadyAcceptedSimonHangOutFive", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void SimonHangOutSixComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedSimonHangOutSix = true;
            PlayerPrefs.SetInt("AlreadyAcceptedSimonHangOutSix", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void SimonHangOutSevenComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedSimonHangOutSeven = true;
            PlayerPrefs.SetInt("AlreadyAcceptedSimonHangOutSeven", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void SimonHangOutEightComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedSimonHangOutEight = true;
            PlayerPrefs.SetInt("AlreadyAcceptedSimonHangOutEight", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void SimonHangOutNineComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedSimonHangOutNine = true;
            PlayerPrefs.SetInt("AlreadyAcceptedSimonHangOutNine", 1); // 1 represents true
            PlayerPrefs.Save();
        }

        private static void SimonHangOutTenComplete()
        {
            DirectMessageManager.Instance.alreadyAcceptedSimonHangOutTen = true;
            PlayerPrefs.SetInt("AlreadyAcceptedSimonHangOutTen", 1); // 1 represents true  
            PlayerPrefs.Save();
        }

        private static void PlayCharacterResponse(string[] data)
        { 
            var parameters = ConvertDataToParameters(data);

            string animation; 

            parameters.TryGetValue(PARAM_LOVE, out animation);

            VNMenuManager.instance.PlayCharacterResponse(animation); 
        } 

        private static void PlayStatRequirement(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            string statAnimation;

            parameters.TryGetValue(PARAM_STATREQUIREMENT, out statAnimation);

            VNMenuManager.instance.PlayStatRequirementMetAnimation(statAnimation); 
        }

        private static void IncreaseCharisma(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            int charismaValue;

            parameters.TryGetValue(PARAM_STAT, out charismaValue);  

            //StatsMenu.instance.CharismaLevelByOne();
            // Assuming Charisma is a PlayerStat managed by PlayerStats.instance
            PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Charisma, charismaValue); 
            StatsMenu.instance.UpdateRankText(PlayerStats.instance.Charisma, StatsMenu.instance.charismaRankText); 

            Debug.Log($"Charisma Points Increased by: {charismaValue}");  
        }

        private static void IncreaseCreativity(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            int creativityValue;

            parameters.TryGetValue(PARAM_STAT, out creativityValue);

            //StatsMenu.instance.CharismaLevelByOne();
            // Assuming Charisma is a PlayerStat managed by PlayerStats.instance
            PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Creativity, creativityValue);
            StatsMenu.instance.UpdateRankText(PlayerStats.instance.Creativity, StatsMenu.instance.creativityRankText); 

            Debug.Log($"Creativity Points Increased by: {creativityValue}");
        }

        private static void IncreaseEmpathy(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            int empathyValue;

            parameters.TryGetValue(PARAM_STAT, out empathyValue);

            //StatsMenu.instance.CharismaLevelByOne();
            // Assuming Charisma is a PlayerStat managed by PlayerStats.instance
            PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Empathy, empathyValue);
            StatsMenu.instance.UpdateRankText(PlayerStats.instance.Empathy, StatsMenu.instance.empathyRankText);

            Debug.Log($"Empathy Points Increased by: {empathyValue}");
        }

        private static void IncreaseHumor(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            int humorValue;

            parameters.TryGetValue(PARAM_STAT, out humorValue);

            //StatsMenu.instance.CharismaLevelByOne();
            // Assuming Charisma is a PlayerStat managed by PlayerStats.instance
            PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Humor, humorValue);
            StatsMenu.instance.UpdateRankText(PlayerStats.instance.Humor, StatsMenu.instance.humorRankText);

            Debug.Log($"Humor Points Increased by: {humorValue}");
        }

        private static void IncreaseEndurance(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            int enduranceValue;

            parameters.TryGetValue(PARAM_STAT, out enduranceValue); 

            //StatsMenu.instance.CharismaLevelByOne();
            // Assuming Charisma is a PlayerStat managed by PlayerStats.instance
            PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Endurance, enduranceValue);
            StatsMenu.instance.UpdateRankText(PlayerStats.instance.Endurance, StatsMenu.instance.enduranceRankText);  

            Debug.Log($"Endurance Points Increased by: {enduranceValue}");
        }

        private static void IncreasePatience(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            int patienceValue;

            parameters.TryGetValue(PARAM_STAT, out patienceValue);

            //StatsMenu.instance.CharismaLevelByOne();
            // Assuming Charisma is a PlayerStat managed by PlayerStats.instance
            PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Patience, patienceValue);
            StatsMenu.instance.UpdateRankText(PlayerStats.instance.Patience, StatsMenu.instance.patienceRankText);

            Debug.Log($"Patience Points Increased by: {patienceValue}");
        }

        private static void IncreaseIntelligence(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            int intelligenceValue;

            parameters.TryGetValue(PARAM_STAT, out intelligenceValue);

            //StatsMenu.instance.CharismaLevelByOne();
            // Assuming Charisma is a PlayerStat managed by PlayerStats.instance
            PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Intelligence, intelligenceValue);
            StatsMenu.instance.UpdateRankText(PlayerStats.instance.Intelligence, StatsMenu.instance.intelligenceRankText);

            Debug.Log($"Intelligence Points Increased by: {intelligenceValue}");
        }

        private static void IncreaseCourage(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            int courageValue;

            parameters.TryGetValue(PARAM_STAT, out courageValue);

            //StatsMenu.instance.CharismaLevelByOne();
            // Assuming Charisma is a PlayerStat managed by PlayerStats.instance
            PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Courage, courageValue);
            StatsMenu.instance.UpdateRankText(PlayerStats.instance.Courage, StatsMenu.instance.courageRankText);

            Debug.Log($"Loyalty Points Increased by: {courageValue}");
        }

        private static void IncreaseKindness(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            int kindnessValue;

            parameters.TryGetValue(PARAM_STAT, out kindnessValue);

            //StatsMenu.instance.CharismaLevelByOne();
            // Assuming Charisma is a PlayerStat managed by PlayerStats.instance
            PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Kindness, kindnessValue);
            StatsMenu.instance.UpdateRankText(PlayerStats.instance.Kindness, StatsMenu.instance.kindnessRankText);

            Debug.Log($"Kindness Points Increased by: {kindnessValue}");
        }

        private static void IncreaseConfidence(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            int confidenceValue;

            parameters.TryGetValue(PARAM_STAT, out confidenceValue);

            //StatsMenu.instance.CharismaLevelByOne();
            // Assuming Charisma is a PlayerStat managed by PlayerStats.instance
            PlayerStats.instance.IncreaseStatPoints(PlayerStats.instance.Confidence, confidenceValue);
            StatsMenu.instance.UpdateRankText(PlayerStats.instance.Confidence, StatsMenu.instance.confidenceRankText);

            Debug.Log($"Insight Points Increased by: {confidenceValue}"); 
        }

        private static void IncreaseKarma(string[] data)
        { 
            var parameters = ConvertDataToParameters(data);

            int karmaValue;

            parameters.TryGetValue(PARAM_STAT, out karmaValue);

            VNMenuManager.instance.karmaMeter.gameObject.SetActive(true); 
            
            PlayerStats.instance.IncreaseKarmaPoints(PlayerStats.instance.Karma, karmaValue);

            VNMenuManager.instance.UpdateKarmaMeter(); 

            Debug.Log($"Karma Points Increased by: {karmaValue}"); 
        }

        private static void DecreaseKarma(string[] data) 
        {
            var parameters = ConvertDataToParameters(data);

            int karmaValue;

            parameters.TryGetValue(PARAM_STAT, out karmaValue);

            VNMenuManager.instance.karmaMeter.gameObject.SetActive(true); 

            PlayerStats.instance.DecreaseKarmaPoints(PlayerStats.instance.Karma, karmaValue);

            VNMenuManager.instance.UpdateKarmaMeter(); 

            Debug.Log($"Karma Points Increased by: {karmaValue}");
        } 

        private static void IncreaseAppreciationGrowth(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            int appreciationValue;

            parameters.TryGetValue(PARAM_STAT, out appreciationValue);

            PlayerStats.instance.IncreaseAppreciationGrowthPoints(PlayerStats.instance.Appreciation, appreciationValue);

            Debug.Log($"Parent Appreciation Growth Points Increased by: {appreciationValue}");
        }

        private static void IncreaseMoney(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            float moneyIncreaseValue; 

            parameters.TryGetValue(PARAM_MONEY, out moneyIncreaseValue);

            // Access the TextMeshPro component from the parent GameObject (moneyUpValue) and set its text
            VNMenuManager.instance.moneyUpValue.GetComponentInChildren<TextMeshProUGUI>().text = $"+ ${moneyIncreaseValue:N2}"; 

            VNMenuManager.instance.moneyAddedText.text = $"+ {moneyIncreaseValue}"; 

            VNMenuManager.instance.AddMoney(moneyIncreaseValue); 

            Debug.Log($"Money Increased by: {moneyIncreaseValue}"); 
        }

        private static void IncreaseMoneyAnim(string[] data)
        {
            var parameters = ConvertDataToParameters(data); 

            string animation; 

            parameters.TryGetValue(PARAM_MONEYUP, out animation); 

            VNMenuManager.instance.PlayMoneyUpValue(animation);

            Debug.Log($"Animation Plays: {animation}");  
        }

        private static void DecreaseMoney(string[] data) 
        {
            var parameters = ConvertDataToParameters(data);

            float moneyDecreaseValue;

            parameters.TryGetValue(PARAM_MONEY, out moneyDecreaseValue);

            // Access the TextMeshPro component from the parent GameObject (moneyUpValue) and set its text
            VNMenuManager.instance.moneyDownValue.GetComponentInChildren<TextMeshProUGUI>().text = $"- ${moneyDecreaseValue:N2}";  

            VNMenuManager.instance.moneyAddedText.text = $"- {moneyDecreaseValue}";   

            VNMenuManager.instance.DeductMoney(moneyDecreaseValue); 

            Debug.Log($"Money Decrease by: {moneyDecreaseValue}"); 
        }

        private static void DecreaseMoneyAnim(string[] data)
        {
            var parameters = ConvertDataToParameters(data); 

            string animation; 

            parameters.TryGetValue(PARAM_MONEYDOWN, out animation);

            VNMenuManager.instance.PlayMoneyDownValue(animation); 

            Debug.Log($"Animation Plays: {animation}"); 
        }

        private static void EnablePhone(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            bool phoneEnabled; 

            parameters.TryGetValue(PARAM_PHONE, out phoneEnabled, defaultValue: false);  

            VNMenuManager.instance.PhoneButtonInteractive(phoneEnabled); 
        } 

        /*private static void RankUpCharacter(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            string characterName;

            int rankValue;

            parameters.TryGetValue(PARAM_CHARACTERNAME, out characterName);
            parameters.TryGetValue(PARAM_RANKVALUE, out rankValue); 

            FriendshipStat.MarkHangoutCompleted(characterName, rankValue); 
        }*/

        private static void DisplayRankUpPage(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            string characterName;

            parameters.TryGetValue(PARAM_CHARACTERNAME, out characterName); 

            RankUpPage.instance.StartCoroutine(RankUpPage.instance.FadeInRankUpPage());
            RankUpPage.instance.ShowRankUp(characterName); 
        }

        private static void IncreaseFriendshipA(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            int friendshipValue;

            parameters.TryGetValue(PARAM_FRIEND, out friendshipValue); 

            //StatsMenu.instance.CharismaLevelByOne();
            // Assuming Charisma is a PlayerStat managed by PlayerStats.instance 

            FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendA, friendshipValue);
            StatsMenu.instance.UpdateFriendshipRankText(FriendshipStats.instance.FriendA, StatsMenu.instance.friendA_RankText); 

            Debug.Log($"Friendship A Points Increased by: {friendshipValue}"); 
        }

        private static void IncreaseFriendshipB(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            int friendshipValue;

            parameters.TryGetValue(PARAM_FRIEND, out friendshipValue);

            //StatsMenu.instance.CharismaLevelByOne();
            // Assuming Charisma is a PlayerStat managed by PlayerStats.instance 

            FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendB, friendshipValue);
            StatsMenu.instance.UpdateFriendshipRankText(FriendshipStats.instance.FriendB, StatsMenu.instance.friendB_RankText);

            Debug.Log($"Friendship B Points Increased by: {friendshipValue}");
        }

        private static void IncreaseFriendshipC(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            int friendshipValue;

            parameters.TryGetValue(PARAM_FRIEND, out friendshipValue);

            //StatsMenu.instance.CharismaLevelByOne();
            // Assuming Charisma is a PlayerStat managed by PlayerStats.instance 

            FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendC, friendshipValue);
            StatsMenu.instance.UpdateFriendshipRankText(FriendshipStats.instance.FriendC, StatsMenu.instance.friendC_RankText);

            Debug.Log($"Friendship C Points Increased by: {friendshipValue}");
        }

        private static void IncreaseFriendshipD(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            int friendshipValue;

            parameters.TryGetValue(PARAM_FRIEND, out friendshipValue);

            //StatsMenu.instance.CharismaLevelByOne();
            // Assuming Charisma is a PlayerStat managed by PlayerStats.instance 

            FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendD, friendshipValue);
            StatsMenu.instance.UpdateFriendshipRankText(FriendshipStats.instance.FriendD, StatsMenu.instance.friendD_RankText);

            Debug.Log($"Friendship D Points Increased by: {friendshipValue}");
        }

        private static void IncreaseFriendshipE(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            int friendshipValue;

            parameters.TryGetValue(PARAM_FRIEND, out friendshipValue);

            //StatsMenu.instance.CharismaLevelByOne();
            // Assuming Charisma is a PlayerStat managed by PlayerStats.instance 

            FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendE, friendshipValue);
            StatsMenu.instance.UpdateFriendshipRankText(FriendshipStats.instance.FriendE, StatsMenu.instance.friendE_RankText);

            Debug.Log($"Friendship E Points Increased by: {friendshipValue}");
        }

        private static void IncreaseFriendshipF(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            int friendshipValue;

            parameters.TryGetValue(PARAM_FRIEND, out friendshipValue);

            //StatsMenu.instance.CharismaLevelByOne();
            // Assuming Charisma is a PlayerStat managed by PlayerStats.instance 

            FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendF, friendshipValue);
            StatsMenu.instance.UpdateFriendshipRankText(FriendshipStats.instance.FriendF, StatsMenu.instance.friendF_RankText);

            Debug.Log($"Friendship F Points Increased by: {friendshipValue}");
        }

        private static void IncreaseFriendshipG(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            int friendshipValue;

            parameters.TryGetValue(PARAM_FRIEND, out friendshipValue);

            //StatsMenu.instance.CharismaLevelByOne();
            // Assuming Charisma is a PlayerStat managed by PlayerStats.instance 

            FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendG, friendshipValue);
            StatsMenu.instance.UpdateFriendshipRankText(FriendshipStats.instance.FriendG, StatsMenu.instance.friendG_RankText);

            Debug.Log($"Friendship G Points Increased by: {friendshipValue}");
        }

        private static void IncreaseFriendshipH(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            int friendshipValue;

            parameters.TryGetValue(PARAM_FRIEND, out friendshipValue);

            //StatsMenu.instance.CharismaLevelByOne();
            // Assuming Charisma is a PlayerStat managed by PlayerStats.instance 

            FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendH, friendshipValue);
            StatsMenu.instance.UpdateFriendshipRankText(FriendshipStats.instance.FriendH, StatsMenu.instance.friendH_RankText);

            Debug.Log($"Friendship H Points Increased by: {friendshipValue}");
        }

        private static void IncreaseFriendshipI(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            int friendshipValue;

            parameters.TryGetValue(PARAM_FRIEND, out friendshipValue);

            //StatsMenu.instance.CharismaLevelByOne();
            // Assuming Charisma is a PlayerStat managed by PlayerStats.instance 

            FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendI, friendshipValue);
            StatsMenu.instance.UpdateFriendshipRankText(FriendshipStats.instance.FriendI, StatsMenu.instance.friendI_RankText);

            Debug.Log($"Friendship I Points Increased by: {friendshipValue}");
        }

        private static void IncreaseFriendshipJ(string[] data)
        {
            var parameters = ConvertDataToParameters(data);

            int friendshipValue;

            parameters.TryGetValue(PARAM_FRIEND, out friendshipValue);

            
            // Assuming Charisma is a PlayerStat managed by PlayerStats.instance 

            FriendshipStats.instance.IncreaseStatPoints(FriendshipStats.instance.FriendJ, friendshipValue);
            StatsMenu.instance.UpdateFriendshipRankText(FriendshipStats.instance.FriendJ, StatsMenu.instance.friendJ_RankText);

            Debug.Log($"Friendship J Points Increased by: {friendshipValue}"); 
        } 

        private static void LoadNewScene(string data)
        {
            SceneManager.LoadScene(data); 
        } 
        
        private static void DisplayUITip(string[] data) 
        {
            var parameters = ConvertDataToParameters(data);

            string UITip;

            parameters.TryGetValue(PARAM_UITip, out UITip);

            VNMenuManager.instance.DisplayTipUI(UITip); 

            Debug.Log($"Animation Plays: {UITip}");
        } 

        private static void HideUITip()
        {
            VNMenuManager.instance.HideTipUI(); 
        } 
        
        //private static void CafeText()
        //{
            //VNMenuManager.instance.TheCafeText();  
        //}

        private static void ShowLoadingScreen() 
        {
            DialogueSystem.instance.ShowLoading(); 
        }  
    }
}