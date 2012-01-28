using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using StrategyTester.TimeSeries;
using Rhino.Mocks;

namespace StrategyTester.TimeSeries.Tests
{
    [TestFixture]
    class OHLCVIntervalReaderTests
    {
        [Test]
        public void CanReadFromValidStream()
        {
            string testString = "26/09/2008,1204.47,1215.77,1187.54,1213.27,5383610000,1213.27";
            MockRepository mocks = new MockRepository();
            TextReader sourceReader  = mocks.Stub<TextReader>();
     
                using (mocks.Record())
                {
                    SetupResult
                        .For(sourceReader.ReadLine())
                        .Return(testString);
                }

                IParseIntervals intervalParser = mocks.Stub<IParseIntervals>();
                
            using (mocks.Record())
                {
                    SetupResult
                        .For(intervalParser.Parse("qwert", 0))
                        .IgnoreArguments()
                        .Return(new OHLCVInterval() 
                        {
                             Close = 1213.27,
                              DataSource="Stub",
                               DateTime=new DateTime(2008,09,26),
                                High=1215.77,
                                 Index=0,
                                  Instrument="StubInstrument",
                                   Interval="Day",
                                     Low=1187.54,
                                      Open=1204.47,
                                       Volume=5383610000,
                                       Id = "StubInstrument" + new DateTime(2008,09,26).Ticks.ToString()
                        }
                        );
                }

            bool hasHeader = false;

          OHLCVIntervalReader reader = new OHLCVIntervalReader(sourceReader, intervalParser, hasHeader);

          foreach (var interval in reader)
          {
              Assert.AreEqual(new DateTime(2008, 09, 26), interval.DateTime);
              Assert.IsFalse(Math.Abs(1204.47 - interval.Open) > double.Epsilon);
              Assert.IsFalse(Math.Abs(1215.77 - interval.High) > double.Epsilon);
              Assert.IsFalse(Math.Abs(1187.54 - interval.Low) > double.Epsilon);
              Assert.IsFalse(Math.Abs(1213.27 - interval.Close) > double.Epsilon);
              Assert.IsFalse(Math.Abs(5383610000 - interval.Volume) > double.Epsilon);
              Assert.AreEqual(0, interval.Index);
              Assert.AreEqual("Stub", interval.DataSource);
              Assert.AreEqual("StubInstrument", interval.Instrument);
              Assert.AreEqual("StubInstrument" + new DateTime(2008, 09, 26).Ticks.ToString(), interval.Id);
              break;
          }
        }
    }
}
