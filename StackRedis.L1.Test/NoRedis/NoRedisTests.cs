using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackRedis.L1.Test.NoRedis
{
    [TestClass]
    public class NoRedisTests
    {
        [TestMethod]
        public void NoRedis_StringGet()
        {
            RedisL1Database db = new RedisL1Database("unique str");
            db.StringSet("nodbkey", "value");
            Assert.AreEqual("value", (string)db.StringGet("nodbkey"));
        }
    }
}
