using UnityEngine;

public static class Configs
{
    public static PlayerConfig Player => Resources.Load<PlayerConfig>(nameof(Player));
}
