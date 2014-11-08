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
        Nothing,     // No resource (eg there's tunnel, rock or reinforced)
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
        Flint,      // Flint can be present in addition to any of the other types
        Salt        // Salt can be present in addition to any of the other types
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

    public enum Direction
    {
        Unknown,
        N,
        NNE,
        NE,
        ENE,
        E,
        ESE,
        SE,
        SSE,
        S,
        SSW,
        SW,
        WSW,
        W,
        WNW,
        NW,
        NNW
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
            this.Direction = Direction.Unknown;
        }

        public Detected(TileType type, Quality quality, Direction direction)
        {
            this.Type = type;
            this.Quality = quality;
            this.Direction = direction;
        }

        public Detected()
        {
            this.Type = TileType.Unknown;
            this.Quality = Quality.Unknown;
            this.Direction = Direction.Unknown;
        }

        public TileType Type;
        public Quality Quality;
        public Direction Direction;

        public static bool isSpecialType(TileType type)
        {
            switch (type)
            {
                case TileType.Flint:
                case TileType.Salt:
                    return true;
                default:
                    return false;
            }
        }

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
            if (Type == TileType.Nothing || d.Type == TileType.Nothing)
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
            if (Type == TileType.Unknown || Type == TileType.Nothing)
                return "Rock";
            else if (Type == TileType.Something)
                return "Something";
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
        private bool? hasSalt;
        private bool? hasFlint;
        public Detected Found;
        TileEstimate TileEstimate;

        public TileType Type
        {
            get {
                if (Found != null && Found.Type != TileType.Nothing)
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
        public bool HasSalt
        {
            get { return hasSalt != null ? (bool)hasSalt : false; }
            set { hasSalt = value; }
        }
        public bool HasFlint
        {
            get { return hasFlint != null ? (bool)hasFlint : false; }
            set { hasFlint = value; }
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
        public HashSet<Detected> Estimates
        {
            get { return TileEstimate == null ? null : TileEstimate.Estimates; }
        }

        public TileStatus()
        {
            this.type = TileType.Unknown;
            this.quality = Quality.Unknown;
            this.Result = null;
            this.Found = null;
            this.TileEstimate = null;
        }

        public void Reset()
        {
            this.Result = null;
            this.Found = null;
            this.TileEstimate = null;
            this.hasSalt = null;
            this.hasFlint = null;
        }

        /**
         * Add a list of detected ores to the tile.
         * # It checks if nothing has been detected so the tile can be marked empty.
         * # If the tile already has an ore set which matches any of the detected ores the quality is updated
         * # If the tile already has some results the results are merged.
         * # Otherwise the list is set.
         */
        public int Add(List<Detected> detected)
        {
            foreach (Detected d in detected)
            {
                if (d.Type == TileType.Nothing)
                {
                    // Nothing has been detected. Mark the tile as empty.
                    Set(d);
                    return 0;
                }
                else if (Found != null)
                {
                    // The detected ore matches what's already there
                    if (Found.Matches(d))
                    {
                        // Update the quality
                        if (Quality == Quality.Unknown && d.Quality != Quality.Unknown)
                            Found.Quality = d.Quality;
                        return 1;
                    }
                }
            }

            // We already have something set for this tile but none of the detected ores matched it.
            // This should not happen so we just bail out here.
            if (Found != null)
                return 0;

            if (TileEstimate == null)
                TileEstimate = new TileEstimate();

            TileEstimate.Add(detected);
            if (TileEstimate.Estimates.Count == 0)
                return 0;
            else
                return 1;
        }

        public bool Matches(Detected d)
        {
            if (d.Type == TileType.Salt)
            {
                return hasSalt == null || (bool)hasSalt == true;
            }
            else if (d.Type == TileType.Flint)
            {
                return hasFlint == null || (bool)hasFlint == true;
            }
            else if (Found != null)
            {
                return Found.Matches(d);
            }
            else if (TileEstimate != null)
            {
                foreach (Detected e in TileEstimate.Estimates)
                {
                    if (e.Matches(d))
                        return true;
                }
            }
            return false;
        }

        /**
         * Get the TileTypes which are covered by "Something". 
         * Iron and Something effectively rules out Iron since it has been 
         * detected on its own.
         */
        private HashSet<TileType> GetSomethingTypes(List<Detected> detected)
        {
            HashSet<TileType> types = new HashSet<TileType>();
            // Add all possible ore types
            foreach (TileType type in Enum.GetValues(typeof(TileType)))
            {
                if (Detected.IsOreType(type) && type != TileType.Something)
                    types.Add(type);
            }

            // Remove anything that has been detected already
            foreach (Detected d in detected)
            {
                TileType type = d.Type;
                if (Detected.IsOreType(type) && type != TileType.Something)
                    types.Remove(type);
            }

            return types;
        }

        public void SetEmpty()
        {
            Set(new Detected(TileType.Nothing, Quality.Unknown));
        }

        public void Set(Detected detected)
        {
            Found = new Detected(detected.Type, detected.Quality); ;
            if (TileEstimate != null)
                TileEstimate = null;
            if (Found.Type != TileType.Nothing)
                type = Found.Type;
        }

        public Boolean IsUndecided
        {
            get { return (this.Found == null && this.TileEstimate != null); }
        }

        public Boolean IsEmpty
        {
            get { return (this.Found != null && this.Found.Type == TileType.Nothing); }
        }

        public Boolean IsSet
        {
            get { return (this.Found != null && this.Found.Type != TileType.Nothing); }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if (IsSet || HasSalt || HasFlint)
            {
                builder.Append(Found.ToString());
                if (HasSalt)
                {
                    builder.Append(", Salt");
                }
                if (HasFlint)
                {
                    builder.Append(", Flint");
                }
            }
            else if (IsUndecided)
            {
                bool first = true;
                foreach (Detected d in TileEstimate.Estimates)
                {
                    if (first)
                        first = false;
                    else
                        builder.Append(", ");
                    builder.Append(d);
                }
            }
            else
            {
            }
            return builder.ToString();
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
                return TileType.Nothing;
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
                case "flint":
                    return TileType.Flint;
                case "salt":
                    return TileType.Salt;
                case "something":
                    return TileType.Something;
                default:
                    System.Diagnostics.Debug.Print("Unknown tile type: {0}", type);
                    return TileType.Unknown;
            }
        }

        private Direction GetDirection(String direction)
        {
            if (direction == null || direction.Length == 0)
                return Direction.Unknown;
            switch (direction)
            {
                case "north":
                    return Direction.N;
                case "east of north":
                    return Direction.NNE;
                case "northeast":
                    return Direction.NE;
                case "north of east":
                    return Direction.ENE;
                case "east":
                    return Direction.E;
                case "south of east":
                    return Direction.ESE;
                case "southeast":
                    return Direction.SE;
                case "east of south":
                    return Direction.SSE;
                case "south":
                    return Direction.S;
                case "west of south":
                    return Direction.SSW;
                case "southwest":
                    return Direction.SW;
                case "south of west":
                    return Direction.WSW;
                case "west":
                    return Direction.W;
                case "north of west":
                    return Direction.WNW;
                case "northwest":
                    return Direction.NW;
                case "west of north":
                    return Direction.NNW;
                default:
                    System.Diagnostics.Debug.Print("Unknown direction: {0}", direction);
                    return Direction.Unknown;
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

        private Direction GetTileDirection(Tile p, int x, int y)
        {
            int dx = p.X - x;
            int dy = p.Y - y;
            int dc = Math.Abs(dx) - Math.Abs(dy);

            if (dx == 0 && dy == 0)
                return Direction.Unknown;
            else if (dx == 0 && dy > 0)
                return Direction.S;
            else if (dx == 0 && dy < 0)
                return Direction.N;
            else if (dx > 0 && dy == 0)
                return Direction.E;
            else if (dx < 0 && dy == 0)
                return Direction.W;
            else if (dx < 0 && dy < 0)
            {
                if (dc == 0)
                    return Direction.NW;
                else if (dc < 0)
                    return Direction.NNW;
                else
                    return Direction.WNW;
            }
            else if (dx < 0 && dy > 0)
            {
                if (dc == 0)
                    return Direction.SW;
                else if (dc < 0)
                    return Direction.SSW;
                else
                    return Direction.WSW;
            }
            else if (dx > 0 && dy < 0)
            {
                if (dc == 0)
                    return Direction.NE;
                else if (dc < 0)
                    return Direction.NNE;
                else
                    return Direction.ENE;
            }
            else if (dx > 0 && dy > 0)
            {
                if (dc == 0)
                    return Direction.SE;
                else if (dc < 0)
                    return Direction.SSE;
                else
                    return Direction.ESE;
            }
            return Direction.Unknown;
        }

        private List<Detected> FilterDetected(List<Detected> detected, Direction dir)
        {
            int specialCount = 0;
            if (detected == null || detected.Count == 0 || dir == Direction.Unknown)
                return detected;

            List<Detected> ret = new List<Detected>();
            foreach (Detected d in detected) {
                if (d.Direction == Direction.Unknown || d.Direction == dir)
                {
                    ret.Add(d);
                    if (Detected.isSpecialType(d.Type))
                    {
                        specialCount++;
                    }
                }
            }
            if (ret.Count == detected.Count)
            {
                return detected;
            }
            else if (ret.Count == specialCount)
            {
                ret.Add(new Detected(TileType.Nothing, Quality.Unknown));
                return ret;
            }
            else
            {
                return ret;
            }
        }

        private void SetDetected(int x, int y, int distance, List<Detected> detected)
        {
            foreach (Tile p in SelectTiles(x, y, distance))
            {
                Direction dir = GetTileDirection(p, x, y);
                List<Detected> dirDetected = FilterDetected(detected, dir);
                SetDetected(p.X, p.Y, dirDetected);
            }
            foreach (Detected d in detected)
            {
                List<Tile> matches = new List<Tile>();
                foreach (Tile p in SelectTiles(x, y, distance))
                {
                    if (d.Direction != Direction.Unknown && d.Direction != GetTileDirection(p, x, y))
                        continue;
                    if (!IsValidTile(p.X, p.Y) || tileStatus[p.X, p.Y].Matches(d))
                        matches.Add(p);
                }
                if (matches.Count == 1)
                {
                    Tile p = matches[0];
                    if (IsValidTile(p.X, p.Y))
                    {
                        if (d.Type == TileType.Salt)
                        {
                            tileStatus[p.X, p.Y].HasSalt = true;
                        }
                        else if (d.Type == TileType.Flint)
                        {
                            tileStatus[p.X, p.Y].HasFlint = true;
                        }
                        else
                        {
                            Detected n = new Detected(d.Type, d.Quality);
                            if (n.Type == TileType.Something)
                            {
                                n.Type = tileStatus[p.X, p.Y].Type;
                            }
                            if (tileStatus[p.X, p.Y].Type == d.Type && d.Quality == Quality.Unknown)
                            {
                                n.Quality = tileStatus[p.X, p.Y].Quality;
                            }
                            tileStatus[p.X, p.Y].Set(n);
                        }
                    }
                }
            }
        }

        public void Remove(AnalyzeResult result)
        {
            if (results.Contains(result))
            {
                results.Remove(result);
                if (!result.PositionSet)
                {
                    // Should not happen
                    System.Diagnostics.Debug.Print("AnalyzeResult to be removed has no position set");
                    Reset();
                }
                else if (tileStatus[result.X, result.Y].Result != result)
                {
                    // Should not happen either
                    System.Diagnostics.Debug.Print("AnalyzeResult to be removed is not where it's supposed to be");
                    Reset();
                }
                else
                {
                    // Reset all tiles that are affected by the Result
                    tileStatus[result.X, result.Y].Result = null;
                    Dictionary<int, List<Detected>> matches = GetMatches(result);
                    foreach (int distance in matches.Keys)
                    {
                        foreach (Tile p in SelectTiles(result.X, result.Y, distance))
                        {
                            tileStatus[p.X, p.Y].Reset();
                        }                        
                    }
                }
                Refresh();
            }
        }

        private Dictionary<int, List<Detected>> GetMatches(AnalyzeResult result)
        {
            Dictionary<int, List<Detected>> matches = new Dictionary<int, List<Detected>>();
            foreach (AnalyzeMatch match in result.Matches)
            {
                if (!matches.ContainsKey(match.Distance))
                    matches.Add(match.Distance, new List<Detected>());
                Detected detected = new Detected(GetTileType(match.Type), GetQuality(match.Quality), GetDirection(match.Direction));
                matches[match.Distance].Add(detected);
            }
            foreach (List<Detected> match in matches.Values)
            {
                int specialCount = 0;
                foreach (Detected d in match)
                {
                    if (Detected.isSpecialType(d.Type))
                    {
                        specialCount++;
                    }
                }
                if (match.Count == specialCount)
                {
                    match.Add(new Detected(TileType.Nothing, Quality.Unknown));
                }
            }

            return matches;
        }

        public void SetResult(int x, int y, AnalyzeResult result)
        {
            if (tileStatus[x, y].Result != null && tileStatus[x, y].Result != result)
            {
                Remove(tileStatus[x, y].Result);
                Refresh();
            }
            if (results.Contains(result))
            {
                Remove(result);
                Refresh();
            }

            Dictionary<int, List<Detected>> matches = GetMatches(result);

            int maxDistance = matches.Keys.Max();
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
            root.SetAttribute("version", "1.2");
            root.SetAttribute("width", this.SizeX.ToString());
            root.SetAttribute("height", this.SizeY.ToString());

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    TileStatus status = tileStatus[x, y];
                    if (status.Type != TileType.Nothing && status.Type != TileType.Unknown)
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
                    if (match.Direction != null)
                        node.SetAttribute("direction", match.Direction);
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

        public static AnalyzeMap Load_1_2(XPathNavigator nav)
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
                    String direction = (String)entry.Current.Evaluate("string(@direction)");
                    int distance = (int)(Double)entry.Current.Evaluate("number(@distance)");

                    if (type != null && type.Length == 0)
                        type = null;
                    if (quality != null && quality.Length == 0)
                        quality = null;
                    if (direction != null && direction.Length == 0)
                        direction = null;

                    matches.Add(new AnalyzeMatch(distance, type, quality, direction));
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
            if ("1.0".Equals(version) || "1.1".Equals(version) || "1.2".Equals(version))
            {
                return Load_1_2(nav);
            }
            else
            {
                throw new Exception("Invalid version " + version);
            }
        }

    }
}
