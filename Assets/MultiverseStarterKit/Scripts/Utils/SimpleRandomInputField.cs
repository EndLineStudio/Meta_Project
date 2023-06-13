using UnityEngine;

public static class SimpleRandomInputField
{
    private const string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    public static string GenerateRandomText(int length)
    {
        string randomText = "";

        for (int i = 0; i < length; i++)
        {
            int randomIndex = Random.Range(0, characters.Length);
            randomText += characters[randomIndex];
        }

        return randomText;
    }
}
