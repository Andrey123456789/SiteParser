using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestTask2;
using TestTask2.Controllers;
using TestTask2.Models;
using TestTask2.Tests.FakeClasses;

namespace TestTask2.Tests.Controllers
{
    [TestClass]
    public class ValuesControllerTest
    {
        ValuesController controller;
        FakeEFContext context = new FakeEFContext();

        [TestInitialize]
        public void Init()
        {
            controller = new ValuesController(context);
        }


        [TestMethod]
        public void GetProduct()
        {
            var result = controller.GetProduct(1);
            var js = new JavaScriptSerializer();
            Assert.IsNotNull(result);
            var p = context.Products.First();
            var img = p.Images.First();
            
            var pExt = new ProductExt(p, new string[] { img.Extension + "; base64,"+Convert.ToBase64String(img.Picture) });
            Assert.AreEqual(js.Serialize(result), js.Serialize(pExt));
        }

        [TestMethod]
        public void GetProducts()
        {
            var pdp = new ParseDomainParams("http://via.placeholder.com/", new HashSet<string>(), new HashSet<string>(), new string[] { "`" }, "", AgilityPackClasses.DescriptionGetKind.dgkLongest, AgilityPackClasses.SearchPriceKind.spkInner);
            var result = controller.GetProducts(pdp);
            var js = new JavaScriptSerializer();
            Assert.AreEqual(js.Serialize(result.First()), js.Serialize(context.Products.First()));
            Assert.AreEqual(result.Count(), context.Products.Count());
        }

        

    }
}
