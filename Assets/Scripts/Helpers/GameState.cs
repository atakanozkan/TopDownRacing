using System;

namespace TDR.Helpers
{
    public enum GameState
    {
        Default = 0,
        Playing = 1,
        Pause = 2,
        GameOver = 4,
        MainMenu = 5,
        Settings = 6,
        ControlBinding = 7,
    }
}