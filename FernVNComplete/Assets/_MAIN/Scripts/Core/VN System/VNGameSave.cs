using DIALOGUE;
using History;
using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace VISUALNOVEL
{
    [System.Serializable]
    public class VNGameSave
    {
        public static VNGameSave activeFile = null;

        public const bool ENCRYPT = false;
        public const string FILE_TYPE = ".vns";
        public const string SCREENSHOT_FILE_TYPE = ".jpg";
        public const float SCREENSHOT_DOWNSCALE_AMOUNT = 0.25f;

        public string filePath => $"{FilePaths.gameSaves}{slotNumber}{FILE_TYPE}";
        public string screenshotPath => $"{FilePaths.gameSaves}{slotNumber}{SCREENSHOT_FILE_TYPE}";


        public string playerName;
        public int slotNumber = 1;

        public bool newGame = true;
        public string[] activeConversations;
        public HistoryState activeState;
        public HistoryState[] historyLogs;
        public VN_VariableData[] variables;

        public string timestamp;
        public string savedSeason;
        public int savedDay;
        public string savedYear;
        public string savedTimeOfDay;  // New field for saving the time of day 

        public string savedDayOfWeek; // New field to save the day of the week  

        public static VNGameSave Load(string filePath, bool activateOnLoad = false)
        {
            VNGameSave save = FileManager.Load<VNGameSave>(filePath, ENCRYPT);

            activeFile = save;

            if (activateOnLoad)
                save.Activate();

            return save;
        }

        public void Save()
        {
            newGame = false;

            activeState = HistoryState.Capture();
            historyLogs = HistoryManager.instance.history.ToArray();
            activeConversations = GetConversationData(); 
            SaveFriendshipAndPlayerStatsToVariables(); 
            variables = GetVariableData();

            // Use the system's local time and format it with AM/PM
            DateTime localTime = DateTime.Now;
            timestamp = localTime.ToString("MM/dd/yyyy - hh:mm tt");  

            // Get the current season, day, year, and time of day from the CalendarSystem
            if (CalendarSystem.Instance != null)
            {
                savedSeason = CalendarSystem.Instance.currentSeason.ToString();
                savedDay = CalendarSystem.Instance.GetCurrentDay();
                savedYear = CalendarSystem.Instance.currentSeason == CalendarSystem.Season.SpringA ? "2020" : "2019";
                savedTimeOfDay = CalendarSystem.Instance.activityPhases[CalendarSystem.Instance.activityCounter];
                savedDayOfWeek = CalendarSystem.Instance.GetCurrentDayOfWeek(); 
            }

            ScreenshotMaster.CaptureScreenshot(VNManager.instance.mainCamera, Screen.width, Screen.height, SCREENSHOT_DOWNSCALE_AMOUNT, screenshotPath);

            string saveJSON = JsonUtility.ToJson(this);
            FileManager.Save(filePath, saveJSON, ENCRYPT);
        }

        private void SaveFriendshipAndPlayerStatsToVariables()
        {
            // Save Friendship Stats
            SaveFriendStat("Mei", FriendshipStats.instance.FriendA);
            SaveFriendStat("Alex", FriendshipStats.instance.FriendB);
            SaveFriendStat("Nina", FriendshipStats.instance.FriendC);
            SaveFriendStat("Simon", FriendshipStats.instance.FriendD);
            SaveFriendStat("FriendE", FriendshipStats.instance.FriendE);
            SaveFriendStat("FriendF", FriendshipStats.instance.FriendF);
            SaveFriendStat("FriendG", FriendshipStats.instance.FriendG);
            SaveFriendStat("FriendH", FriendshipStats.instance.FriendH);
            SaveFriendStat("FriendI", FriendshipStats.instance.FriendI);
            SaveFriendStat("FriendJ", FriendshipStats.instance.FriendJ);

            // Save Player Stats
            SavePlayerStat("Charisma", PlayerStats.instance.Charisma);
            SavePlayerStat("Creativity", PlayerStats.instance.Creativity);
            SavePlayerStat("Empathy", PlayerStats.instance.Empathy);
            SavePlayerStat("Humor", PlayerStats.instance.Humor);
            SavePlayerStat("Endurance", PlayerStats.instance.Endurance);
            SavePlayerStat("Patience", PlayerStats.instance.Patience);
            SavePlayerStat("Intelligence", PlayerStats.instance.Intelligence);
            SavePlayerStat("Courage", PlayerStats.instance.Courage);
            SavePlayerStat("Kindness", PlayerStats.instance.Kindness);
            SavePlayerStat("Confidence", PlayerStats.instance.Confidence);
            SavePlayerStat("Karma", PlayerStats.instance.Karma);

            // Save Pronouns
            VariableStore.TrySetValue("PlayerStats.Pronoun.Subject", PlayerStats.instance.Pronouns.Subject);
            VariableStore.TrySetValue("PlayerStats.Pronoun.Object", PlayerStats.instance.Pronouns.Object);
            VariableStore.TrySetValue("PlayerStats.Pronoun.PossessiveAdjective", PlayerStats.instance.Pronouns.PossessiveAdjective);
            VariableStore.TrySetValue("PlayerStats.Pronoun.PossessivePronoun", PlayerStats.instance.Pronouns.PossessivePronoun);
            VariableStore.TrySetValue("PlayerStats.Pronoun.Reflexive", PlayerStats.instance.Pronouns.Reflexive); 

            // Save Money
            if (VariableStore.TryGetValue("PlayerStats.Money", out object money)) 
                VariableStore.TrySetValue("PlayerStats.Money_Saved", money); 
        }

        private void SaveFriendStat(string name, FriendshipStat stat)
        {
            VariableStore.TrySetValue($"FriendshipStats.{name}", stat.Level);
            VariableStore.TrySetValue($"FriendshipStats.{name}_Points", stat.Points);
        }

        private void SavePlayerStat(string name, PlayerStat stat)
        {
            VariableStore.TrySetValue($"PlayerStats.{name}", stat.Level);
            VariableStore.TrySetValue($"PlayerStats.{name}_Points", stat.Points);
        }

        public void Activate()
        {
            if (activeState != null)
                activeState.Load();

            HistoryManager.instance.history = historyLogs.ToList();
            HistoryManager.instance.logManager.Clear();
            HistoryManager.instance.logManager.Rebuild();

            SetVariableData(); 

            RestoreFriendshipAndPlayerStatsFromVariables(); 

            SetConversationData();

            DialogueSystem.instance.prompt.Hide();

            // ADD THIS LINE BELOW
            VNMenuManager.instance?.SendMessage("UpdateEarningsText", PlayerStats.GetMoney(), SendMessageOptions.DontRequireReceiver); 

            // Refresh the Stats Menu UI after restoring stat values
            if (StatsMenu.instance != null)
            {
                StatsMenu.instance.UpdateStatsBars();
                StatsMenu.instance.UpdateAllRankTexts();
                StatsMenu.instance.UpdateFriendshipBars();
                StatsMenu.instance.UpdateAllFriendshipRankTexts();
            } 
        }

        private void RestoreFriendshipAndPlayerStatsFromVariables()
        {
            // Friendship Levels
            RestoreFriendStat("Mei", FriendshipStats.instance.FriendA);
            RestoreFriendStat("Alex", FriendshipStats.instance.FriendB);
            RestoreFriendStat("Nina", FriendshipStats.instance.FriendC);
            RestoreFriendStat("Simon", FriendshipStats.instance.FriendD);
            RestoreFriendStat("FriendE", FriendshipStats.instance.FriendE);
            RestoreFriendStat("FriendF", FriendshipStats.instance.FriendF);
            RestoreFriendStat("FriendG", FriendshipStats.instance.FriendG);
            RestoreFriendStat("FriendH", FriendshipStats.instance.FriendH);
            RestoreFriendStat("FriendI", FriendshipStats.instance.FriendI);
            RestoreFriendStat("FriendJ", FriendshipStats.instance.FriendJ);

            // Player Levels
            RestorePlayerStat("Charisma", PlayerStats.instance.Charisma);
            RestorePlayerStat("Creativity", PlayerStats.instance.Creativity);
            RestorePlayerStat("Empathy", PlayerStats.instance.Empathy);
            RestorePlayerStat("Humor", PlayerStats.instance.Humor);
            RestorePlayerStat("Endurance", PlayerStats.instance.Endurance);
            RestorePlayerStat("Patience", PlayerStats.instance.Patience);
            RestorePlayerStat("Intelligence", PlayerStats.instance.Intelligence);
            RestorePlayerStat("Courage", PlayerStats.instance.Courage);
            RestorePlayerStat("Kindness", PlayerStats.instance.Kindness);
            RestorePlayerStat("Confidence", PlayerStats.instance.Confidence);
            RestorePlayerStat("Karma", PlayerStats.instance.Karma);

            if (VariableStore.TryGetValue("PlayerStats.Pronoun.Subject", out object subj))
                PlayerStats.instance.SetPronoun("Subject", subj);
            if (VariableStore.TryGetValue("PlayerStats.Pronoun.Object", out object obj))
                PlayerStats.instance.SetPronoun("Object", obj);
            if (VariableStore.TryGetValue("PlayerStats.Pronoun.PossessiveAdjective", out object posAdj))
                PlayerStats.instance.SetPronoun("PossessiveAdjective", posAdj);
            if (VariableStore.TryGetValue("PlayerStats.Pronoun.PossessivePronoun", out object posPro))
                PlayerStats.instance.SetPronoun("PossessivePronoun", posPro);
            if (VariableStore.TryGetValue("PlayerStats.Pronoun.Reflexive", out object refl))
                PlayerStats.instance.SetPronoun("Reflexive", refl); 

            if (VariableStore.TryGetValue("PlayerStats.Money_Saved", out object savedMoney))
                VariableStore.TrySetValue("PlayerStats.Money", savedMoney);

        }

        private void RestoreFriendStat(string name, FriendshipStat stat)
        {
            if (VariableStore.TryGetValue($"FriendshipStats.{name}", out object level))
                stat.Level = Convert.ToInt32(level);

            if (VariableStore.TryGetValue($"FriendshipStats.{name}_Points", out object points))
                stat.Points = Convert.ToInt32(points);
        }

        private void RestorePlayerStat(string name, PlayerStat stat)
        {
            if (VariableStore.TryGetValue($"PlayerStats.{name}", out object level))
                stat.Level = Convert.ToInt32(level);

            if (VariableStore.TryGetValue($"PlayerStats.{name}_Points", out object points))
                stat.Points = Convert.ToInt32(points);
        }

        private string[] GetConversationData()
        {
            List<string> retData = new List<string>();
            var conversations = DialogueSystem.instance.conversationManager.GetConversationQueue();

            for (int i = 0; i < conversations.Length; i++)
            {
                var conversation = conversations[i];
                string data = "";

                if (conversation.file != string.Empty)
                {
                    var compressedData = new VN_ConversationDataCompressed();
                    compressedData.fileName = conversation.file;
                    compressedData.progress = conversation.GetProgress();
                    compressedData.startIndex = conversation.fileStartIndex;
                    compressedData.endIndex = conversation.fileEndIndex;
                    data = JsonUtility.ToJson(compressedData);
                }
                else
                {
                    var fullData = new VN_ConversationData();
                    fullData.conversation = conversation.GetLines();
                    fullData.progress = conversation.GetProgress();
                    data = JsonUtility.ToJson(fullData);
                }

                retData.Add(data);
            }

            return retData.ToArray();
        }

        private void SetConversationData()
        {
            for (int i = 0; i < activeConversations.Length; i++)
            {
                try
                {
                    string data = activeConversations[i];
                    Conversation conversation = null;

                    var fullData = JsonUtility.FromJson<VN_ConversationData>(data);
                    if (fullData != null && fullData.conversation != null && fullData.conversation.Count > 0)
                    {
                        conversation = new Conversation(fullData.conversation, fullData.progress);
                    }
                    else
                    {
                        var compressedData = JsonUtility.FromJson<VN_ConversationDataCompressed>(data);
                        if (compressedData != null && compressedData.fileName != string.Empty)
                        {
                            TextAsset file = Resources.Load<TextAsset>(compressedData.fileName);

                            int count = compressedData.endIndex - compressedData.startIndex;

                            List<string> lines = FileManager.ReadTextAsset(file).Skip(compressedData.startIndex).Take(count + 1).ToList();

                            conversation = new Conversation(lines, compressedData.progress, compressedData.fileName, compressedData.startIndex, compressedData.endIndex);
                        }
                        else
                        {
                            Debug.LogError($"Unknown conversation format! Unable to reload conversation from VNGameSave using data '{data}'");
                        }
                    }

                    if (conversation != null && conversation.GetLines().Count > 0)
                    {
                        if (i == 0)
                            DialogueSystem.instance.conversationManager.StartConversation(conversation);
                        else
                            DialogueSystem.instance.conversationManager.Enqueue(conversation);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Encountered error while extracting saved conversation data! {e}");
                    continue;
                }
            }
        }

        private VN_VariableData[] GetVariableData()
        {
            List<VN_VariableData> retData = new List<VN_VariableData>();

            foreach (var database in VariableStore.databases.Values)
            {
                foreach (var variable in database.variables)
                {
                    VN_VariableData variableData = new VN_VariableData();
                    variableData.name = $"{database.name}.{variable.Key}";
                    string val = $"{variable.Value.Get()}";
                    variableData.value = val;
                    variableData.type = val == string.Empty ? "System.String" : variable.Value.Get().GetType().ToString();
                    retData.Add(variableData);
                }
            }

            return retData.ToArray();
        }

        private void SetVariableData()
        {
            foreach (var variable in variables)
            {
                string val = variable.value;

                switch (variable.type)
                {
                    case "System.Boolean":
                        if (bool.TryParse(val, out bool b_val))
                        {
                            VariableStore.TrySetValue(variable.name, b_val);
                            continue;
                        }
                        break;
                    case "System.Int32":
                        if (int.TryParse(val, out int i_val))
                        {
                            VariableStore.TrySetValue(variable.name, i_val);
                            continue;
                        }
                        break;
                    case "System.Single":
                        if (float.TryParse(val, out float f_val))
                        {
                            VariableStore.TrySetValue(variable.name, f_val);
                            continue;
                        }
                        break;
                    case "System.String":
                        VariableStore.TrySetValue(variable.name, val);
                        continue;
                }

                Debug.LogError($"Could not interpret variable type. {variable.name} = {variable.type}");
            }
        }
    }
}
