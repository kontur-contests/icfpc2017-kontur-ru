using System;
using System.IO;

namespace lib.Structures
{
    public class Move : IEquatable<Move>
    {
        public ClaimMove claim;
        public PassMove pass;
        public SplurgeMove splurge;
        public OptionMove option;

        public static Move Claim(int punter, int source, int target)
        {
            return new Move { claim = new ClaimMove { punter = punter, source = source, target = target } };
        }

        public static Move Option(int punter, int source, int target)
        {
            return new Move { option = new OptionMove { punter = punter, source = source, target = target } };
        }

        public static Move Pass(int punter)
        {
            return new Move {pass = new PassMove {punter = punter}};
        }

        public static Move Splurge(int punter, int[] siteIds)
        {
            return new Move {splurge = new SplurgeMove {punter = punter, route = siteIds}};
        }

        public override string ToString()
        {
            if (claim != null)
                return claim.ToString();
            if (pass != null)
                return pass.ToString();
            if (splurge != null)
                return splurge.ToString();
            if (option != null)
                return option.ToString();
            return "Invalid move";
        }

        public bool Equals(Move other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(claim, other.claim) && Equals(pass, other.pass) && Equals(splurge, other.splurge) && Equals(option, other.option);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Move) obj);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (claim != null ? claim.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (pass != null ? pass.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (splurge != null ? splurge.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (option != null ? option.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(Move left, Move right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Move left, Move right)
        {
            return !Equals(left, right);
        }

        public static Move DecodeFrom(BinaryReader reader)
        {
            //TODO: поддержка других ходов.
            // Это нужно только для сохранения логов в Firebase
            byte marker = reader.ReadByte();
            if (marker == 0) return null;
            int punter = reader.ReadInt32();
            int source = reader.ReadInt32();
            int target = reader.ReadInt32();
            return Claim(punter, source, target);
        }

        public void EncodeTo(BinaryWriter w)
        {
            //TODO: поддержка других ходов.
            // Это нужно только для сохранения логов в Firebase
            if (claim == null) return;
            w.Write((byte) 1);
            w.Write(claim.punter);
            w.Write(claim.source);
            w.Write(claim.target);
        }
    }

    public static class MoveExtension
    {
        public static int GetPunter(this Move move)
        {
            if (move.claim != null)
                return move.claim.punter;
            if (move.pass != null)
                return move.pass.punter;
            if (move.splurge != null)
                return move.splurge.punter;
            if (move.option != null)
                return move.option.punter;
            return -1;
        }
    }
}