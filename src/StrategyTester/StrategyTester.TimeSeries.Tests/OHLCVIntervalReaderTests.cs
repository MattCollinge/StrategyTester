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
                             Close = 1213.27f,
                             // DataSource="Stub",
                               DateTime=new DateTime(2008,09,26),
                                High=1215.77f,
                                 Index=0,
                                  Instrument="StubInstrument",
                                   //Interval="Day",
                                   Exchange="StubExchange",
                                     Low=1187.54f,
                                      Open=1204.47f,
                                       Volume=5383610000,
                                       //Id = Guid.NewGuid()//"StubInstrument" + new DateTime(2008,09,26).Ticks.ToString()
                        }
                        );
                }

            bool hasHeader = false;

          OHLCVIntervalReader reader = new OHLCVIntervalReader(sourceReader, intervalParser, hasHeader);

          foreach (var interval in reader)
          {
              Assert.AreEqual(new DateTime(2008, 09, 26), interval.DateTime);
              Assert.IsFalse(Math.Abs(1204.47f - interval.Open) > Single.Epsilon);
              Assert.IsFalse(Math.Abs(1215.77f - interval.High) > Single.Epsilon);
              Assert.IsFalse(Math.Abs(1187.54f - interval.Low) > Single.Epsilon);
              Assert.IsFalse(Math.Abs(1213.27f - interval.Close) > Single.Epsilon);
              Assert.AreEqual(5383610000, interval.Volume);
              Assert.AreEqual(0, interval.Index);
              Assert.AreEqual("StubExchange", interval.Exchange);
           
              //Assert.AreEqual("Stub", interval.DataSource);
              Assert.AreEqual("StubInstrument", interval.Instrument);
             // Assert.AreEqual("StubInstrument" + new DateTime(2008, 09, 26).Ticks.ToString(), interval.Id);
              break;
          }
        }
    }
}
