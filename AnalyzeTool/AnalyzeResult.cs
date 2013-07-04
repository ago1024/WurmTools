using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AnalyzeTool
{
    public class AnalyzeResult
    {
        private List<AnalyzeMatch> matches;
        private int x;
        private int y;
        private bool positionSet;

        public AnalyzeResult(List<AnalyzeMatch> matches)
        {
            this.matches = matches;
            this.x = -1;
            this.y = -1;
            this.positionSet = false;
        }

        public int X
        {
            get { return x; }
            set { x = value; positionSet = true; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; positionSet = true;  }
        }

        public bool PositionSet
        {
            get { return positionSet; }
        }

        public void ResetPosition()
        {
            x = -1;
            y = -1;
            positionSet = false;
        }

        public List<AnalyzeMatch> Matches
        {
            get { return matches; }
        }

        override
        public String ToString()
        {
            StringWriter writer = new StringWriter();
            int distance = 0;

            if (matches.Count == 0)
            {
                writer.Write("Nothing");
            }
            else
            {
                foreach (AnalyzeMatch match in matches)
                {
                    if (match.Type == null)
                        continue;

                    if (distance != match.Distance)
                    {
                        if (distance != 0)
                        {
                            writer.Write(" ");
                        }
                        distance = match.Distance;
                        writer.Write("{0}: ", distance);
                    }
                    else
                    {
                        writer.Write(", ");
                    }
                    if (match.Quality != null)
                    {
                        writer.Write("{0} ({1})", match.Type, match.Quality);
                    }
                    else
                    {
                        writer.Write("{0}", match.Type);
                    }
                }
            }
            return writer.ToString();
        }
    }
}
