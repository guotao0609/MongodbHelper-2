using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongodbHelper;
using System.Collections.Generic;
using System.Linq;

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
            string schoolid = Guid.NewGuid().ToString();
            var p = new People() { Name = "nidie", Age = 9, SchoolGuid = schoolid };
            testdb.Insert<People>(p);
            var s = new School() { SchoolGuid = schoolid, Name = "智障三中" };
            testdb.Insert<School>(s);
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
            var result = testdb.Update<People>(new System.Collections.Generic.Dictionary<string, object>() { { "Name", "hahaha" }, { "Age", 110 } }, p => p.Primaryid.Equals("58184fec6dd4fa9a2ac177ec"));
            Assert.AreEqual(true, result > 0);

        }


        [TestMethod]
        public void TestMapReduce()
        {
            string map = @"function() {
        emit(this.Name, 9);
    }";
            string reduce = @"function(key, values) {return key;}";
            testdb.MapReduce<People, People>(new MapReduceOptionsProxy<People, People>() { Map = map, Reduce = reduce, DatabaseName = "Tempdb", CollectionName = "People" });

        }

        [TestMethod]
        public void TestJoin()
        {
            //var schools = testdb.GetIQueryable<School>();
            //var peoples = testdb.GetIQueryable<People>();
            //var r = from t in schools
            //        join u in peoples on t.SchoolGuid equals u.SchoolGuid into joined
            //        select new { t, joined };
            //var r1 = r.ToList();
            var schools = testdb.GetIQueryable<School>();
            var peoples = testdb.GetIQueryable<People>();
            //var r = from t in schools
            //        join u in peoples on t.SchoolGuid equals u.SchoolGuid into joined
            //        from j in joined.DefaultIfEmpty()
            //        join u2 in peoples on j.Age equals u2.Age into joined2
            //        from j2 in joined2.DefaultIfEmpty()
            //        select j2;
            var r2 = from s in schools
                     join p in peoples on s.SchoolGuid equals p.SchoolGuid into joined
                     from j in joined.DefaultIfEmpty()
                     select j;
            var r3=from s in r2
                    join p in peoples on s.Age equals p.Age into joined
                    from j in joined.DefaultIfEmpty()
                    select j;
            var r2r = r2.ToList();


            //var schools2 = from s in schools
            //               join p in peoples on s.SchoolGuid equals p.SchoolGuid
            //               where p.Age.Equals(9)
            //               select p;
            //var r3 = from s in schools2
            //         join p in schools on s.SchoolGuid equals p.SchoolGuid
            //         select s;
            //var r2r = r3.ToList();

            //var rr = r.ToList();
        }

	[TestMethod]
        public void TestMethod2()
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            //            MapReduceOptionsProxy<UserTag, UserTag> options = new MapReduceOptionsProxy<UserTag, UserTag>();
            //            options.Map = @"function() {
            //                    emit(this,{ this.usertype,this.userid});
            //                }";
            //            options.Reduce = @"function(key, values) {return {utype: values.usertype,uid:values.userid};}";
            //            options.OutputEnum = MapReduceOutputOptionsEnum.Replace;
            //            options.DatabaseName = "dm2usermappingdb";
            //            options.CollectionName = "usertagtemp";
            //            options.Limit = 10000;
            //            db.MapReduce<UserTag>(options);


            //var r = from u2 in users2
            //        join u in users on u2.cookie equals u.cookie
            //        select u;
            //var r1 = r.ToList(); ;

            //var t = db.QueryExt<UserTag, IEnumerable<UserTagTemp>>(q => q.Take(10).Select(m => new UserTagTemp() { userid = m.userid, usertype = m.usertype }).AsEnumerable());
            //db.BatchInsert<UserTagTemp>(t);
            watch.Stop();
            System.Diagnostics.Debug.WriteLine("耗时：" + watch.ElapsedMilliseconds);

        }
    }

    [MappingInformation("Test", "People")]
    public class People : CollectionEntityBase
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string SchoolGuid { get; set; }
    }

    [MappingInformation("Test", "School")]
    public class School : CollectionEntityBase
    {
        public string SchoolGuid { get; set; }
        public string Name { get; set; }
    }
	
    [MappingInformation("dm2usertagdb", "usertag")]
    public class UserTag : CollectionEntityBase
    {
        public string hyuniqueid { get; set; }
        public string companyhyuniqueid { get; set; }
        public string brandhyuniqueid { get; set; }
        public string userhyuniqueid { get; set; }
        public string usertype { get; set; }
        public string userid { get; set; }
        public string treetagmappinghyuniqueid { get; set; }
        public string tagcode { get; set; }
        public string tagname { get; set; }
        public double tagscore { get; set; }
        public string treecode { get; set; }
        public string treename { get; set; }
        public string treehyuniqueid { get; set; }
        public string taskid { get; set; }
        public string ruleid { get; set; }
        public string ip { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public string headurl { get; set; }
        public string siteid { get; set; }
        public string channelid { get; set; }
        public string platformid { get; set; }
        public DateTime logcreatetime { get; set; }
        public DateTime createtime { get; set; }
    }

    [MappingInformation("dm2usermappingdb", "usermapping")]
    public class UserMapping : CollectionEntityBase
    {
        public string hyuniqueid { get; set; }
        public string companyhyuniqueid { get; set; }
        public string brandhyuniqueid { get; set; }
        public string cookie { get; set; }
        public string cellphone { get; set; }
        public string socialmediatype { get; set; }
        public string socialmediavalue { get; set; }
        public string ip { get; set; }
        public string city { get; set; }
        public string region { get; set; }
        public string country { get; set; }
        public string headurl { get; set; }
        public string siteid { get; set; }
        public string channelid { get; set; }
        public string platformid { get; set; }
        public DateTime logcreatetime { get; set; }
        public DateTime createtime { get; set; }
    }

    [MappingInformation("dm2usermappingdb", "usertagtemp")]
    public class UserTagTemp : CollectionEntityBase
    {
        public string usertype { get; set; }
        public string userid { get; set; }
    }
}
