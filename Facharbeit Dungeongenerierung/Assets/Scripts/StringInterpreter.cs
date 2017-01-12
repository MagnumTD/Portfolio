using UnityEngine;
using System.Collections;

public class StringInterpreter
{
    #region ConstantSigns
    public const char Start = 'S';
    public const char GenericRoom = 'R';
    public const char GenericHallway = 'G';

    public const char EntranceRoom = 'e';
    public const char GoalRoom = 'z';
    public const char EmptyRoom = 'r';
    public const char LockedRoom = 'v';
    public const char KeyRoom = 's';
    public const char LeftCurve = '+';
    public const char RightCurve = '-';
    #endregion

    #region ConstantRules

    private string[] Rule = new string[10] 
    {
         "" + EntranceRoom + GenericRoom + GenericRoom + GenericRoom + GoalRoom,
         "" + EmptyRoom + GenericRoom,
         "" + '[' + RightCurve + GenericHallway + KeyRoom + ']' + LockedRoom + GenericRoom,
         //"" + RightCurve + '[' + GenericHallway + KeyRoom + ']' + LockedRoom + GenericRoom,
         "" + EmptyRoom,
         "" + '[' + LeftCurve + GenericHallway + KeyRoom + ']' + LockedRoom + GenericRoom,
         //"" + LeftCurve + '[' + GenericHallway + KeyRoom + ']' + LockedRoom + GenericRoom,
         "" + LeftCurve + GenericRoom,
         "" + RightCurve + GenericRoom,
         "" + EmptyRoom + GenericHallway,
         "" + EmptyRoom,
         "" + GenericRoom,
    };

    private int[] RuleChance = new int[10]
    {
        /* Start Rule */ 100,
        /* GenericRoom */
        20,
        20,
        20,
        20,
        10,
        10,
        /* Generic Hallway */
        50,
        45,
        5
    };


    #endregion

    public static string IterateComplete(string currentWord)
    {
        // Static function, so no instance of the class is needed in the contextclass. Calls iterate until there are no generics left and every char is terminal
        var stringInterpreter = new StringInterpreter();
        string newWord = stringInterpreter.Iterate(currentWord);
        while (newWord.Contains(Start.ToString()) || newWord.Contains(GenericHallway.ToString()) || newWord.Contains(GenericRoom.ToString()))
        {
            newWord = (stringInterpreter.Iterate(newWord));
        }

        return newWord;
    }


    private string Iterate(string currentWord)
    {
        // Creates new empty string, then checks each char in the given string and applies rules to it, then adds it to the new word
        string newWord = "";
        foreach (char c in currentWord)
        {
            switch (c)
            {
                case Start:
                {
                    newWord = Rule[0];
                    break;
                }
                case GenericRoom:
                {
                    newWord += GenericRoomRules();
                    break;
                }
                case GenericHallway:
                {
                    newWord += GenericHallwayRules();
                    break;
                }
                default:
                {
                    newWord += c;
                    break;
                }
            }
        }

        return newWord;
    }

    private string GenericRoomRules()
    {
        // Replacementrules for the generic room, creates a number between 0 and 100 and then decides, which rule is to be applied
        int randomPercent = Random.Range(0, 100);
        if (randomPercent < RuleChance[1]) return Rule[1];
        else if (randomPercent < RuleChance[1] + RuleChance[2]) return Rule[2];
        else if (randomPercent < RuleChance[1] + RuleChance[2] + RuleChance[3]) return Rule[3];
        else if (randomPercent < RuleChance[1] + RuleChance[2] + RuleChance[3] + RuleChance[4]) return Rule[4];
        else if (randomPercent < RuleChance[1] + RuleChance[2] + RuleChance[3] + RuleChance[4] + RuleChance[5]) return Rule[5];
        else if (randomPercent < RuleChance[1] + RuleChance[2] + RuleChance[3] + RuleChance[4] + RuleChance[5] + RuleChance[6]) return Rule[6];
        return "";
    }

    private string GenericHallwayRules()
    {        
        // Replacementrules for the generic hallway, creates a number between 0 and 100 and then decides, which rule is to be applied
        int randomPercent = Random.Range(0, 100);
        if (randomPercent < RuleChance[7]) return Rule[7];
        else if (randomPercent < RuleChance[7] + RuleChance[8]) return Rule[8];
        else if (randomPercent < RuleChance[7] + RuleChance[8] + RuleChance[9]) return Rule[9];
        return "";
    }
}
