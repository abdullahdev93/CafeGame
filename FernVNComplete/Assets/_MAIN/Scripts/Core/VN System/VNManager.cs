using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using TESTING;
using UnityEngine;

namespace VISUALNOVEL
{
    public class VNManager : MonoBehaviour
    {
        public static VNManager instance { get; private set; }

        [SerializeField] private VisualNovelSO config;

        public Camera mainCamera;

        private void Awake()
        {
            instance = this;

            VNDatabaseLinkSetup linkSetup = GetComponent<VNDatabaseLinkSetup>();
            linkSetup.SetupExternalLinks();

            if (VNGameSave.activeFile == null)
                VNGameSave.activeFile = new VNGameSave();
        }

        private void Start()
        {
            LoadGame();
        }

        private void LoadGame()
        {
            if (VNGameSave.activeFile.newGame)
            { 
                //if (VariableStore.TryGetValue("Default.StartingFile", out object value)) 
                if (PlayerPrefs.HasKey("StartingFile"))
                {
                    string startingFile = PlayerPrefs.GetString("StartingFile"); 
                    //string startingFile = value.ToString(); 
                    PlayerPrefs.Save(); 

                    Debug.Log($"Loading game with StartingFile: {startingFile}");

                    if (!string.IsNullOrEmpty(startingFile))
                    {
                        List<string> lines = FileManager.ReadTextAsset(GetFileByName(startingFile));
                        if (lines != null && lines.Count > 0)
                        {
                            Conversation start = new Conversation(lines);
                            DialogueSystem.instance.Say(start);
                            return;
                        }
                    }

                    // If no valid file is found, load a fallback (ApartmentFile)
                    List<string> fallbackLines = FileManager.ReadTextAsset(config.apartmentFile);
                    Conversation fallbackStart = new Conversation(fallbackLines);
                    DialogueSystem.instance.Say(fallbackStart);

                    Debug.LogWarning("No valid StartingFile found. Loading fallback ApartmentFile.");

                    // Clean up PlayerPrefs
                    PlayerPrefs.DeleteKey("StartingFile");
                    PlayerPrefs.Save();

                    // Clean up the variable
                    //VariableStore.RemoveVariable("Default.StartingFile"); 
                }

                else
                {
                    // Get the current season, day, and activity counter from the CalendarSystem
                    CalendarSystem.Season currentSeason = CalendarSystem.Instance.GetCurrentSeason();
                    int currentDay = CalendarSystem.Instance.GetCurrentDay();
                    int activityCounter = CalendarSystem.Instance.activityCounter;

                    // Fetch the appropriate TextAsset based on season, day, and activityCounter
                    TextAsset startingFile = config.GetStartingFile(currentSeason, currentDay, activityCounter);

                    // Load and start the conversation from the file
                    List<string> lines = FileManager.ReadTextAsset(startingFile);
                    Conversation start = new Conversation(lines);
                    DialogueSystem.instance.Say(start);
                }
            } 

            else
            {
                VNGameSave.activeFile.Activate();
            }
        }

        private TextAsset GetFileByName(string fileName)
        {
            switch (fileName)
            {
                case "ParkFile": return config.parkFile;
                case "MallFile": return config.mallFile;
                case "LibraryFile": return config.libraryFile;
                case "BarFile": return config.barFile;
                case "BarEveningFile": return config.barEveningFile;
                case "MoviesFile": return config.moviesFile;
                case "MoviesFileTwo": return config.moviesFileTwo;
                case "MeiHangOutTwo": return config.meiHangOutTwo;
                case "MeiHangOutThree": return config.meiHangOutThree;
                case "MeiHangOutFour": return config.meiHangOutFour;
                case "BowlingAlleyFile": return config.bowlingAlleyFile;
                case "ArcadeFile": return config.arcadeFile;
                case "UniversityFile": return config.universityFile;
                case "SimonUniversity": return config.simonUniversityFile;
                case "AfterGoodCafe": return config.afterGoodCafeFile;
                case "ConvienceStoreFile": return config.convienceStoreFile;
                case "WinningLuckyBucks": return config.winningLuckBucks;
                case "WinningRainbowRiches": return config.winningRainbowRiches;
                case "WinningMegaFortune": return config.winningMegaFortune;
                case "LosingLotteryTicket": return config.losingLotteryTicket;
                case "Negroni": return config.negroniFile;
                case "RedWine": return config.redWineFile;
                case "Mojito": return config.mojitoFile;
                case "Manhattan": return config.manhattanFile;
                case "HotToddy": return config.hotToddyFile;
                case "EspressoMartini": return config.expressoMartiniFile;
                case "OldFashioned": return config.oldFashionedFile;
                case "WhiskeySour": return config.whiskeySourFile;
                case "ShotOfTequila": return config.shotOfTequilaFile;
                case "Margarita": return config.margaritaFile;
                case "ArtOfConversation": return config.artOfConversation;
                case "ConvienceStoreRandomKarmaEvent": return config.convienceStoreRandomKarmaEvent;
                case "ConvienceStoreRandomKarmaEventTwo": return config.convienceStoreRandomKarmaEventTwo;
                case "ConvienceStoreRandomKarmaEventThree": return config.convienceStoreRandomKarmaEventThree;   
                case "Introduction": return config.introductionFile;
                case "JogMorningSpring": return config.jogMorningSpring;
                case "JogAfternoonSpring": return config.jogAfternoonSpring;
                case "JogEveningSpring": return config.jogEveningSpring;
                case "MeiJogAfternoonSpring": return config.meiJogAfternoonSpring;
                case "SimonJogAfternoonSpring": return config.simonJogAfternoonSpring;
                case "MeiEspressoMartini": return config.meiEspressoMartini;
                case "MeiMargarita": return config.meiMargarita; 
                case "MeiOldFashioned": return config.meiOldFashioned;
                case "MeiShotOfTequila": return config.meiShotOfTequila; 
                case "MeiRedWine": return config.meiRedWine;
                case "MeiMojito": return config.meiMojito;
                case "MeiManhattan": return config.meiManhattan;
                case "MeiHotToddy": return config.meiHotToddy;
                case "MeiWhiskeySour": return config.meiWhiskeySour;
                case "MeiNegroni": return config.meiNegroni;
                case "Mei_Jog_Spring": return config.meiJogSpring; 
                case "NinaEightIntelligence": return config.ninaEightIntelligence;
                case "NinaEightEmpathy": return config.ninaEightEmpathy; 
                case "NinaHangOutOne": return config.ninaHangOutOne;
                case "NinaHangoutTwo": return config.ninaHangOutTwo; 
                case "NinaHangoutThree": return config.ninaHangOutThree;
                case "NinaHangoutFour": return config.ninaHangOutFour;
                case "NinaHangoutFive": return config.ninaHangOutFive; 
                case "NinaHangoutSix": return config.ninaHangOutSix;
                case "NinaHangoutSeven": return config.ninaHangOutSeven;
                case "NinaHangoutEight": return config.ninaHangOutEight; 
                case "NinaHangoutNine": return config.ninaHangOutNine; 
                case "NinaHangoutTen": return config.ninaHangOutTen; 
                case "SimonArcade": return config.simonArcade;
                case "SimonHangoutSeven": return config.simonHangOutSeven;
                case "SimonHangoutSix": return config.simonHangOutSix;
                case "MeiHangOutOne": return config.meiHangOutOne; 

                case "WelcomeToBaywoodPARTTWO": return config.welcomeToBaywoodPARTTWO;
                case "MeiHangOut1PARTTWO": return config.meiHangOutOnePARTTWO;
                case "MeiHangOut2PARTTWO": return config.meiHangOutTwoPARTTWO;
                case "MeiHangOut3PARTTWO": return config.meiHangOutThreePARTTWO;
                case "MeiHangOut4PARTTWO": return config.meiHangOutFourPARTTWO;
                case "MeiHangOut5PARTTWO": return config.meiHangOutFivePARTTWO;
                case "MeiHangOut6PARTTWO": return config.meiHangOutSixPARTTWO;
                case "MeiHangOut7PARTTWO": return config.meiHangOutSevenPARTTWO;
                case "MeiHangOut8PARTTWO": return config.meiHangOutEightPARTTWO;
                case "MeiHangOut9PARTTWO": return config.meiHangOutNinePARTTWO;
                case "MeiHangOut10PARTTWO": return config.meiHangOutTenPARTTWO; 
                case "ApartmentStartM": return config.apartmentStartM;
                case "ApartmentStartA": return config.apartmentStartA;
                case "ApartmentStartE": return config.apartmentStartE;
                //case "Mei_Jog_Spring_Afternoon_Sunny_Con_Char_FL_2_01": return config.Mei_Jog_Spring_Afternoon_Sunny_Con_Char_FL_02_File_01;
                //case "Mei_Jog_Spring_Afternoon_Sunny_Con_FL_1_01": return config.Mei_Jog_Spring_Afternoon_Sunny_Con_FL_01_File_01;
                //case "Mei_Jog_Spring_Afternoon_Sunny_FL_1_01": return config.Mei_Jog_Spring_Afternoon_Sunny_FL_01_File_01;
                default: return config.apartmentFile; // Default file if unknown
            }
        }


        /*private void LoadGame()
        {
            if (VNGameSave.activeFile.newGame)
            {
                // Check for specific StartingFile set in PlayerPrefs
                if (PlayerPrefs.HasKey("StartingFile"))
                {
                    string startingFile = PlayerPrefs.GetString("StartingFile");

                    // Check for various locations
                    if (startingFile == "ParkFile")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.parkFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "MallFile")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.mallFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "LibraryFile")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.libraryFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "BarFile")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.barFile); 
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "BarEveningFile") 
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.barEveningFile); 
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "BarAfterDartsFile") 
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.barAfterDartsFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "BarAfterAmazingKaraokeFile")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.barAfterAmazingKaraokeFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "BarAfterGoodKaraokeFile")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.barAfterGoodKaraokeFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "BarAfterOKKaraokeFile")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.barAfterOKKaraokeFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "BarAfterTerribleKaraokeFile")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.barAfterTerribleKaraokeFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "BarAfterHorrendousKaraokeFile")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.barAfterHorrendousKaraokeFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "MoviesFile")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.moviesFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "MoviesHJFile")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.moviesHJFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "MoviesADFile")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.moviesADFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "MoviesHSFile")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.moviesHSFile); 
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "MoviesFileTwo") 
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.moviesFileTwo); 
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "MoviesFileMei") 
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.moviesFileMei); 
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    } 
                    else if (startingFile == "PoolFileMei") 
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.poolFileMei); 
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "RehearsalFileMei")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.rehearsalFileMei); 
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "BarNina")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.barNina);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "NinaHangOutOne")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.ninaHangOutOne);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    } 
                    else if (startingFile == "NinaWalkHome")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.ninaWalkHome); 
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "NinaHookUp")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.ninaHookUp);  
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "NinaCompany")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.ninaCompany);  
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "AlexCritterTrails") 
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.alexCritterTrails); 
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "Introduction") 
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.introductionFile); 
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    } 
                    else if (startingFile == "MeiHangOutTwo")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.meiHangOutTwo); 
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "MeiHangOutThree")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.meiHangOutThree);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "MeiHangOutFour")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.meiHangOutFour);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "MeiHangOutFive")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.meiHangOutFive);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "MeiHangOutSix")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.meiHangOutSix);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "MeiHangOutSeven")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.meiHangOutSeven);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "MeiHangOutEight")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.meiHangOutEight);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "MeiHangOutNine")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.meiHangOutNine); 
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "BowlingAlleyFile")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.bowlingAlleyFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "ArcadeFile")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.arcadeFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }
                    else if (startingFile == "UniversityFile")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.universityFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }

                    else if (startingFile == "SimonUniversity")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.simonUniversityFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }

                    else if (startingFile == "AfterGoodCafe")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.afterGoodCafeFile);  
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    } 

                    else if (startingFile == "ConvienceStoreFile")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.convienceStoreFile); 
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }

                    else if (startingFile == "WinningLuckyBucks")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.winningLuckBucks);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }

                    else if (startingFile == "WinningRainbowRiches")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.winningRainbowRiches); 
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }

                    else if (startingFile == "WinningMegaFortune")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.winningMegaFortune); 
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }

                    else if (startingFile == "LosingLotteryTicket")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.losingLotteryTicket); 
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }

                    else if (startingFile == "Negroni")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.negroniFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }

                    else if (startingFile == "RedWine")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.redWineFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }

                    else if (startingFile == "Mojito")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.mojitoFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }

                    else if (startingFile == "Manhattan")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.manhattanFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }

                    else if (startingFile == "HotToddy")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.hotToddyFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }

                    else if (startingFile == "EspressoMartini")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.expressoMartiniFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }

                    else if (startingFile == "OldFashioned")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.oldFashionedFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }

                    else if (startingFile == "WhiskeySour")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.whiskeySourFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }

                    else if (startingFile == "ShopOfTequila")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.shotOfTequilaFile);
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }

                    else if (startingFile == "Margarita")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.margaritaFile); 
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }

                    else if (startingFile == "AfterGoodCafeALT")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.afterGoodCafeALTFile); 
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }

                    else if (startingFile == "ArtOfConversation")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.artOfConversation); 
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }

                    else if (startingFile == "ApartmentFile")
                    {
                        List<string> lines = FileManager.ReadTextAsset(config.apartmentFile); 
                        Conversation start = new Conversation(lines);
                        DialogueSystem.instance.Say(start);
                    }

                    // Clean up PlayerPrefs
                    PlayerPrefs.DeleteKey("StartingFile");
                    PlayerPrefs.Save();
                }
                else
                {
                    // Get the current season, day, and activity counter from the CalendarSystem
                    CalendarSystem.Season currentSeason = CalendarSystem.Instance.GetCurrentSeason();
                    int currentDay = CalendarSystem.Instance.GetCurrentDay();
                    int activityCounter = CalendarSystem.Instance.activityCounter;

                    // Fetch the appropriate TextAsset based on season, day, and activityCounter
                    TextAsset startingFile = config.GetStartingFile(currentSeason, currentDay, activityCounter);

                    // Load and start the conversation from the file
                    List<string> lines = FileManager.ReadTextAsset(startingFile);
                    Conversation start = new Conversation(lines);
                    DialogueSystem.instance.Say(start);
                }
            }
            else
            {
                VNGameSave.activeFile.Activate();
            }
        }*/ 
    }
}
