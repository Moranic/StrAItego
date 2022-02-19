namespace StrAItego.Game
{
    public class Piece
    {
        public Rank Rank { get; set; }
        public Team Team { get; set; }
        public int SetupOrigin { get; set; }
        public PotentialRank PotentialRank { get; set; }

        public Piece(Rank r, Team t, int setupOrigin) {
            Rank = r;
            Team = t;
            SetupOrigin = setupOrigin;
            PotentialRank = PotentialRank.Any;
        }

        Piece(Rank r, Team t, int sO, PotentialRank pr) {
            Rank = r;
            Team = t;
            SetupOrigin = sO;
            PotentialRank = pr;
        }

        public Piece Copy() {
            return new Piece(Rank, Team, SetupOrigin, PotentialRank);
        }

        public void CopyTo(Piece p) {
            p.Rank = Rank;
            p.Team = Team;
            p.SetupOrigin = SetupOrigin;
            p.PotentialRank = PotentialRank;
        }

        public void Invert() {
            Team = 1 - Team;
        }

        public static implicit operator Unit(Piece p) => p is null ? Unit.None : (Unit)((int)p.Rank + ((int)p.Team * 12));
        public static implicit operator Rank(Piece p) => p is null ? Rank.None : p.Rank;
        public static implicit operator Team(Piece p) => p is null ? Team.Neither : p.Team;
        public static implicit operator PotentialRank(Piece p) => p is null ? PotentialRank.None : p.PotentialRank;
        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Used for comparing ranks.
        /// </summary>
        public static bool operator ==(Piece a, Piece b) => a?.Rank == b?.Rank;
        /// <summary>
        /// Used for comparing ranks.
        /// </summary>
        public static bool operator !=(Piece a, Piece b) => !(a == b);
        /// <summary>
        /// Used for comparing ranks.
        /// </summary>
        public static bool operator >(Piece a, Piece b) => (a?.Rank ?? Rank.None) > (b?.Rank ?? Rank.None);
        /// <summary>
        /// Used for comparing ranks.
        /// </summary>
        public static bool operator <(Piece a, Piece b) => (a?.Rank ?? Rank.None) < (b?.Rank ?? Rank.None);
        /// <summary>
        /// Used for comparing ranks.
        /// </summary>
        public static bool operator >=(Piece a, Piece b) => a > b || a == b;
        /// <summary>
        /// Used for comparing ranks.
        /// </summary>
        public static bool operator <=(Piece a, Piece b) => a < b || a == b;

        public void ResetPotentialRank() {
            PotentialRank = PotentialRank.Any;
        }

        public bool HasMoved {
            get { return (PotentialRank & PotentialRank.BombOrFlag) == 0; }
        }

        public override string ToString() {
            return Team.ToString() + " " + Rank.ToString();
        }
    }
}
