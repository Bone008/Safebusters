using UnityEngine;

namespace Model
{
    public class ColorGroup
    {
        public static readonly ColorGroup Default = new ColorGroup { Id = 0, DisplayColor = Color.black };

        public int Id { get; private set; }
        public Color DisplayColor { get; private set; }
    }
}
