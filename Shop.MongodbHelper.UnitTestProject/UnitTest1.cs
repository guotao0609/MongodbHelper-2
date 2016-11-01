using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shop.MongodbHelper;

namespace Shop.MongodbHelper.UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        MongodbAccess testdb = new MongodbAccess("Test");

        [TestMethod]
        public void TestQuery()
        {
            var d = testdb.Query<People>(p => p.Age == 16);
            Assert.AreEqual(true, d.Count > 0);
        }

        [TestMethod]
        public void TestInsert()
        {
            Exception ex = null;
            try
            {

                testdb.Insert<People>(new People() { Id = 2, Name = "lily", Age = 18 });
            }
            catch (Exception e) { ex = e; }
            Assert.AreEqual(ex, null);
        }


        [TestMethod]
        public void TestDelete()
        {
            var result = testdb.Delete<People>(p => p.Age == 16);
            Assert.AreEqual(true, result > 0);
        }

        [TestMethod]
        public void TestUpdate()
        {
            var result = testdb.Update<People>(new People() { Id = 2, Name = "lily2", Age = 17 }, p => p.Age == 18);
            Assert.AreEqual(true, result > 0);

        }
    }

    [CollectionName("People")]
    public class People
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
