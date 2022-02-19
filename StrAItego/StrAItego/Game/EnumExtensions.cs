using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrAItego.Game
{
    public static class EnumExtensions
    {
        #region Static arrays
        // Format: [Square][Direction] NESW
        static readonly Square[][] _adjacencyTable = {
            new Square[] {Square.A2,   Square.B1,   Square.None, Square.None },  //A1
            new Square[] {Square.B2,   Square.C1,   Square.None, Square.A1   },  //B1
            new Square[] {Square.C2,   Square.D1,   Square.None, Square.B1   },  //C1
            new Square[] {Square.D2,   Square.E1,   Square.None, Square.C1   },  //D1
            new Square[] {Square.E2,   Square.F1,   Square.None, Square.D1   },  //E1
            new Square[] {Square.F2,   Square.G1,   Square.None, Square.E1   },  //F1
            new Square[] {Square.G2,   Square.H1,   Square.None, Square.F1   },  //G1
            new Square[] {Square.H2,   Square.J1,   Square.None, Square.G1   },  //H1
            new Square[] {Square.J2,   Square.K1,   Square.None, Square.H1   },  //J1
            new Square[] {Square.K2,   Square.None, Square.None, Square.J1   },  //K1

            new Square[] {Square.A3,   Square.B2,   Square.A1,   Square.None },  //A2
            new Square[] {Square.B3,   Square.C2,   Square.B1,   Square.A2   },  //B2
            new Square[] {Square.C3,   Square.D2,   Square.C1,   Square.B2   },  //C2
            new Square[] {Square.D3,   Square.E2,   Square.D1,   Square.C2   },  //D2
            new Square[] {Square.E3,   Square.F2,   Square.E1,   Square.D2   },  //E2
            new Square[] {Square.F3,   Square.G2,   Square.F1,   Square.E2   },  //F2
            new Square[] {Square.G3,   Square.H2,   Square.G1,   Square.F2   },  //G2
            new Square[] {Square.H3,   Square.J2,   Square.H1,   Square.G2   },  //H2
            new Square[] {Square.J3,   Square.K2,   Square.J1,   Square.H2   },  //J2
            new Square[] {Square.K3,   Square.None, Square.K1,   Square.J2   },  //K2

            new Square[] {Square.A4,   Square.B3,   Square.A2,   Square.None },  //A3
            new Square[] {Square.B4,   Square.C3,   Square.B2,   Square.A3   },  //B3
            new Square[] {Square.C4,   Square.D3,   Square.C2,   Square.B3   },  //C3
            new Square[] {Square.D4,   Square.E3,   Square.D2,   Square.C3   },  //D3
            new Square[] {Square.E4,   Square.F3,   Square.E2,   Square.D3   },  //E3
            new Square[] {Square.F4,   Square.G3,   Square.F2,   Square.E3   },  //F3
            new Square[] {Square.G4,   Square.H3,   Square.G2,   Square.F3   },  //G3
            new Square[] {Square.H4,   Square.J3,   Square.H2,   Square.G3   },  //H3
            new Square[] {Square.J4,   Square.K3,   Square.J2,   Square.H3   },  //J3
            new Square[] {Square.K4,   Square.None, Square.K2,   Square.J3   },  //K3

            new Square[] {Square.A5,   Square.B4,   Square.A3,   Square.None },  //A4
            new Square[] {Square.B5,   Square.C4,   Square.B3,   Square.A4   },  //B4
            new Square[] {Square.None, Square.D4,   Square.C3,   Square.B4   },  //C4
            new Square[] {Square.None, Square.E4,   Square.D3,   Square.C4   },  //D4
            new Square[] {Square.E5,   Square.F4,   Square.E3,   Square.D4   },  //E4
            new Square[] {Square.F5,   Square.G4,   Square.F3,   Square.E4   },  //F4
            new Square[] {Square.None, Square.H4,   Square.G3,   Square.F4   },  //G4
            new Square[] {Square.None, Square.J4,   Square.H3,   Square.G4   },  //H4
            new Square[] {Square.J5,   Square.K4,   Square.J3,   Square.H4   },  //J4
            new Square[] {Square.K5,   Square.None, Square.K3,   Square.J4   },  //K4

            new Square[] {Square.A6,   Square.B5,   Square.A4,   Square.None },  //A5
            new Square[] {Square.B6,   Square.None, Square.B4,   Square.A5   },  //B5
            new Square[] {Square.E6,   Square.F5,   Square.E4,   Square.None },  //E5
            new Square[] {Square.F6,   Square.None, Square.F4,   Square.E5   },  //F5
            new Square[] {Square.J6,   Square.K5,   Square.J4,   Square.None },  //J5
            new Square[] {Square.K6,   Square.None, Square.K4,   Square.J5   },  //K5

            new Square[] {Square.A7,   Square.B6,   Square.A5,   Square.None },  //A6
            new Square[] {Square.B7,   Square.None, Square.B5,   Square.A6   },  //B6
            new Square[] {Square.E7,   Square.F6,   Square.E5,   Square.None },  //E6
            new Square[] {Square.F7,   Square.None, Square.F5,   Square.E6   },  //F6
            new Square[] {Square.J7,   Square.K6,   Square.J5,   Square.None },  //J6
            new Square[] {Square.K7,   Square.None, Square.K5,   Square.J6   },  //K6

            new Square[] {Square.A8,   Square.B7,   Square.A6,   Square.None },  //A7
            new Square[] {Square.B8,   Square.C7,   Square.B6,   Square.A7   },  //B7
            new Square[] {Square.C8,   Square.D7,   Square.None, Square.B7   },  //C7
            new Square[] {Square.D8,   Square.E7,   Square.None, Square.C7   },  //D7
            new Square[] {Square.E8,   Square.F7,   Square.E6,   Square.D7   },  //E7
            new Square[] {Square.F8,   Square.G7,   Square.F6,   Square.E7   },  //F7
            new Square[] {Square.G8,   Square.H7,   Square.None, Square.F7   },  //G7
            new Square[] {Square.H8,   Square.J7,   Square.None, Square.G7   },  //H7
            new Square[] {Square.J8,   Square.K7,   Square.J6,   Square.H7   },  //J7
            new Square[] {Square.K8,   Square.None, Square.K6,   Square.J7   },  //K7

            new Square[] {Square.A9,   Square.B8,   Square.A7,   Square.None },  //A8
            new Square[] {Square.B9,   Square.C8,   Square.B7,   Square.A8   },  //B8
            new Square[] {Square.C9,   Square.D8,   Square.C7,   Square.B8   },  //C8
            new Square[] {Square.D9,   Square.E8,   Square.D7,   Square.C8   },  //D8
            new Square[] {Square.E9,   Square.F8,   Square.E7,   Square.D8   },  //E8
            new Square[] {Square.F9,   Square.G8,   Square.F7,   Square.E8   },  //F8
            new Square[] {Square.G9,   Square.H8,   Square.G7,   Square.F8   },  //G8
            new Square[] {Square.H9,   Square.J8,   Square.H7,   Square.G8   },  //H8
            new Square[] {Square.J9,   Square.K8,   Square.J7,   Square.H8   },  //J8
            new Square[] {Square.K9,   Square.None, Square.K7,   Square.J8   },  //K8

            new Square[] {Square.A10,  Square.B9,   Square.A8,   Square.None },  //A9
            new Square[] {Square.B10,  Square.C9,   Square.B8,   Square.A9   },  //B9
            new Square[] {Square.C10,  Square.D9,   Square.C8,   Square.B9   },  //C9
            new Square[] {Square.D10,  Square.E9,   Square.D8,   Square.C9   },  //D9
            new Square[] {Square.E10,  Square.F9,   Square.E8,   Square.D9   },  //E9
            new Square[] {Square.F10,  Square.G9,   Square.F8,   Square.E9   },  //F9
            new Square[] {Square.G10,  Square.H9,   Square.G8,   Square.F9   },  //G9
            new Square[] {Square.H10,  Square.J9,   Square.H8,   Square.G9   },  //H9
            new Square[] {Square.J10,  Square.K9,   Square.J8,   Square.H9   },  //J9
            new Square[] {Square.K10,  Square.None, Square.K8,   Square.J9   },  //K9

            new Square[] {Square.None, Square.B10,  Square.A9,   Square.None },  //A10
            new Square[] {Square.None, Square.C10,  Square.B9,   Square.A10  },  //B10
            new Square[] {Square.None, Square.D10,  Square.C9,   Square.B10  },  //C10
            new Square[] {Square.None, Square.E10,  Square.D9,   Square.C10  },  //D10
            new Square[] {Square.None, Square.F10,  Square.E9,   Square.D10  },  //E10
            new Square[] {Square.None, Square.G10,  Square.F9,   Square.E10  },  //F10
            new Square[] {Square.None, Square.H10,  Square.G9,   Square.F10  },  //G10
            new Square[] {Square.None, Square.J10,  Square.H9,   Square.G10  },  //H10
            new Square[] {Square.None, Square.K10,  Square.J9,   Square.H10  },  //J10
            new Square[] {Square.None, Square.None, Square.K9,   Square.J10  }   //K10

        };

        static readonly int[] _rowTable = new int[]
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
         1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
         2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
         3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
         4, 4,       4, 4,       4, 4,
         5, 5,       5, 5,       5, 5,
         6, 6, 6, 6, 6, 6, 6, 6, 6, 6,
         7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
         8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
         9, 9, 9, 9, 9, 9, 9, 9, 9, 9};

        static readonly int[] _columnTable = new int[]
        {0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
         0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
         0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
         0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
         0, 1,       4, 5,       8, 9,
         0, 1,       4, 5,       8, 9,
         0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
         0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
         0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
         0, 1, 2, 3, 4, 5, 6, 7, 8, 9};

        static readonly int[] _originalOfRank = new int[] { 0, 1, 1, 8, 5, 4, 4, 4, 3, 2, 1, 1, 6 };
        #endregion

        /// <summary>
        /// Get the row that Square s is in.
        /// </summary>
        public static int Row(this Square s)
        {
            return _rowTable[(int)s];
        }

        /// <summary>
        /// Get the column that Square s is in.
        /// </summary>
        public static int Column(this Square s)
        {
            return _columnTable[(int)s];
        }

        /// <summary>
        /// Returns the square adjacent to this one in a specific Direction d.
        /// </summary>
        public static Square AdjacentSquare(this Square s, Direction d) => _adjacencyTable[(int)s][(int)d];

        /// <summary>
        /// Returns all squares adjacent to this one.
        /// </summary>
        public static Square[] AdjacentSquares(this Square s) => _adjacencyTable[(int)s];

        /// <summary>
        /// Determines the outcome of an attack where this Piece attacks Piece defender.
        /// </summary>
        public static Outcome Attacks(this Piece attacker, Piece defender)
        {
            if (attacker == Rank.Spy && defender == Rank.Marshal)
                return Outcome.Victory;    // Attacking Spy beats Marshal

            if (attacker == Rank.Miner && defender == Rank.Bomb)
                return Outcome.Victory;    // Attacking Miner beats Bomb

            return attacker >  defender ? Outcome.Victory : // If a's rank is higher, then it can beat d.
                   attacker == defender ? Outcome.Tie :     // If ranks are equal, it's a tie.
                                          Outcome.Defeat;   // Otherwise, defeat.
        }

        /// <summary>
        /// Returns the number of pieces of a specific rank are originally on a Stratego board.
        /// </summary>
        public static int OriginalOfRank(this Rank r) => _originalOfRank[(int)r];

        /// <summary>
        /// Convert PotentialRank to a single rank, and throw if not possible.
        /// <br>Also converts negative PotentialRanks, e.g. NotFlag => Flag.</br>
        /// </summary>
        public static Rank ToRank(this PotentialRank x)
        {
            return x switch
            {
                PotentialRank.Bomb => Rank.Bomb,
                PotentialRank.Captain => Rank.Captain,
                PotentialRank.Colonel => Rank.Colonel,
                PotentialRank.Flag => Rank.Flag,
                PotentialRank.General => Rank.General,
                PotentialRank.Lieutenant => Rank.Lieutenant,
                PotentialRank.Major => Rank.Major,
                PotentialRank.Marshal => Rank.Marshal,
                PotentialRank.Miner => Rank.Miner,
                PotentialRank.Scout => Rank.Scout,
                PotentialRank.Sergeant => Rank.Sergeant,
                PotentialRank.Spy => Rank.Spy,
                PotentialRank.NotBomb => Rank.Bomb,
                PotentialRank.NotCaptain => Rank.Captain,
                PotentialRank.NotColonel => Rank.Colonel,
                PotentialRank.NotFlag => Rank.Flag,
                PotentialRank.NotGeneral => Rank.General,
                PotentialRank.NotLieutenant => Rank.Lieutenant,
                PotentialRank.NotMajor => Rank.Major,
                PotentialRank.NotMarshal => Rank.Marshal,
                PotentialRank.NotMiner => Rank.Miner,
                PotentialRank.NotScout => Rank.Scout,
                PotentialRank.NotSergeant => Rank.Sergeant,
                PotentialRank.NotSpy => Rank.Spy,
                PotentialRank.None => Rank.None,
                _ => throw new ArgumentException("Unknown update info " + x.ToString() + " (" + (int)x + ")")
            };
        }

        /// <summary>
        /// Convert rank to a PotentialRank of only this rank.
        /// </summary>
        public static PotentialRank ToPotentialRank(this Rank x)
        {
            return x switch
            {
                Rank.Bomb => PotentialRank.Bomb,
                Rank.Captain => PotentialRank.Captain,
                Rank.Colonel => PotentialRank.Colonel,
                Rank.Flag => PotentialRank.Flag,
                Rank.General => PotentialRank.General,
                Rank.Lieutenant => PotentialRank.Lieutenant,
                Rank.Major => PotentialRank.Major,
                Rank.Marshal => PotentialRank.Marshal,
                Rank.Miner => PotentialRank.Miner,
                Rank.Scout => PotentialRank.Scout,
                Rank.Sergeant => PotentialRank.Sergeant,
                Rank.Spy => PotentialRank.Spy,
                Rank.None => PotentialRank.None,
                _ => throw new ArgumentException("Unknown rank " + x.ToString() + " (" + (int)x + ")")
            };
        }

        /// <summary>
        /// Convert piece to a two-letter string.
        /// </summary>
        public static string UnitToString(this Piece p)
        {
            if (p == null)
                return "  ";
            if (p.Team == Team.Red)
                return "R" + p.Rank.ToString();
            else
                return "B" + p.Rank.ToString();
        }

        /// <summary>
        /// Convert Rank to a single letter string.
        /// </summary>
        public static string ToString(this Rank r)
        {
            return r switch
            {
                Rank.None => " ",
                Rank.Flag => "F",
                Rank.Spy => "1",
                Rank.Scout => "2",
                Rank.Miner => "3",
                Rank.Sergeant => "4",
                Rank.Lieutenant => "5",
                Rank.Captain => "6",
                Rank.Major => "7",
                Rank.Colonel => "8",
                Rank.General => "9",
                Rank.Marshal => "M",
                Rank.Bomb => "B",
                _ => throw new ArgumentException("Unknown rank " + r.ToString() + " (" + (int)r + ")")
            };
        }

        /// <summary>
        /// True if a piece with this PotentialRank could kill a piece of Rank rank.
        /// </summary>
        public static bool CouldKill(this PotentialRank pr, Rank rank)
        {
            if (pr == PotentialRank.None)
                return false;

            switch (rank)
            {
                case Rank.Flag:
                    return true;
                case Rank.Bomb:
                    return (pr & PotentialRank.Miner) > 0;
                case Rank.Marshal:
                    return (pr & (PotentialRank.Marshal & PotentialRank.Spy)) > 0;
                default:
                    PotentialRank cankill = (rank + 1).ToPotentialRank() - 1;
                    return (pr & cankill) > 0;
            }
        }

        /// <summary>
        /// True if a piece with this PotentialRank could kill a piece of Rank rank without killing itself.
        /// </summary>
        public static bool CouldKillSafely(this PotentialRank pr, Rank rank)
        {
            if (pr == PotentialRank.None)
                return false;

            switch (rank)
            {
                case Rank.Flag:
                    return true;
                case Rank.Bomb:
                    return (pr & PotentialRank.Miner) > 0;
                case Rank.Marshal:
                    return (pr & PotentialRank.Spy) > 0;
                default:
                    PotentialRank cankill = rank.ToPotentialRank() - 1;
                    return (pr & cankill) > 0;
            }
        }

        /// <summary>
        /// True if a given Rank will kill against a piece that has PotentialRank pr.
        /// </summary>
        public static bool WillKill(this Rank rank, PotentialRank pr)
        {
            return rank switch
            {
                Rank.Flag => false,
                Rank.Bomb => false,
                Rank.Miner => (pr & (PotentialRank.NotScout & PotentialRank.NotBomb & PotentialRank.NotFlag)) == 0,
                Rank.Spy => (pr & PotentialRank.NotMarshal) == 0,
                _ => (pr & (rank.ToPotentialRank() - 1)) == 0,
            };
        }

        /// <summary>
        /// True if a given Rank will kill or suicide against a piece that has PotentialRank pr.
        /// </summary>
        public static bool WillKillOrSuicide(this Rank rank, PotentialRank pr)
        {
            return rank switch
            {
                Rank.Flag => false,
                Rank.Bomb => false,
                Rank.Miner => (pr & (PotentialRank.NotScout & PotentialRank.NotMiner & PotentialRank.NotBomb & PotentialRank.NotFlag)) == 0,
                Rank.Spy => (pr & PotentialRank.NotMarshal & PotentialRank.NotSpy) == 0,
                _ => (pr & ((rank + 1).ToPotentialRank() - 1)) == 0,
            };
        }

        /// <summary>
        /// True if only a single Rank could match this PotentialRank.
        /// </summary>
        public static bool IsDiscovered(this PotentialRank info)
        {
            bool res = info > 0 && (info & (info - 1)) == 0;
            return res;
        }
    }
}
