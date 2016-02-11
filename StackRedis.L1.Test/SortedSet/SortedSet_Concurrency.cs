using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StackRedis.L1.Test.SortedSet
{
    [TestClass]
    public class SortedSet_Concurrency : UnitTestBase
    {
        [TestMethod]
        public void SortedSet_Concurrency_Add()
        {
            Random random = new Random();
            object lockObj = new object();

            List<Task> tasks = new List<Task>();
            
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    //Insert some random values
                    int rand = random.Next(0, 20);
                    Thread.Sleep(rand);

                    lock(lockObj)
                    {
                        _memDb.SortedSetAdd("key", "mem" + rand, rand);
                    }

                    //Ensure it was added
                    Assert.AreEqual(1, _redisDirectDb.SortedSetRangeByScore("key", rand, rand).Length);

                    //Retrieve some random values
                    int from = random.Next(0, 10);
                    int to = random.Next(11, 20);
                    
                    lock(lockObj)
                    {
                        var memResult = _memDb.SortedSetRangeByScoreWithScores("key", from, to);
                        var redisResult = _memDb.SortedSetRangeByScoreWithScores("key", from, to);
                        Assert.AreEqual(memResult.Length, redisResult.Length);
                        System.Diagnostics.Debug.WriteLine($"Requesting {from} to {to} resulted in {memResult.Length} items.");
                    }

                    lock(lockObj)
                    {
                        //Remove some random values
                        from = random.Next(0, 10);
                        to = random.Next(11, 20);
                        _memDb.SortedSetRemoveRangeByScore("key", from, to);
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}
