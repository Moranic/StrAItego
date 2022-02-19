using System;

namespace StrAItego.Game
{
    partial class Board {

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

        public static Square GetSquare(int row, int column)
        {
            if (row < 0 || row > 9 || column < 0 || column > 9)
                return Square.None;
            return _squareTable[row * 10 + column];
        }
    }
}
