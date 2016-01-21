using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// Controller of the level container that holds all safes.
/// </summary>
public class Level : MonoBehaviour
{

    public GameObject safePrefab;
    public LevelGenerationOptions generationOptions = new LevelGenerationOptions();
    public LevelChallengePrefabs challengePrefabs = new LevelChallengePrefabs();
    
    public GameObject Player1Cam;
    public GameObject Player2Cam;
    public GameObject[] neuroToxinParticleSystem;
    public GameObject[] safeContentPrefabs;
    
    public Sprite[] iconSprites = new Sprite[0];
    public float[] iconSpriteRotations = new float[0];

    [HideInInspector]
    public List<Safe> safes;
    [HideInInspector]
    public int currentLiveCount;

    void Awake()
    {
        if(iconSprites.Length != iconSpriteRotations.Length)
        {
            Debug.LogError("Config error: Icon Sprites and Icon Sprite Rotations have to be the same length!");
            return;
        }

        currentLiveCount = generationOptions.maxLives;
        GenerateLevel();
    }

    private void GenerateLevel()
    {
		// get safe-bounds
        Bounds bounds = safePrefab.GetComponent<Safe>().GetFrameBounds();
        float safeWidth = bounds.size.x;
        float safeHeight = bounds.size.y;

		// calculate base offset
        int rows = Mathf.CeilToInt(generationOptions.safesToGenerate / (float)generationOptions.safesPerRow);
        Vector3 baseOffset = new Vector3((-generationOptions.safesPerRow + 1) * safeWidth / 2, rows * safeHeight - safeHeight / 2, 0);
		Vector3 offset;
        
		// create safe gameobjects and logic
		safes = new List<Safe>();
        int currentlyActiveSafeCount = 0;
        for (int i = 0; i < generationOptions.safesToGenerate; i++)
        {
			// create GameObject
            var go = Instantiate(safePrefab, Vector3.zero, Quaternion.identity) as GameObject;
            go.transform.SetParent(transform);
            go.name = "Safe " + i;

            // offset from top left position
            offset = new Vector3(safeWidth * (i % generationOptions.safesPerRow), -safeHeight * (int)(i / generationOptions.safesPerRow), 0);
            go.transform.localPosition = baseOffset + offset;

            //spawn in a content for the safe
            GameObject.Instantiate(safeContentPrefabs[UnityEngine.Random.Range(0,safeContentPrefabs.Length)], go.transform.FindChild("safeContent").position,go.transform.FindChild("safeContent").rotation);

			// choose challenge
            Type challengeType = generationOptions.availableChallenges[UnityEngine.Random.Range(0, generationOptions.availableChallenges.Length)];
            AbstractChallenge challenge = (AbstractChallenge)go.AddComponent(challengeType);

			// find challenge-prefabs
            GameObject frontPrefab = challengePrefabs.GetFrontPrefab(challengeType);
            GameObject backPrefab = challengePrefabs.GetBackPrefab(challengeType);
            bool decoratedBack = challengePrefabs.HasDecoratedBack(challengeType);

			// initialize safe logic
			Safe safe = go.GetComponent<Safe>();
			safe.challenge = challenge;   
            safe.SetMaxTimer(UnityEngine.Random.Range(30, 61));
            safe.SpawnChallengeObjects(frontPrefab, backPrefab, decoratedBack);

            safe.SetBackwards(i % 2 == 1);
            // this is a possible alternative algorithm where the left half of the wall is front-facing
            // and the right half is back-facing (uneven safes-per-row numbers alternate front/back in the middle
            //int spr = generationOptions.safesPerRow;
            //safe.SetBackwards(i % spr >= (spr / 2) || (spr % 2 == 1 && i % spr == (spr / 2) && (i / spr % 2) == 1));

            bool shouldActivate;
            if (generationOptions.activateRandomly)
            {
                float activeProbability = generationOptions.safesActiveAtStart / (float)generationOptions.safesToGenerate;
                shouldActivate = (currentlyActiveSafeCount < generationOptions.safesActiveAtStart && UnityEngine.Random.value < activeProbability);
            }
            else
                shouldActivate = (i < generationOptions.safesActiveAtStart);


            if (shouldActivate)
            {
                safe.SetActive(true);
                print(go.name+" is active at start");
                currentlyActiveSafeCount++;
            }
            else {
                safe.SetActive(false);
            }

            // get random colorgroup (should be improved at some point)
            Color color = generationOptions.colorGroups[UnityEngine.Random.Range(0, generationOptions.colorGroups.Length)];
            safe.SetDisplayColor(color);

			// set 1 safe to activate after solved challenge (should be improved at some point)
            safe.SetNumberOfSafesToActivate(1);

            safes.Add(safe);
        }

        if (generationOptions.activateRandomly)
        {
            //make sure we do actually have the specified amount of safes active
            while (currentlyActiveSafeCount < generationOptions.safesActiveAtStart)
            {
                int rI = UnityEngine.Random.Range(0, safes.Count);
                if (!safes[rI].IsActive)
                {
                    safes[rI].SetActive(true);
                    print(safes[rI].gameObject.name + " is active at start (by force)");
                    currentlyActiveSafeCount++;
                }
            }
        }
        print("currently active safes count: " + currentlyActiveSafeCount);
    }

    // activates [amount] new safes of the same color as [color]
    public void ActivateNewSafes(Color color, int amount)
    {
        if(generationOptions.activateRandomly)
        {
            List<Safe> candidates = safes.Where(s => !s.IsActive && !s.IsOpen && s.GetDisplayColor() == color).ToList();
            // activate [amount] random safes, but not more than still available
            int count = Mathf.Min(amount, candidates.Count);
            for(int i=0; i<count; i++)
            {
                int index = UnityEngine.Random.Range(0, candidates.Count);
                candidates[index].SetActive(true);
                print(candidates[index].gameObject.name + " was activated!");
                candidates.RemoveAt(index);
            }
        }
        else
        {
            // take the first (up to) [amount] inactive safes of that color and activate them
            var toActivate = safes.Where(s => !s.IsActive && !s.IsOpen && s.GetDisplayColor() == color).Take(amount);
            foreach (Safe safe in toActivate)
            {
                safe.SetActive(true);
                print(safe.gameObject.name + " was activated!");
            }
        }
    }

}
