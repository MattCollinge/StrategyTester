using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Bson.Serialization;

namespace StrategyTester.TimeSeries
{
   public class OHLCVIntervalRepository
    {
       private static MongoServer server;
       private static MongoDatabase db;
       private string collectionName = "OHLCV";

       static OHLCVIntervalRepository()
       {
           string connectionString = "mongodb://localhost";
           server = MongoServer.Create(connectionString);
           db = server.GetDatabase("EoDData");

           BsonClassMap.RegisterClassMap<OHLCVInterval>(cm =>
           {
               cm.AutoMap();
               cm.SetIdMember(cm.GetMemberMap(c => c.Id));
           });
           
       }

       public IEnumerable<OHLCVInterval> GetByTimeSpan(string instrument, DateTime from, DateTime to)
       {
           var collection = db.GetCollection<OHLCVInterval>(collectionName); 
           var mongoQuery = Query.GT("DateTime", from).LT(to);
           
           foreach (var item in collection.Find(mongoQuery))
           {
               yield return item;
           } 
       }

       public void Save(OHLCVInterval intervalToSave)
       {
           var collection = db.GetCollection<OHLCVInterval>(collectionName); //intervalToSave.Instrument); 

           collection.Insert<OHLCVInterval>(intervalToSave);
 
       }
    }
}
