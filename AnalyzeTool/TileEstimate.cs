using System;
using System.Collections.Generic;
using System.Text;

namespace AnalyzeTool
{
    public class TileEstimate
    {

        private HashSet<Detected> estimates = null;

        private static HashSet<Detected> emptySet = new HashSet<Detected>(comparer);
        private static IEqualityComparer<Detected> comparer = new DetectedEqualityComparer();
        private static List<Detected> somethingTypes = new List<Detected>();

        public HashSet<Detected> Estimates
        {
            get { if (estimates == null) return emptySet; else return estimates; }
        }

        static TileEstimate()
        {
            foreach (TileType type in Enum.GetValues(typeof(TileType)))
            {
                if (Detected.IsOreType(type) && type != TileType.Something)
                    somethingTypes.Add(new Detected(type, Quality.Unknown));
                else if (Detected.isSpecialType(type) && type != TileType.Something)
                    somethingTypes.Add(new Detected(type, Quality.Unknown));
            }
        }

        public void Add(List<Detected> detected)
        {
            if (estimates == null)
                estimates = this.MakeSet(detected);
            else if (estimates.Count > 0)
            {
                HashSet<Detected> set = this.MakeSet(detected);
                HashSet<Detected> newSet = new HashSet<Detected>(estimates, comparer);
                newSet.IntersectWith(set);

                foreach (Detected d in set)
                {
                    if (estimates.Contains(d) && d.Quality != Quality.Unknown)
                    {
                        newSet.Remove(d);
                        newSet.Add(d);
                    }
                }

                estimates = newSet;
            }

            if (estimates.Count == 0)
                estimates = emptySet;
        }

        private HashSet<Detected> MakeSet(List<Detected> detected)
        {
            HashSet<Detected> set = new HashSet<Detected>(comparer);

            foreach (Detected d in detected)
            {
                if (d.Type == TileType.Nothing)
                {
                    continue;
                }
                else if (d.Type == TileType.Something)
                {
                    set.UnionWith(somethingTypes);
                }
                else if (Detected.IsOreType(d.Type))
                {
                    if (!set.Contains(d) || d.Quality != Quality.Unknown)
                    {
                        set.Remove(d);
                        set.Add(d);
                    }
                }
                else if (Detected.isSpecialType(d.Type))
                {
                    if (!set.Contains(d))
                        set.Add(d);
                }
            }
            return set;
        }

        private String ToString(HashSet<Detected> set)
        {
            StringBuilder str = new StringBuilder();
            foreach (Detected d in set)
            {
                str.Append(d);
                str.Append(" ");
            }
            return str.ToString();
        }

        public override String ToString()
        {
            return ToString(Estimates);
        }
    }

    public class DetectedEqualityComparer : IEqualityComparer<Detected>
    {

        public bool Equals(Detected x, Detected y)
        {
            if (x.Type != y.Type)
                return false;
            if (x.Quality == Quality.Unknown || y.Quality == Quality.Unknown)
                return true;
            if (x.Quality == y.Quality)
                return true;
            return false;
        }

        public int GetHashCode(Detected obj)
        {
            return obj.Type.GetHashCode();
        }
    }
}
