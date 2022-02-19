using System;

namespace StrAItego.Game
{
    partial class Board {
        // This file used to contain a lot of things that are now in EnumExtensions.

        static readonly Square[] _squareTable = new Square[]
        { Square.A1,  Square.B1,  Square.C1,  Square.D1,  Square.E1,  Square.F1,  Square.G1,  Square.H1,  Square.J1,  Square.K1,
          Square.A2,  Square.B2,  Square.C2,  Square.D2,  Square.E2,  Square.F2,  Square.G2,  Square.H2,  Square.J2,  Square.K2,
          Square.A3,  Square.B3,  Square.C3,  Square.D3,  Square.E3,  Square.F3,  Square.G3,  Square.H3,  Square.J3,  Square.K3,
          Square.A4,  Square.B4,  Square.C4,  Square.D4,  Square.E4,  Square.F4,  Square.G4,  Square.H4,  Square.J4,  Square.K4,
          Square.A5,  Square.B5,  Square.None, Square.None, Square.E5,  Square.F5, Square.None, Square.None, Square.J5,  Square.K5,
          Square.A6,  Square.B6,  Square.None, Square.None, Square.E6,  Square.F6, Square.None, Square.None, Square.J6,  Square.K6,
          Square.A7,  Square.B7,  Square.C7,  Square.D7,  Square.E7,  Square.F7,  Square.G7,  Square.H7,  Square.J7,  Square.K7,
          Square.A8,  Square.B8,  Square.C8,  Square.D8,  Square.E8,  Square.F8,  Square.G8,  Square.H8,  Square.J8,  Square.K8,
          Square.A9,  Square.B9,  Square.C9,  Square.D9,  Square.E9,  Square.F9,  Square.G9,  Square.H9,  Square.J9,  Square.K9,
          Square.A10, Square.B10, Square.C10, Square.D10, Square.E10, Square.F10, Square.G10, Square.H10, Square.J10, Square.K10
        };
    }

    public enum Outcome : byte
    {
        Defeat,
        Tie,
        Victory
    }

    public enum Unit : byte
    {
        None,
        RedFlag,
        RedSpy,
        RedScout,
        RedMiner,
        RedSergeant,
        RedLieutenant,
        RedCaptain,
        RedMajor,
        RedColonel,
        RedGeneral,
        RedMarshal,
        RedBomb,
        BlueFlag,
        BlueSpy,
        BlueScout,
        BlueMiner,
        BlueSergeant,
        BlueLieutenant,
        BlueCaptain,
        BlueMajor,
        BlueColonel,
        BlueGeneral,
        BlueMarshal,
        BlueBomb
    }

    [Flags]
    public enum PotentialRank : int
    {
        // This enum works as a bitfield.
        // Each bit represents whether or not a unit could possibly be of a certain rank. The first bit marks the team.
        // [FLAG][SPY][SCOUT][MINER][SERGEANT][LIEUTENANT][CAPTAIN][MAJOR][COLONEL][GENERAL][MARSHAL][BOMB]
        // This enum makes it easier to work with this format.
        None = 0,
        Bomb = 1,
        Marshal = 2,
        General = 4,
        Colonel = 8,
        Major = 16,
        Captain = 32,
        Lieutenant = 64,
        Sergeant = 128,
        Miner = 256,
        Scout = 512,
        Spy = 1024,
        NotBombOrFlag = 2046,
        Flag = 2048,
        BombOrFlag = 2049,
        NotFlag = 2047,
        NotSpy = 3071,
        NotScout = 3583,
        NotMiner = 3839,
        NotSergeant = 3967,
        NotLieutenant = 4031,
        NotCaptain = 4063,
        NotMajor = 4079,
        NotColonel = 4087,
        NotGeneral = 4091,
        NotMarshal = 4093,
        NotBomb = 4094,
        Any = 4095
    }

    public enum Rank : byte
    {
        None,
        Flag,
        Spy,
        Scout,
        Miner,
        Sergeant,
        Lieutenant,
        Captain,
        Major,
        Colonel,
        General,
        Marshal,
        Bomb
    }

    public enum Team : int
    {
        Red, Blue, Neither, Both
    }

    public enum Square : int
    {
        // Representation in text is vertically flipped (e.g. North = Down)
        A1, B1, C1, D1, E1, F1, G1, H1, J1, K1, // 0-39
        A2, B2, C2, D2, E2, F2, G2, H2, J2, K2,
        A3, B3, C3, D3, E3, F3, G3, H3, J3, K3,
        A4, B4, C4, D4, E4, F4, G4, H4, J4, K4,
        A5, B5, E5, F5, J5, K5, // 40-51
        A6, B6, E6, F6, J6, K6,
        A7, B7, C7, D7, E7, F7, G7, H7, J7, K7, // 52-91
        A8, B8, C8, D8, E8, F8, G8, H8, J8, K8,
        A9, B9, C9, D9, E9, F9, G9, H9, J9, K9,
        A10, B10, C10, D10, E10, F10, G10, H10, J10, K10,
        None                                             // 92 (out of bounds)
    }

    public enum Direction : int
    {
        North, East, South, West
    }
}
