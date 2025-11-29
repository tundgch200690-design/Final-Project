using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayer", menuName = "Betrayal/Player")]
public class Player : ScriptableObject
{
    [Header("Player Information")]
    public string playerName;
    public Sprite avatar;

    [Header("Player Stats")]
    public int might = 4;
    public int speed = 4;
    public int sanity = 4;
    public int knowledge = 4;

    public void SetStat(string statName, int value)
    {
        switch (statName.ToLower())
        {
            case "might": might = Mathf.Max(0, value); break;
            case "speed": speed = Mathf.Max(0, value); break;
            case "sanity": sanity = Mathf.Max(0, value); break;
            case "knowledge": knowledge = Mathf.Max(0, value); break;
        }
    }

    public void ResetStats()
    {
        might = speed = sanity = knowledge = 4;
    }
}