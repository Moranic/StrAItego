using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrAItego.Game
{
    public struct Move
    {
        public Move(Piece attacker, Square from, Square to, Piece defender, int id) {
            Attacker = attacker;
            Origin = from;
            Destination = to;
            Defender = defender;
            ID = id;
        }

        public Piece Attacker { get; }

        public Square Origin { get; }

        public Square Destination { get; }

        public Piece Defender { get; }

        public PotentialRank InfoOfAttacker { get { return Attacker?.PotentialRank ?? PotentialRank.None; } }

        public PotentialRank InfoOfDefender { get { return Defender?.PotentialRank ?? PotentialRank.None; } }

        public int ID { get; set; }

        public Move GetInvertedMove() {
            Piece unit = Attacker.Copy();
            unit.Invert();
            Square from = 91 - Origin;
            Square to = 91 - Destination;
            Piece attackedUnit = Defender?.Copy();
            attackedUnit?.Invert();
            return new Move(unit, from, to, attackedUnit, ID);
        }

        public override string ToString() {
            return Attacker + " from " + Origin + " to " + Destination + (Defender != null ? " attacking " + Defender : "");
        }

        public string LogString() {
            return $"{Attacker, -15}" + " from " + Origin + " to " + Destination + (Defender != null ? " attacking " + Defender : "");
        }
    }
}
