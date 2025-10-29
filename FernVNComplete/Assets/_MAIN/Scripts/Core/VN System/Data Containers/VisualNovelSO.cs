using UnityEngine;

[CreateAssetMenu(fileName = "Visual Novel Configuration", menuName = "Dialogue System/Visual Novel Configuration Asset")]
public class VisualNovelSO : ScriptableObject
{
    /*public TextAsset springFile;
    public TextAsset summerFile;
    public TextAsset fallFile;
    public TextAsset winterFile; 
    public TextAsset springAFile;
    public TextAsset introductionFile; 
    public TextAsset apartmentFile; // Apartment file as fallback    
    public TextAsset apartmentStartM; // Apartment file as fallback 
    public TextAsset apartmentStartA; // Apartment file as fallback 
    public TextAsset apartmentStartE; // Apartment file as fallback 
    public TextAsset parkFile;
    public TextAsset mallFile; // Add mallFile here 
    public TextAsset libraryFile;
    public TextAsset barFile;
    public TextAsset barEveningFile; 
    public TextAsset barAfterDartsFile;
    public TextAsset barAfterAmazingKaraokeFile;  
    public TextAsset barAfterGoodKaraokeFile;  
    public TextAsset barAfterOKKaraokeFile;  
    public TextAsset barAfterTerribleKaraokeFile;  
    public TextAsset barAfterHorrendousKaraokeFile;  
    public TextAsset moviesFile;
    public TextAsset moviesHJFile;
    public TextAsset moviesADFile;
    public TextAsset moviesHSFile; 
    public TextAsset moviesFileTwo;
    public TextAsset moviesFileThree;
    public TextAsset moviesFileFour;
    public TextAsset moviesFileMei;
    public TextAsset poolFileMei;
    public TextAsset rehearsalFileMei;
    public TextAsset barNina;  
    public TextAsset ninaWalkHome; 
    public TextAsset ninaHookUp;
    public TextAsset ninaCompany;
    public TextAsset alexCritterTrails;
    public TextAsset meiHangOutOne; 
    public TextAsset meiHangOutTwo;
    public TextAsset meiHangOutThree;
    public TextAsset meiHangOutFour;
    public TextAsset meiHangOutFive;
    public TextAsset meiHangOutSix; 
    public TextAsset meiHangOutSeven;
    public TextAsset meiHangOutEight;
    public TextAsset meiHangOutNine;
    public TextAsset meiHangOutTen; 
    public TextAsset bowlingAlleyFile;
    public TextAsset arcadeFile;
    public TextAsset universityFile;
    public TextAsset simonUniversityFile;
    public TextAsset afterGoodCafeFile;
    public TextAsset afterGoodCafeALTFile;
    public TextAsset convienceStoreFile;
    public TextAsset winningLuckBucks;
    public TextAsset winningRainbowRiches;
    public TextAsset winningMegaFortune;
    public TextAsset losingLotteryTicket;
    public TextAsset negroniFile;
    public TextAsset redWineFile;
    public TextAsset mojitoFile;
    public TextAsset manhattanFile;
    public TextAsset hotToddyFile; 
    public TextAsset expressoMartiniFile;
    public TextAsset oldFashionedFile;
    public TextAsset whiskeySourFile;
    public TextAsset shotOfTequilaFile;
    public TextAsset margaritaFile;
    public TextAsset artOfConversation;
    public TextAsset convienceStoreRandomKarmaEvent;
    public TextAsset convienceStoreRandomKarmaEventTwo;
    public TextAsset convienceStoreRandomKarmaEventThree;
    public TextAsset jogMorningSpring;
    public TextAsset jogAfternoonSpring;
    public TextAsset jogEveningSpring;
    public TextAsset meiJogAfternoonSpring;
    public TextAsset simonJogAfternoonSpring;
    public TextAsset meiEspressoMartini;
    public TextAsset meiMargarita;
    public TextAsset meiOldFashioned;
    public TextAsset meiShotOfTequila;
    public TextAsset meiRedWine;
    public TextAsset meiMojito;
    public TextAsset meiManhattan;
    public TextAsset meiHotToddy;
    public TextAsset meiWhiskeySour;
    public TextAsset meiNegroni;
    public TextAsset meiJogSpring; 
    public TextAsset ninaEightIntelligence;
    public TextAsset ninaEightEmpathy; 
    public TextAsset ninaHangOutOne;
    public TextAsset ninaHangOutTwo; 
    public TextAsset ninaHangOutThree; 
    public TextAsset ninaHangOutFour; 
    public TextAsset ninaHangOutFive;
    public TextAsset ninaHangOutSix;
    public TextAsset ninaHangOutSeven; 
    public TextAsset ninaHangOutEight;
    public TextAsset ninaHangOutNine;
    public TextAsset ninaHangOutTen;
    public TextAsset simonArcade;
    public TextAsset simonHangOutSix; 
    public TextAsset simonHangOutSeven; 
    public TextAsset welcomeToBaywoodPARTTWO;
    public TextAsset meiHangOutOnePARTTWO; 
    public TextAsset meiHangOutTwoPARTTWO;
    public TextAsset meiHangOutThreePARTTWO;
    public TextAsset meiHangOutFourPARTTWO;
    public TextAsset meiHangOutFivePARTTWO;
    public TextAsset meiHangOutSixPARTTWO;
    public TextAsset meiHangOutSevenPARTTWO;
    public TextAsset meiHangOutEightPARTTWO;
    public TextAsset meiHangOutNinePARTTWO;
    public TextAsset meiHangOutTenPARTTWO; 
    //public TextAsset Mei_Jog_Spring_Afternoon_Sunny_Con_Char_FL_02_File_01;
    //public TextAsset Mei_Jog_Spring_Afternoon_Sunny_Con_FL_01_File_01;
    //public TextAsset Mei_Jog_Spring_Afternoon_Sunny_FL_01_File_01;*/

    [Header("Seasonal Files")]
    public TextAsset springFile;
    public TextAsset summerFile;
    public TextAsset fallFile;
    public TextAsset winterFile;
    public TextAsset springAFile;

    [Header("Introduction & Apartment")]
    public TextAsset introductionFile;
    public TextAsset apartmentFile;       // Fallback
    public TextAsset apartmentStartM;     // Fallback Morning
    public TextAsset apartmentStartA;     // Fallback Afternoon
    public TextAsset apartmentStartE;     // Fallback Evening

    [Header("Locations")]
    public TextAsset parkFile;
    public TextAsset mallFile;
    public TextAsset libraryFile;
    public TextAsset bowlingAlleyFile;
    public TextAsset arcadeFile;
    public TextAsset universityFile;
    public TextAsset simonUniversityFile;

    [Header("Bar & Events")]
    public TextAsset barFile;
    public TextAsset barEveningFile;
    public TextAsset barAfterDartsFile;
    public TextAsset barAfterAmazingKaraokeFile;
    public TextAsset barAfterGoodKaraokeFile;
    public TextAsset barAfterOKKaraokeFile;
    public TextAsset barAfterTerribleKaraokeFile;
    public TextAsset barAfterHorrendousKaraokeFile;

    [Header("Movies")]
    public TextAsset moviesFile;
    public TextAsset moviesHJFile;
    public TextAsset moviesADFile;
    public TextAsset moviesHSFile;
    public TextAsset moviesFileTwo;
    public TextAsset moviesFileThree;
    public TextAsset moviesFileFour;
    public TextAsset moviesFileMei;

    [Header("Mei Hangouts")]
    public TextAsset meiHangOutOne;
    public TextAsset meiHangOutTwo;
    public TextAsset meiHangOutThree;
    public TextAsset meiHangOutFour;
    public TextAsset meiHangOutFive;
    public TextAsset meiHangOutSix;
    public TextAsset meiHangOutSeven;
    public TextAsset meiHangOutEight;
    public TextAsset meiHangOutNine;
    public TextAsset meiHangOutTen;

    [Header("Mei Activities")]
    public TextAsset poolFileMei;
    public TextAsset rehearsalFileMei;
    public TextAsset meiJogSpring;
    public TextAsset meiJogAfternoonSpring;
    public TextAsset meiEspressoMartini;
    public TextAsset meiMargarita;
    public TextAsset meiOldFashioned;
    public TextAsset meiShotOfTequila;
    public TextAsset meiRedWine;
    public TextAsset meiMojito;
    public TextAsset meiManhattan;
    public TextAsset meiHotToddy;
    public TextAsset meiWhiskeySour;
    public TextAsset meiNegroni;

    [Header("Nina Events & Hangouts")]
    public TextAsset barNina;
    public TextAsset ninaWalkHome;
    public TextAsset ninaHookUp;
    public TextAsset ninaCompany;
    public TextAsset ninaEightIntelligence;
    public TextAsset ninaEightEmpathy;
    public TextAsset ninaHangOutOne;
    public TextAsset ninaHangOutTwo;
    public TextAsset ninaHangOutThree;
    public TextAsset ninaHangOutFour;
    public TextAsset ninaHangOutFive;
    public TextAsset ninaHangOutSix;
    public TextAsset ninaHangOutSeven;
    public TextAsset ninaHangOutEight;
    public TextAsset ninaHangOutNine;
    public TextAsset ninaHangOutTen;

    [Header("Simon Events & Hangouts")]
    public TextAsset simonArcade;
    public TextAsset simonHangOutSix;
    public TextAsset simonHangOutSeven;

    [Header("After Cafe Events")]
    public TextAsset afterGoodCafeFile;
    public TextAsset afterGoodCafeALTFile;

    [Header("Convenience Store & Lottery")]
    public TextAsset convienceStoreFile;
    public TextAsset convienceStoreRandomKarmaEvent;
    public TextAsset convienceStoreRandomKarmaEventTwo;
    public TextAsset convienceStoreRandomKarmaEventThree;
    public TextAsset winningLuckBucks;
    public TextAsset winningRainbowRiches;
    public TextAsset winningMegaFortune;
    public TextAsset losingLotteryTicket;

    [Header("Drinks")]
    public TextAsset negroniFile;
    public TextAsset redWineFile;
    public TextAsset mojitoFile;
    public TextAsset manhattanFile;
    public TextAsset hotToddyFile;
    public TextAsset expressoMartiniFile;
    public TextAsset oldFashionedFile;
    public TextAsset whiskeySourFile;
    public TextAsset shotOfTequilaFile;
    public TextAsset margaritaFile;

    [Header("Jogging Events")]
    public TextAsset jogMorningSpring;
    public TextAsset jogAfternoonSpring;
    public TextAsset jogEveningSpring;
    public TextAsset simonJogAfternoonSpring;

    [Header("Special Files")]
    public TextAsset artOfConversation;
    public TextAsset welcomeToBaywoodPARTTWO;

    [Header("Mei Hangouts Part Two")]
    public TextAsset meiHangOutOnePARTTWO;
    public TextAsset meiHangOutTwoPARTTWO;
    public TextAsset meiHangOutThreePARTTWO;
    public TextAsset meiHangOutFourPARTTWO;
    public TextAsset meiHangOutFivePARTTWO;
    public TextAsset meiHangOutSixPARTTWO;
    public TextAsset meiHangOutSevenPARTTWO;
    public TextAsset meiHangOutEightPARTTWO;
    public TextAsset meiHangOutNinePARTTWO;
    public TextAsset meiHangOutTenPARTTWO;

    [Header("Alex Events")]
    public TextAsset alexCritterTrails;


    // Method to return the appropriate TextAsset based on the season and day
    public TextAsset GetStartingFile(CalendarSystem.Season season, int day, int activityCounter)
    {
        switch (season)
        {
            /*case CalendarSystem.Season.Spring:
                if (day == 16 && activityCounter < 2)
                    return springFile; 
                    //return Resources.Load<TextAsset>("MeiHangOutOne"); // Load "MeiHangOutOne" for Spring, day 16
                return apartmentFile;*/

            case CalendarSystem.Season.Spring:
                if (day == 15) 
                    return introductionFile; 
                    //return Resources.Load<TextAsset>("Introduction"); // Load "MeiHangOutOne" for Summer, day 17
                return apartmentFile;
            /*case CalendarSystem.Season.Summer:
                if (day == 17)
                    return Resources.Load<TextAsset>("MeiHangOutOne"); // Load "MeiHangOutOne" for Summer, day 17
                return apartmentFile;

            case CalendarSystem.Season.Fall: 
                if (day == 23)
                    return Resources.Load<TextAsset>("MeiHangOutOne"); // Load "MeiHangOutOne" for Fall, day 23
                return apartmentFile;

            case CalendarSystem.Season.Winter:
                if (day == 25)
                    return Resources.Load<TextAsset>("MeiHangOutOne"); // Load "MeiHangOutOne" for Winter, day 25
                return apartmentFile; 

            case CalendarSystem.Season.SpringA:
                if (day == 12)
                    return Resources.Load<TextAsset>("MeiHangOutOne"); // Load "MeiHangOutOne" for SpringA, day 12
                return apartmentFile; */ 

            default:
                return apartmentFile; // If none of the specific dates match, return the Apartment file
        }
    }
}
