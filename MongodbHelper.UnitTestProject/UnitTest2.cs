using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongodbHelper;

namespace MongodbLibrary.UnitTestProject
{
    [TestClass]
    public class UnitTest2
    {
        MongodbAccess testdb = new MongodbAccess("dm2usermappingdb", "mongodb://ec2-54-222-150-141.cn-north-1.compute.amazonaws.com.cn:27017");
        MongodbAccess testdb2 = new MongodbAccess("dm2companydb", "mongodb://ec2-54-222-150-141.cn-north-1.compute.amazonaws.com.cn:27017");
        [TestMethod]
        public void TestMethod1()
        {
            var s = testdb.Query<UserMapping>(t => true);
            var d = testdb2.Query<Brand>(t => true);
        }
    }

    [CollectionName("brand")]
    public class UserMapping : CollectionEntityBase
    {
        public string hyuniqueid { get; set; }
        public string companyhyuniqueid { get; set; }
        public string brandhyuniqueid { get; set; }
        public string cookie { get; set; }
        public string qqid { get; set; }
        public string sinaid { get; set; }
        public string contactid { get; set; }
        public string cellphone { get; set; }
        public string siteid { get; set; }
        public string channelid { get; set; }
        public string platformid { get; set; }
        public DateTime createtime { get; set; }
    }

    [CollectionName("brand")]
    public class Brand : CollectionEntityBase
    {
        public string hyuniqueid { get; set; }
        public string companyhyuniqueid { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string createtime { get; set; }
    }
}
