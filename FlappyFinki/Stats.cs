using System;

namespace FlappyFinki
{
    public class Stats : IEquatable<Stats>, IComparable<Stats>
    {
        public string PlayerName { get; private set; }
        public int Score { get; private set; }
        private DateTime date;
        public Stats(string name, int score)
        {
            PlayerName = name;
            Score = score;
            date = DateTime.Now;
        }
        public void IncreaseScore()
        {
            Score += 1;
        }

        public override string ToString()
        {
            return PlayerName + " : " + Score + " - " + date.ToShortDateString();
        }

        public bool Equals(Stats other)
        {
            return PlayerName.Equals(other.PlayerName) && Score.Equals(other.Score) && date.Equals(other.date);
        }

        public int CompareTo(Stats other)
        {
            return other.Score - Score;
        }
    }
}
