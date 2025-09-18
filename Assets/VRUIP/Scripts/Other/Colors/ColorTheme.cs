using System;
using UnityEngine;

namespace VRUIP
{
    [Serializable]
    public class ColorTheme
    {
        public Color primaryColor; // Most intense (ex: Background)
        public Color secondaryColor; // Opposite of most intense (ex: Text)
        public Color thirdColor; // Less intense version of primary (ex: Button normal)
        public Color fourthColor; // Different color (ex: Button pressed)

        // Constructor
        public ColorTheme(Color primary, Color secondary, Color third, Color fourth)
        {
            primaryColor = primary;
            secondaryColor = secondary;
            thirdColor = third;
            fourthColor = fourth;
        }
    }
}
