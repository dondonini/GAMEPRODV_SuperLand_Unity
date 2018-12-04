using UnityEngine;

public class PlayerData
{
    public enum Slots {
        None,
        Slot1,
        Slot2,
        Slot3,
    };

    public static Slots currentSlot;

    public static int masterStars;
    public static int stars;
    public static int coins;

    /************************************************************************/
    /* Slot Data Methods                                                    */
    /************************************************************************/

    public static void SaveSlotData()
    {
        if (currentSlot == Slots.None)
        {
            Debug.LogError("No slot has been selected!");
            return;
        }

        string key = currentSlot.ToString();
        string saveFileName = key + ".sls";

        // Unpack data
        ES3.Save<int>(key + "MASTER_STARS", masterStars,    saveFileName);
        ES3.Save<int>(key + "STARS",        stars,          saveFileName);
        ES3.Save<int>(key + "COINS",        coins,          saveFileName);
    }

    public static void LoadSlotData(Slots _slot)
    {
        // Set the current slot
        switch(_slot)
        {
            case Slots.Slot1:
                currentSlot = Slots.Slot1;
                break;
            case Slots.Slot2:
                currentSlot = Slots.Slot2;
                break;
            case Slots.Slot3:
                currentSlot = Slots.Slot3;
                break;
            default:
                currentSlot = Slots.None;
                break;
        }

        if (currentSlot == Slots.None)
        {
            Debug.LogError("No slot has been selected!");
            return;
        }

        string key = currentSlot.ToString();
        string saveFileName = key + ".sls";

        // Unpack data
        masterStars = ES3.Load<int>(key + "MASTER_STARS",   saveFileName);
        stars       = ES3.Load<int>(key + "STARS",          saveFileName);
        coins       = ES3.Load<int>(key + "COINS",          saveFileName);
    }

    public static void DeleteSlotData(Slots _slot)
    {
        string saveFileName;

        // Set the current slot
        switch(_slot)
        {
            case Slots.Slot1:
                saveFileName = Slots.Slot1.ToString() + ".sls";
                break;
            case Slots.Slot2:
                saveFileName = Slots.Slot2.ToString() + ".sls";
                break;
            case Slots.Slot3:
                saveFileName = Slots.Slot3.ToString() + ".sls";
                break;
            default:
                saveFileName = Slots.None.ToString() ;
                break;
        }

        // Check slot if it exists and delete it
        if (ES3.FileExists(saveFileName))
        {
            ES3.DeleteFile(saveFileName);
        }
    }
}
