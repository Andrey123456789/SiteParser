using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestTask2;
using TestTask2.Controllers;

namespace TestTask2.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {

        HomeController controller;

        [TestInitialize]
        public void Init()
        {
            controller = new HomeController();
        }

        [TestMethod]
        public void Index()
        {
            // Действие
            ViewResult result = controller.Index() as ViewResult;

            // Утверждение
            Assert.IsNotNull(result);
            Assert.AreEqual("Home Page", result.ViewBag.Title);
        }

        [TestMethod]
        public void Product()
        {
            // Действие
            ViewResult result = controller.Product(1) as ViewResult;

            // Утверждение
            Assert.IsNotNull(result);
            Assert.AreEqual("Product Information", result.ViewBag.Title);
        }

        [TestMethod]
        public void ProductNotExists()
        {
            // Действие
            ViewResult result = controller.Product(null) as ViewResult;

            // Утверждение
            Assert.IsNull(result);
        }
    }
}
