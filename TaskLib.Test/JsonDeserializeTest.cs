using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Task.ConsoleApp.Web;

namespace TaskLib.Test
{
    [TestClass]
    public class JsonDeserializeTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            string messageStr = "{\"Operation\":\"add\",\"Tasks\":[{\"TaskId\":0,\"UserProfile_Id\":0,\"UserProfile\":null,\"Name\":\"retask\",\"Description\":null,\"CreatedDT\":\"0001-01-01T00:00:00\",\"TargetDT\":null,\"RemindPeriod\":null,\"IsComplited\":false,\"SubTasks\":[],\"SupTaskId\":null,\"SupTask\":null,\"Id\":0}]}";
            Message message = Newtonsoft.Json.JsonConvert.DeserializeObject<Message>(messageStr);

        }

        [TestMethod]
        public void Test2()
        {
            //var r = Newtonsoft.Json.JsonConvert.SerializeObject(
            //    new Message()
            //    {
            //        Operation = OperationType.init,
            //        Tasks = new List<Task>() { new Task() { Name = "asd1" }, new Task() { Name = "zxc" } }
            //    });


            string messageStr = "{Operation:\"init\",Tasks:[{\"SubTasks\":[],\"TaskId\":1,\"UserProfile_Id\":1,\"UserProfile\":null,\"Name\":\"test1 e\",\"Description\":\"test task jqgrid add edit\",\"CreatedDT\":\"2018 - 08 - 28T14: 34:30.767\",\"TargetDT\":\"2018 - 08 - 28T16: 10:00\",\"RemindPeriod\":\"00:17:00\",\"IsComplited\":false,\"SupTaskId\":null,\"SupTask\":null,\"Id\":1},{\"SubTasks\":[],\"TaskId\":13,\"UserProfile_Id\":1,\"UserProfile\":null,\"Name\":\"retaskqat6\",\"Description\":null,\"CreatedDT\":\"2018 - 08 - 29T14: 36:12.44\",\"TargetDT\":null,\"RemindPeriod\":null,\"IsComplited\":false,\"SupTaskId\":null,\"SupTask\":null,\"Id\":13}]}";
            Message message = Newtonsoft.Json.JsonConvert.DeserializeObject<Message>(messageStr);
        }
        [TestMethod]
        public void Test22()
        {
            string messageStr = "{Operation:\"update\",Tasks:[{\"TaskId\":20,\"UserProfile_Id\":0,\"UserProfile\":null,\"Name\":\"T30\",\"Description\":null,\"CreatedDT\":\"2018-08-30T10:11:05\",\"TargetDT\":null,\"RemindPeriod\":\"00:03:30\",\"IsComplited\":false,\"SubTasks\":[],\"SupTaskId\":null,\"SupTask\":null,\"Id\":20}]}";
            Message message = Newtonsoft.Json.JsonConvert.DeserializeObject<Message>(messageStr);
        }

        [TestMethod]
        public void Test()
        {
            var cl = new MyClass() { MyPropertyInt = 1, MyPropertyStr = "one" };
            IMyClass icl = cl as IMyClass;

            string serializeObject = Newtonsoft.Json.JsonConvert.SerializeObject(icl, typeof(IMyClass), new Newtonsoft.Json.JsonSerializerSettings() { });

        }

        [TestMethod]
        public void TestConcreteTypeConvert()
        {
            MyClass[] myClassArray = new MyClass[] { new MyClass(), new MyClass(), new MyClass() };


        }

        public interface IMyClass
        {
            string MyPropertyStr { get; set; }
            string MyPropertyStr2 { get; set; }
        }
        public class MyClass : IMyClass
        {
            public int MyPropertyInt { get; set; }
            public string MyPropertyStr { get; set; }
            public string MyPropertyStr2 { get; set; }
        }
    }

    public class JsonTypeContractResolver<T> : DefaultContractResolver where T : class
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(typeof(T), memberSerialization);
            return properties;
        }
    }

    public static class ObjectExtension
    {
        public static string ToJson<T>(this object obj) where T : class
        {
            string serializedObject = null;
            JsonSerializerSettings settings = new JsonSerializerSettings() { ContractResolver = new JsonTypeContractResolver<T>() };

            if(obj is IEnumerable<T> array)
            {
                StringBuilder strBuilder = new StringBuilder();
                strBuilder.Append("[");
                foreach(var item in array)
                {
                    string itemJson = JsonConvert.SerializeObject(obj, settings);
                    strBuilder.Append(itemJson);
                    strBuilder.Append(", ");
                }
                strBuilder.Remove(strBuilder.Length - 1, 1);
                strBuilder.Append("]");
                serializedObject = strBuilder.ToString();
            }
            else
                serializedObject = JsonConvert.SerializeObject(obj, settings);

            return serializedObject;
        }
    }
}
