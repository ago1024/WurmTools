using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace AnalyzeTool
{
    public enum TileType
    {
        Unknown,
        Nothing,     // No resource (eg there's tunnel, rock or reinforced) (future use, currently Unknown is used to identify those tiles)
        Something,   // Some kind of ore or resource
        Rock,        // Pure rock
        Reinforced,  // Reinforced rock
        Tunnel,      // Tile has been mined out
        Copper,
        Gold,
        Iron,
        Lead,
        Marble,
        Silver,
        Slate,
        Tin,
        Zinc,
    }

    public enum Quality
    {
        Unknown,
        Poor,
        Acceptable,
        Normal,
        Good,
        VeryGood,
        Utmost,
    }

    public struct Tile
    {
        private int x;
        private int y;

        public Tile(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int X
        {
            get { return x; }
        }

        public int Y
        {
            get { return y; }
        }
    }

    public class Detected
    {
        public Detected(TileType type, Quality quality)
        {
            this.Type = type;
            this.Quality = quality;
        }

        public Detected()
        {
            this.Type = TileType.Unknown;
            this.Quality = Quality.Unknown;
        }

        public TileType Type;
        public Quality Quality;

        public static bool IsOreType(TileType type)
        {
            switch (type)
            {
                case TileType.Copper:
                case TileType.Gold:
                case TileType.Iron:
                case TileType.Lead:
                case TileType.Marble:
                case TileType.Silver:
                case TileType.Slate:
                case TileType.Tin:
                case TileType.Zinc:
                case TileType.Something:
                    return true;
                default:
                    return false;
            }
        }

        public bool Matches(Detected d)
        {
            if (Type == TileType.Unknown || d.Type == TileType.Unknown)
            {
                return Type == d.Type;
            }
            else if (Type == TileType.Something && IsOreType(d.Type))
            {
                return true;
            }
            else if (d.Type == TileType.Something && IsOreType(Type))
            {
                return true;
            }
            else if (Type == d.Type)
            {
                if (Quality != Quality.Unknown && d.Quality != Quality.Unknown)
                {
                    return Quality == d.Quality;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            if (Type == TileType.Unknown)
                return "Rock";
            else if (Quality == Quality.Unknown)
                return Type.ToString();
            else
                return String.Format("{0} ({1})", Type, Quality);            
        }
    }

    public class TileStatus
    {
        private AnalyzeResult result;
        private int? qualityExact;
        private TileType type;
        private Quality quality;

        public TileType Type
        {
            get {
                if (Found != null && Found.Type != TileType.Unknown)
                    return Found.Type;
                return type;
            }
            set {
                type = value;
                if (Found == null)
                    Found = new Detected();
                Found.Type = value;
            }
        }
        public bool HasExactQuality 
        {
            get { return qualityExact != null; }
        }
        public int ExactQuality
        {
            get { return (int)qualityExact; }
            set 
            { 
                qualityExact = value;
                Quality = ConvertExactQuality(value);
            }
        }
        public Quality Quality
        {
            get
            {
                if (quality != Quality.Unknown)
                    return quality;
                if (Found != null && Found.Quality != Quality.Unknown)
                    return Found.Quality;
                return Quality.Unknown;
            }
            set {
                quality = value;
            }
        }

        public Quality ConvertExactQuality(int exactQuality)
        {
            if (exactQuality < 30)
                return Quality.Poor;
            if (exactQuality < 40)
                return Quality.Acceptable;
            if (exactQuality < 60)
                return Quality.Normal;
            if (exactQuality < 80)
                return Quality.Good;
            if (exactQuality < 95)
                return Quality.VeryGood;
            return Quality.Utmost;            
        }

        public AnalyzeResult Result
        {
            get { return result; }
            set { result = value; }
        }
        public Detected Found;
        public List<Detected> Estimates;

        public TileStatus()
        {
            this.type = TileType.Unknown;
            this.quality = Quality.Unknown;
            this.Result = null;
            this.Found = null;
            this.Estimates = null;
        }

        public void Reset()
        {
            this.Result = null;
            this.Found = null;
            this.Estimates = null;
        }

        public int Add(List<Detected> detected)
        {
            foreach (Detected d in detected)
            {
                if (d.Type == TileType.Unknown)
                {
                    Set(d);
                    return 0;
                }
                else if (Found != null)
                {
                    if (Found.Matches(d) && d.Type != TileType.Something)
                    {
                        if (Quality == Quality.Unknown)
                            Set(d);
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }

            if (Estimates != null)
                return Merge(detected);
            else 
            {
                Estimates = new List<Detected>();
                foreach (Detected d in detected)
                {
                    Estimates.Add(d);
                }
                return 1;
            }
        }

        public bool Matches(Detected d)
        {
            if (Found != null)
            {
                return Found.Matches(d);
            }
            else if (Estimates != null)
            {
                foreach (Detected e in Estimates)
                {
                    if (e.Matches(d))
                        return true;
                }
            }
            return false;
        }

        private HashSet<TileType> GetSomethingTypes(List<Detected> detected)
        {
            HashSet<TileType> types = new HashSet<TileType>();
            foreach (TileType type in Enum.GetValues(typeof(TileType)))
            {
                if (Detected.IsOreType(type) && type != TileType.Something)
                    types.Add(type);
            }

            foreach (Detected d in detected)
            {
                TileType type = d.Type;
                if (Detected.IsOreType(type) && type != TileType.Something)
                    types.Remove(type);
            }

            return types;
        }

        private int Merge(List<Detected> detected)
        {
            HashSet<TileType> somethingTypesD = null;
            HashSet<TileType> somethingTypesE = null;
            foreach (Detected d in detected) 
            {
                if (d.Type == TileType.Something)
                {
                    somethingTypesD = GetSomethingTypes(detected);
                    break;
                }
            }
            foreach (Detected e in Estimates)
            {
                if (e.Type == TileType.Something)
                {
                    somethingTypesE = GetSomethingTypes(Estimates);
                    break;
                }
            }

            List<Detected> result = new List<Detected>();
            foreach (Detected d in detected)
            {
                foreach (Detected e in Estimates)
                {
                    if (e.Type == TileType.Unknown || d.Type == TileType.Unknown)
                    {
                        result.Add(d);
                    }
                    else if (e.Type == TileType.Something && somethingTypesE.Contains(d.Type))
                    {
                        result.Add(d);
                    }
                    else if (d.Type == TileType.Something && somethingTypesD.Contains(e.Type))
                    {
                        result.Add(e);
                    }
                    else if (e.Type == d.Type)
                    {
                        if (e.Quality != Quality.Unknown && d.Quality != Quality.Unknown)
                        {
                            if (e.Quality.Equals(d.Quality))
                                result.Add(d);
                        }
                        else if (e.Quality != Quality.Unknown)
                        {
                            result.Add(e);
                        }
                        else
                        {
                            result.Add(d);
                        }
                    }
                }
            }
            if (result.Count > 0)
            {
                Estimates = result;
                return 1;
            }
            else
            {
                SetEmpty();
                return 0;
            }
        }

        public void SetEmpty()
        {
            Set(new Detected(TileType.Unknown, Quality.Unknown));
        }

        public void Set(Detected detected)
        {
            Found = new Detected(detected.Type, detected.Quality); ;
            if (Estimates != null)
                Estimates = null;
            if (Found.Type != TileType.Unknown)
                type = Found.Type;
        }

        public Boolean IsUndecided
        {
            get { return (this.Found == null && this.Estimates != null); }
        }

        public Boolean IsEmpty
        {
            get { return (this.Found != null && this.Found.Type == TileType.Unknown); }
        }

        public Boolean IsSet
        {
            get { return (this.Found != null && this.Found.Type != TileType.Unknown); }
        }

        public override string ToString()
        {
            if (IsSet)
            {
                return Found.ToString();
            }
            else if (IsUndecided)
            {
                StringBuilder builder = new StringBuilder();
                bool first = true;
                foreach (Detected d in Estimates)
                {
                    if (first)
                        first = false;
                    else
                        builder.Append(", ");
                    builder.Append(d);
                }
                return builder.ToString();
            }
            else
            {
                return "";
            }
        }
    }

    public class AnalyzeMap
    {
        private int sizeX = 16;
        private int sizeY = 16;
        private TileStatus[,] tileStatus;

        public delegate void MapResizeHandler(Object sender, int newX, int newY, int dx, int dy);
        public event MapResizeHandler OnResize;

        private HashSet<AnalyzeResult> results = new HashSet<AnalyzeResult>();

        public int SizeX
        {
            get { return sizeX; }
            set { ResizeMap(value, sizeY); }
        }

        public int SizeY
        {
            get { return sizeY; }
            set { ResizeMap(sizeX, value); }
        }

        public TileStatus this[int x, int y]
        {
            get { return tileStatus[x, y]; }
        }

        public TileStatus this[Tile tile]
        {
            get { return tileStatus[tile.X, tile.Y]; }
        }

        private Quality GetQuality(String quality)
        {
            if (quality == null || quality.Length == 0)
                return Quality.Unknown;

            switch (quality)
            {
                case "poor":
                    return Quality.Poor;
                case "acceptable":
                    return Quality.Acceptable;
                case "normal":
                    return Quality.Normal;
                case "good":
                    return Quality.Good;
                case "very good":
                    return Quality.VeryGood;
                case "utmost":
                    return Quality.Utmost;
                default:
                    System.Diagnostics.Debug.Print("Unknown quality: {0}", quality);
                    return Quality.Unknown;
            }
        }

        private TileType GetTileType(String type)
        {
            if (type == null || type.Length == 0)
                return TileType.Unknown;
            switch (type)
            {
                case "copper ore":
                    return TileType.Copper;
                case "gold ore":
                    return TileType.Gold;
                case "iron ore":
                    return TileType.Iron;
                case "lead ore":
                    return TileType.Lead;
                case "marble shards":
                    return TileType.Marble;
                case "silver ore":
                    return TileType.Silver;
                case "slate shards":
                    return TileType.Slate;
                case "tin ore":
                    return TileType.Tin;
                case "zinc ore":
                    return TileType.Zinc;
                case "something":
                    return TileType.Something;
                default:
                    System.Diagnostics.Debug.Print("Unknown tile type: {0}", type);
                    return TileType.Unknown;
            }
        }

        public AnalyzeMap(int sizeX, int sizeY)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            tileStatus = new TileStatus[sizeX, sizeY];
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    tileStatus[x, y] = new TileStatus();
                }
            }
        }

        public void ResizeMap(int newX, int newY)
        {
            ResizeMap(newX, newY, 0, 0);
        }

        public void OffsetMap(int dx, int dy)
        {
            ResizeMap(sizeX + dx, sizeY + dy, dx, dy);
        }

        public void ResizeMap(int newX, int newY, int dx, int dy)
        {
            if (newX <= 0 || newY <= 0)
            {
                throw new Exception("A map can't be smaller than 1 tile in each direction");
            }
            TileStatus[,] newTileStatus = new TileStatus[newX, newY];
            for (int x = 0; x < newX; x++)
            {
                for (int y = 0; y < newY; y++)
                {
                    int oldX = x - dx;
                    int oldY = y - dy;
                    if (oldX < 0 || oldY < 0 || oldX >= sizeX || oldY >= sizeY)
                        newTileStatus[x, y] = new TileStatus();
                    else
                    {
                        newTileStatus[x, y] = tileStatus[oldX, oldY];
                        if (newTileStatus[x, y].Result != null && newTileStatus[x, y].Result.PositionSet)
                        {
                            newTileStatus[x, y].Result.X += dx;
                            newTileStatus[x, y].Result.Y += dy;
                        }
                    }
                }
            }
            tileStatus = newTileStatus;
            sizeX = newX;
            sizeY = newY;

            if (OnResize != null)
                OnResize(this, newX, newY, dx, dy);
        }

        public bool IsValidTile(Tile tile)
        {
            return IsValidTile(tile.X, tile.Y);
        }

        private bool IsValidTile(int x, int y)
        {
            return x >= 0 && x < SizeX && y >= 0 && y < SizeY;
        }

        private int SetDetected(int x, int y, List<Detected> detected)
        {
            if (IsValidTile(x, y))
            {
                return tileStatus[x, y].Add(detected);
            }
            else
            {
                return 1;
            }
        }

        private int[][] pointWalkerData = { new int[] { -1, -1, +1, 0 }, new int[] { +1, -1, 0, +1 }, new int[] { +1, +1, -1, 0 }, new int[] { -1, +1, 0, -1 } };
        private List<Tile> SelectTiles(int x, int y, int distance)
        {
            List<Tile> ret = new List<Tile>();
            foreach (int[] data in pointWalkerData)
            {
                int sx = x + data[0] * distance;
                int sy = y + data[1] * distance;

                ret.Add(new Tile(sx, sy));
                for (int step = 0; step < 2 * distance - 1; step++)
                {
                    sx += data[2];
                    sy += data[3];
                    ret.Add(new Tile(sx, sy));
                }
            }
            return ret;
        }

        private void SetDetected(int x, int y, int distance, List<Detected> detected)
        {
            foreach (Tile p in SelectTiles(x, y, distance))
            {
                SetDetected(p.X, p.Y, detected);
            }
            foreach (Detected d in detected)
            {
                List<Tile> matches = new List<Tile>();
                foreach (Tile p in SelectTiles(x, y, distance))
                {
                    if (!IsValidTile(p.X, p.Y) || tileStatus[p.X, p.Y].Matches(d))
                        matches.Add(p);
                }
                if (matches.Count == 1)
                {
                    Tile p = matches[0];
                    if (IsValidTile(p.X, p.Y))
                    {
                        Detected n = new Detected(d.Type, d.Quality);
                        if (tileStatus[p.X, p.Y].Type == d.Type && d.Quality == Quality.Unknown)
                        {
                            n.Quality = tileStatus[p.X, p.Y].Quality;
                        }
                        tileStatus[p.X, p.Y].Set(n);
                    }
                }
            }
        }

        public void SetResult(int x, int y, AnalyzeResult result)
        {
            if (tileStatus[x, y].Result != null && tileStatus[x, y].Result != result)
            {
                results.Remove(tileStatus[x, y].Result);
                Reset();
                Refresh();
            }
            if (results.Contains(result))
            {
                results.Remove(result);
                Reset();
                Refresh();
            }

            Dictionary<int, List<Detected>> matches = new Dictionary<int,List<Detected>>();
            foreach (AnalyzeMatch match in result.Matches)
            {
                if (!matches.ContainsKey(match.Distance))
                    matches.Add(match.Distance, new List<Detected>());
                matches[match.Distance].Add(new Detected(GetTileType(match.Type), GetQuality(match.Quality)));
            }

            int maxDistance = 0;
            foreach (int distance in matches.Keys)
            {
                maxDistance = Math.Max(distance, maxDistance);
            }

            if (x - maxDistance < 0 || y - maxDistance < 0 || x + maxDistance >= sizeX || y + maxDistance >= sizeY)
            {
                int dx = Math.Max(0, maxDistance - x);
                int dy = Math.Max(0, maxDistance - y);
                int newX = Math.Max(sizeX, x + maxDistance + 1);
                int newY = Math.Max(sizeY, y + maxDistance + 1);
                ResizeMap(newX + dx, newY + dy, dx, dy);

                x += dx;
                y += dy;
            }


            foreach (int distance in matches.Keys) 
            {
                SetDetected(x, y, distance, matches[distance]);
            }

            tileStatus[x, y].Result = result;
            result.X = x;
            result.Y = y;
            results.Add(result);
        }

        public void Reset()
        {
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    tileStatus[x, y].Reset();
                }
            }
        }

        public void ClearResults()
        {
            Reset();
            foreach (AnalyzeResult result in results)
            {
                result.ResetPosition();
            }
            results.Clear();
        }

        public void Refresh()
        {
            List<AnalyzeResult> temp = new List<AnalyzeResult>(results);
            results.Clear();
            foreach (AnalyzeResult result in temp)
            {
                SetResult(result.X, result.Y, result);
            }
        }

        private static String namespaceUri = "urn:gotti.org:AnalyzeTool/Map";
        public XmlDocument Serialize()
        {
            XmlDocument doc = new XmlDocument();
            doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
            XmlElement root = doc.CreateElement("AnalyzeMap", namespaceUri);
            doc.AppendChild(root);

            root.SetAttribute("xmlns", namespaceUri);
            root.SetAttribute("version", "1.1");
            root.SetAttribute("width", this.SizeX.ToString());
            root.SetAttribute("height", this.SizeY.ToString());

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    TileStatus status = tileStatus[x, y];
                    if (status.Type != TileType.Unknown)
                    {
                        XmlElement element = doc.CreateElement("Tile", namespaceUri);
                        element.SetAttribute("x", x.ToString());
                        element.SetAttribute("y", y.ToString());
                        element.SetAttribute("type", status.Type.ToString());
                        if (status.HasExactQuality)
                            element.SetAttribute("quality", status.ExactQuality.ToString());
                        else if (status.Quality != Quality.Unknown)
                            element.SetAttribute("quality", status.Quality.ToString());
                        root.AppendChild(element);
                    }
                }
            }

            foreach (AnalyzeResult result in results)
            {
                XmlElement element = doc.CreateElement("Analyze", namespaceUri);
                element.SetAttribute("x", result.X.ToString());
                element.SetAttribute("y", result.Y.ToString());

                foreach (AnalyzeMatch match in result.Matches)
                {
                    XmlElement node = doc.CreateElement("Entry", namespaceUri);
                    node.SetAttribute("distance", match.Distance.ToString());
                    if (match.Type != null)
                        node.SetAttribute("type", match.Type);
                    if (match.Quality != null)
                        node.SetAttribute("quality", match.Quality);
                    element.AppendChild(node);
                }

                root.AppendChild(element);
            }

            return doc;
        }

        public void Save(Stream stream)
        {
            XmlDocument doc = Serialize();
            doc.Save(stream);
        }

        public static AnalyzeMap Load_1_1(XPathNavigator nav)
        {
            IXmlNamespaceResolver resolver = new MyNamespaceResolver();
            XPathNavigator root = nav.SelectSingleNode("//atm:AnalyzeMap", resolver);
            Double w = (Double) root.Evaluate("number(@width)");
            Double h = (Double) root.Evaluate("number(@height)");

            AnalyzeMap map = new AnalyzeMap((int)w, (int)h);

            XPathNodeIterator iter;
            iter = root.Select("atm:Tile", resolver);
            while (iter.MoveNext())
            {
                int x = (int)(Double)iter.Current.Evaluate("number(@x)");
                int y = (int)(Double)iter.Current.Evaluate("number(@y)");
                String type = (String)iter.Current.Evaluate("string(@type)");
                String quality = (String)iter.Current.Evaluate("string(@quality)");
                if (map.IsValidTile(x, y))
                {
                    map[x, y].Type = (TileType)Enum.Parse(typeof(TileType), type);
                    int exactQL;
                    if (quality != null && quality.Length != 0)
                    {
                        if (Int32.TryParse(quality, out exactQL))
                        {
                            map[x, y].ExactQuality = exactQL;
                        }
                        else
                        {
                            map[x, y].Quality = (Quality)Enum.Parse(typeof(Quality), quality);
                        }
                    }
                }
                else
                {
                    throw new Exception(String.Format("Tile {0},{1} is not valid for the map", x, y));
                }
            }

            iter = root.Select("atm:Analyze", resolver);
            while (iter.MoveNext())
            {
                int x = (int)(Double)iter.Current.Evaluate("number(@x)");
                int y = (int)(Double)iter.Current.Evaluate("number(@y)");

                List<AnalyzeMatch> matches = new List<AnalyzeMatch>();
                XPathNodeIterator entry = iter.Current.Select("atm:Entry", resolver);
                while (entry.MoveNext())
                {
                    String type = (String)entry.Current.Evaluate("string(@type)");
                    String quality = (String)entry.Current.Evaluate("string(@quality)");
                    int distance = (int)(Double)entry.Current.Evaluate("number(@distance)");

                    if (type != null && type.Length == 0)
                        type = null;
                    if (quality != null && quality.Length == 0)
                        quality = null;

                    matches.Add(new AnalyzeMatch(distance, type, quality));
                }

                AnalyzeResult result = new AnalyzeResult(matches);
                result.X = x;
                result.Y = y;
                map.SetResult(x, y, result);
            }

            return map;
        }

        private class MyNamespaceResolver : IXmlNamespaceResolver
        {
            public string LookupNamespace(string prefix)
            {
                switch (prefix)
                {
                    case "atm":
                        return namespaceUri;
                    default:
                        return null;
                }
            }

            public string LookupPrefix(string namespaceName)
            {
                throw new NotImplementedException();
            }

            public IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope)
            {
                throw new NotImplementedException();
            }
        }

        public static AnalyzeMap Load(Stream stream)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(stream);
            XPathNavigator nav = doc.CreateNavigator();     
            String version = nav.Evaluate("string(//atm:AnalyzeMap/@version)", new MyNamespaceResolver()) as String;
            if ("1.0".Equals(version) || "1.1".Equals(version))
            {
                return Load_1_1(nav);
            }
            else
            {
                throw new Exception("Invalid version " + version);
            }
        }

    }
}
