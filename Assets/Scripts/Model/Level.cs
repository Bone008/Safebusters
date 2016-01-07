using System.Collections.Generic;

namespace Model
{
    public class Level
    {
        //public List<ColorGroup> ColorGroups { get { return new List<ColorGroup>(); } }
        public List<Safe> Safes { get; set; }
        public int SafesPerRow { get { return 4; } }

        public Level()
        {
            Safes = new List<Safe>();
            Safes.Add(new Safe() { IsOpen = true, IsActive = false });
            Safes.Add(new Safe());
            Safes.Add(new Safe());
            Safes.Add(new Safe() { IsActive = false });
            Safes.Add(new Safe());
            Safes.Add(new Safe());
            Safes.Add(new Safe() { IsBackwards = true });
            Safes.Add(new Safe());
            Safes.Add(new Safe());
            Safes.Add(new Safe() { IsBackwards = true });
        }
    }
}