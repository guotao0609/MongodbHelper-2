using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongodbHelper;
using System.Collections.Generic;

namespace MongodbHelper.UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        MongodbAccess testdb = new MongodbAccess("mongodb://localhost:27017");

        [TestMethod]
        public void TestQuery()
        {
            //var d = testdb.Query<People>(p => p.Age.Equals(16) || p.Age.Equals(18));
            //var single = testdb.Query<People>(p => p.Primaryid.Equals("58185ddde843a019d85c9221"));
            //var all = testdb.Query<People>(p => true);
            var d = testdb.Query<People, List<People>>(m => m.Where(v => v.Age.Equals(16)).ToList());
            Assert.AreEqual(true, d.Count > 0);
        }

        [TestMethod]
        public void TestInsert()
        {
            var p = new People() { Name = "nima", Age = 9 };
            testdb.Insert<People>(p);
            Assert.AreEqual(true, !string.IsNullOrEmpty(p.Primaryid));
        }


        [TestMethod]
        public void TestDelete()
        {
            var result = testdb.Delete<People>(p => p.Primaryid.Equals("581850006dd4fa9a2ac177ed"));
            Assert.AreEqual(true, result > 0);
        }

        [TestMethod]
        public void TestUpdate()
        {
            var result = testdb.Update<People>(new System.Collections.Generic.Dictionary<System.Linq.Expressions.Expression<Func<People, object>>, object>() { { p => p.Name, "nimei" }, { p => p.Age, 10 } }, p => p.Equals("58185ddde843a019d85c9221"));
            Assert.AreEqual(true, result > 0);

        }
    }

    [ModelMapping("Test","People")]
    public class People : CollectionEntityBase
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
