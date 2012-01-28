using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StrategyTester.TimeSeries
{
    public class OHLCVIntervalReader : IEnumerable<OHLCVInterval>
    {
        private System.IO.TextReader sourceReader;
        private IParseIntervals intervalParser;
        private bool hasHeader;

        public OHLCVIntervalReader(System.IO.TextReader sourceReader, IParseIntervals intervalParser, bool hasHeader)
        {
            this.sourceReader = sourceReader;
            this.intervalParser = intervalParser;
            this.hasHeader = hasHeader;

            if (intervalParser == null)
                throw new ArgumentException("intervalParser must not be null");

            this.intervalParser = intervalParser;

            //sourceFile = new FileInfo(fileName);

             if (sourceReader == null)
                throw new ArgumentException(string.Format("Stream must not be null"));

          //  reader = new StreamReader(sourceFile.OpenRead());

            this.sourceReader = sourceReader;

            if (hasHeader)
                this.sourceReader.ReadLine();
        }

        public IEnumerator<OHLCVInterval> GetEnumerator()
        {
            int index = 0;
            while (sourceReader.Peek() != -1)
            {
                yield return intervalParser.Parse(sourceReader.ReadLine(), index);
                index++;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
