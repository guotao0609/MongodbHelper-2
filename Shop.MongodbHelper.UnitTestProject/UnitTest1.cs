using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shop.MongodbHelper;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Shop.MongodbHelper.UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        MongodbAccess testdb = new MongodbAccess("Test");

        [TestMethod]
        public void TestQuery()
        {
            var d = testdb.Query<People>(p => p.Age.Equals(16) || p.Age.Equals(18));
            var single = testdb.Query<People>(p => p.Id.Equals(ObjectId.Parse("58185ddde843a019d85c9221")));
            var all = testdb.Query<People>(p => true);
            Assert.AreEqual(true, d.Count > 0);
        }

        [TestMethod]
        public void TestInsert()
        {
            var p = new People() { Name = "nima", Age = 9 };
            testdb.Insert<People>(p);
            Assert.AreEqual(true, !string.IsNullOrEmpty(p.Id.ToString()));
        }


        [TestMethod]
        public void TestDelete()
        {
            var result = testdb.Delete<People>(p => p.Id.Equals(ObjectId.Parse("581850006dd4fa9a2ac177ed")));
            Assert.AreEqual(true, result > 0);
        }

        [TestMethod]
        public void TestUpdate()
        {
            var result = testdb.Update<People>(new System.Collections.Generic.Dictionary<System.Linq.Expressions.Expression<Func<People, object>>, object>() { { p => p.Name, "nimei" }, { p => p.Age, 10 } }, p => p.Equals(ObjectId.Parse("58185ddde843a019d85c9221")));
            Assert.AreEqual(true, result > 0);

        }
    }

    [CollectionName("People")]
    public class People
    {
        [BsonId]
        public ObjectId Id;
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
