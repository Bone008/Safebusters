using UnityEngine;
using System;

/// <summary>
/// All parameters necessary to generate levels of varying difficulties.
/// </summary>
[Serializable]
public class LevelGenerationOptions
{
    public int safesToGenerate = 12;
    public int safesPerRow = 3;
    public int safesActiveAtStart = 5;  //Amount of active safes at start
    public bool activateRandomly = false;

    public Color[] colorGroups;
    public Type[] availableChallenges = { 
		typeof(ButtonChallenge),
		typeof(ButtonSequenceChallenge),
        typeof(InterconnectedDialChallenge),
		typeof(SlidersKnobsChallenge),
        typeof(SynchronousSlidersChallenge)
	};

    // will be extended with additional parameters and logic later
}
