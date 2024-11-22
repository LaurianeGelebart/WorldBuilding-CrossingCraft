using UnityEngine;
public static class Utils
{
    public static int BitToInt(int bit1, int bit2)
    {
        return (bit1 << 1) | bit2; // Combine les deux bits en un entier
    }

    public static Color GetNavajoWhite()
    {
        return new Color(255f / 255f, 222f / 255f, 173f / 255f);
    }

    public static Color GetLightSlateGray()
    {
        return new Color(119f / 255f, 136f / 255f, 153f / 255f);
    }

    public static Color GetLightSkyBlue()
    {
        return new Color(135f / 255f, 206f / 255f, 250f / 255f);
    }

    public static Color GetMediumSeaGreen()
    {
        return new Color(60f / 255f, 179f / 255f, 113f / 255f);
    }

}