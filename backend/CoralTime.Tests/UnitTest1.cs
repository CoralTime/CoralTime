using CoralTime.DAL.Models;
using GeekLearning.Testavior.Environment;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace CoralTime.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethodNonAuth()
        {
            var testEnvironment = new TestEnvironment<Startup, TestStartupConfigurationService>();

            var response = testEnvironment.Client.GetAsync("/api/v1/Test/ping").Result;
            response.EnsureSuccessStatusCode();

            // Test result content
            var result = JsonConvert.DeserializeObject<string>(response.Content.ReadAsStringAsync().Result);

            Assert.AreEqual(result, "I'm alive!");
        }

        [TestMethod]
        public void TestMethodAuth()
        {
            var testEnvironment = new TestEnvironment<Startup, TestStartupConfigurationService>();

            var response = testEnvironment.Client.GetAsync("/api/v1/odata/Members").Result;
            response.EnsureSuccessStatusCode();

            // Test result content
            var result = (JObject)JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
            var membersList = JsonConvert.DeserializeObject<List<Member>>(result["value"].ToString());

            Assert.AreNotEqual(membersList.Count, 0);
        }
    }
}