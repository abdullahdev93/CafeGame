using UnityEngine;

[CreateAssetMenu(fileName = "NewApartmentItem", menuName = "Game/ApartmentItem")]
public class ApartmentItemData : ScriptableObject
{
    public string itemName;

    [TextArea(3, 10)]
    public string itemDescription;

    public Sprite itemIcon; // Icon for the UI display

    public ItemType itemType; // Define the item type

    public enum ItemType
    {
        ArtOfConversation,
        CharmingForDummies,
        MagneticPresence,
        MasteringTheSocialGame,
        SmallTalkBigImpact, 
        TheJokeWhisperer,
        StandUpForDummies,
        WittyComebacksSarcasm,
        WhyWeLaugh,
        SatireSpotlight,
        ScribblesToMasterpieces,
        VisionaryWeekly,
        TheImaginationReport,
        TheCreativeMindset,
        TheCreatorsJournal,
        WalkingInTheirShoes,
        TheEmotionalCompass,
        KindWordsStrongBonds,
        HeartfeltStories,
        HowToUnderstandOthers 

    }
} 
