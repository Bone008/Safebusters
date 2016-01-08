using System.Collections.Generic;
using Model.Challenges;
using UnityEngine;

namespace Model
{
    public class Level : MonoBehaviour
    {
        public int SAFE_COUNT = 12;

        //public List<ColorGroup> ColorGroups { get { return new List<ColorGroup>(); } }
		public Color[] ColorGroups { get; set; }
		public List<Safe> Safes { get; set; }
        public int SafesPerRow { get { return 4; } }

        public Level()
        {
			// init color groups
			ColorGroups = new Color[] {
				new Color (0.5f, 0.7f, 0.6f),
				new Color (0.7f, 0.7f, 0.5f)
			};

			// create empty list of safes
            Safes = new List<Safe>();
        }

        public Safe CreateSafeScriptOnGO(GameObject currentSafe) {
            // get challenge
            IChallenge challenge;
            if (Random.value > 0.3)
                challenge = new ButtonChallenge();
            else
                challenge = new PlaceholderChallenge();

            // get random colorgroup
            Color color = ColorGroups[Random.Range(0, ColorGroups.Length)];

            // get number of safes to activate
            int numberToActivate = 1; // complicated algorithm

            Safe instance = currentSafe.AddComponent<Safe>();

            instance.Challenge = challenge;
            instance.DisplayColor = color;
            instance.NumberOfSafesToActivate = numberToActivate;
            instance.IsActive = Random.value > 0.2f; /* remove later */
            instance.IsOpen = Random.value < 0.1f; /* remove later */
            instance.IsBackwards = (Random.value > 0.5f);
            // add new safe to list
            Safes.Add(instance);
            return instance;
        }
    }
}