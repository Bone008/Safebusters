using UnityEngine;
using System;
using System.Collections;
using System.Linq;

public class PasswordReelsChallenge : AbstractChallenge
{

    private const float MAX_ROTATION_SPEED = 120;

    private const int PASSWORD_LENGTH = 3;
    private static readonly string[] POSSIBLE_PASSWORDS = { "the", "and", "for", "are", "but", "not", "you", "all", "any", "can", "her", "was", "one", "our", "out", "day", "get", "has", "him", "his", "how", "man", "new", "now", "old", "see", "two", "way", "who", "boy", "did", "its", "let", "put", "say", "she", "too", "use", "dad", "mom", "act", "bar", "car", "dew", "eat", "far", "gym", "hey", "ink", "jet", "key", "log", "mad", "nap", "odd", "pal", "ram", "saw", "tan", "vet", "wet", "yap", "zoo", "run", "fun", "sun", "sea" };

    private string password;
    private SlotMachineReelController[] reels;

    protected override void InitChallenge()
    {
        reels = GetComponentsInChildren<SlotMachineReelController>();

        password = POSSIBLE_PASSWORDS[UnityEngine.Random.Range(0, POSSIBLE_PASSWORDS.Length)].ToUpper();

        char[] allLetters = new char[26];
        for (char let = 'A'; let <= 'Z'; let++)
            allLetters[let - 'A'] = let;

        for (int i = 0; i < PASSWORD_LENGTH; i++)
        {
            // fill reel with n slots with correct letter and n-1 other letters
            char correctLetter = password[i];
            var otherLetters = allLetters.Where(let => let != correctLetter).OrderBy(_ => UnityEngine.Random.value).Take(reels[i].GetSlotCount() - 1);
            reels[i].SetLetters(otherLetters.Concat(Enumerable.Repeat(correctLetter, 1)).OrderBy(let => let).ToArray());

            // start at random position
            reels[i].SetActiveLetterIndex(UnityEngine.Random.Range(0, reels[i].GetSlotCount()));
        }
    }

    void Update()
    {
        if (!hasFocusFront)
        {
            reels[0].SetRotationSpeed(0);
            reels[1].SetRotationSpeed(0);
            reels[2].SetRotationSpeed(0);
        }
        else
        {
            // check for win condition
            bool hasWon = true;
            for(int i=0; i<PASSWORD_LENGTH; i++)
            {
                if(reels[i].GetActiveLetter() != password[i])
                {
                    hasWon = false;
                    break;
                }
            }
            if(hasWon)
            {
                safe.SolveChallenge();
                return;
            }

            // set velocities
            float left = frontInputState.GetAnalogInput(GameAnalogInput.LeftSlider);
            float right = frontInputState.GetAnalogInput(GameAnalogInput.RightSlider);

            // limit to 6 concrete velocities:
            // -100%, -60%, -20%, 20%, 60%, 100%
            float velLeft = (Mathf.Ceil(left * 6) - 3.5f) / 2.5f;
            float velRight = (Mathf.Ceil(right * 6) - 3.5f) / 2.5f;

            reels[0].SetRotationSpeed(velLeft * MAX_ROTATION_SPEED);
            reels[1].SetRotationSpeed(0.6f * MAX_ROTATION_SPEED);
            reels[2].SetRotationSpeed(velRight * MAX_ROTATION_SPEED);
        }
    }
}
