﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestTask2.AgilityPackClasses;
using TestTask2.Models;
using TestTask2.StringUtils;

namespace TestTask2.Tests.HtmlAgilityClasses
{
    [TestClass]
    public class HtmlDocumentExtTest
    {
        HtmlDocument doc;

        byte[] img;

        List<Product> prTest;

        Currencies CurRegexes;

        CurrencyRegexes currency;

        string[] currencySeparators;

        string decimalSeparator;

        JavaScriptSerializer js;

        [TestInitialize]
        public void Init()
        {
            img = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10, 0, 0, 0, 13, 73, 72, 68, 82, 0, 0, 1, 44, 0, 0, 1, 44, 4, 3, 0, 0, 0, 139, 83, 147, 70, 0, 0, 0, 27, 80, 76, 84, 69, 0, 153, 255, 150, 150, 150, 56, 151, 215, 93, 151, 189, 131, 150, 163, 112, 150, 176, 18, 152, 241, 75, 151, 202,
                37, 152, 228, 148, 65, 161, 73, 0, 0, 0, 9, 112, 72, 89, 115, 0, 0, 14, 196, 0, 0, 14, 196, 1, 149, 43, 14, 27, 0, 0, 2, 142, 73, 68, 65, 84, 120, 156, 237, 215, 59, 111, 219, 48, 16, 192, 241, 243, 83, 26, 125, 142, 148, 100, 180, 209, 47, 16, 1, 105, 231, 104, 168, 187,
                198, 112, 80, 116, 148, 129, 22, 93, 227, 161, 143, 209, 70, 145, 239, 221, 35, 69, 202, 70, 45, 116, 163, 166, 255, 15, 1, 28, 233, 14, 32, 205, 199, 145, 22, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 254, 231, 93, 189, 107,
                236, 35, 127, 44, 143, 238, 241, 79, 253, 165, 39, 105, 252, 88, 126, 184, 136, 198, 228, 116, 38, 170, 186, 180, 207, 181, 234, 157, 107, 223, 30, 95, 175, 179, 42, 123, 189, 58, 71, 67, 114, 66, 7, 221, 109, 181, 145, 185, 150, 149, 30, 69, 246, 186, 211, 251, 171, 164, 76,
                203, 247, 122, 219, 69, 99, 114, 66, 245, 70, 114, 125, 144, 73, 209, 204, 235, 103, 27, 150, 141, 124, 45, 175, 146, 166, 69, 35, 167, 155, 46, 26, 147, 211, 201, 221, 183, 62, 60, 203, 222, 38, 242, 116, 107, 227, 176, 178, 87, 205, 191, 89, 39, 27, 162, 172, 236, 162, 33, 57,
                161, 177, 186, 86, 23, 178, 94, 216, 152, 220, 181, 143, 245, 213, 226, 114, 209, 92, 187, 104, 72, 78, 40, 115, 51, 118, 122, 146, 202, 230, 100, 82, 186, 63, 241, 255, 187, 110, 60, 137, 159, 57, 241, 195, 41, 121, 209, 69, 67, 114, 106, 174, 165, 215, 182, 225, 187, 182, 151,
                254, 173, 77, 213, 246, 60, 85, 174, 211, 33, 26, 146, 19, 251, 110, 75, 102, 187, 242, 45, 205, 150, 231, 110, 237, 11, 27, 177, 110, 62, 231, 135, 101, 23, 13, 201, 105, 213, 106, 51, 85, 31, 221, 190, 151, 169, 43, 14, 251, 133, 127, 159, 233, 235, 180, 107, 123, 170, 182, 93,
                99, 52, 36, 167, 165, 174, 20, 181, 45, 53, 109, 195, 97, 230, 234, 205, 122, 25, 147, 70, 106, 91, 54, 70, 67, 114, 234, 110, 89, 139, 190, 17, 109, 70, 174, 71, 163, 208, 173, 245, 205, 185, 56, 141, 92, 231, 99, 52, 36, 167, 237, 150, 188, 233, 115, 239, 104, 205, 244, 162,
                148, 231, 219, 114, 216, 209, 178, 253, 182, 232, 91, 91, 182, 224, 111, 46, 146, 102, 58, 236, 218, 242, 21, 187, 103, 39, 218, 226, 90, 94, 36, 89, 121, 31, 110, 39, 102, 191, 197, 207, 139, 43, 69, 227, 80, 183, 214, 161, 91, 153, 198, 166, 127, 29, 253, 172, 197, 104, 72, 78,
                200, 159, 33, 54, 47, 135, 182, 112, 103, 231, 42, 239, 10, 87, 44, 91, 238, 56, 178, 89, 139, 209, 67, 242, 42, 63, 9, 135, 79, 223, 153, 88, 221, 215, 97, 241, 87, 237, 225, 51, 224, 153, 232, 54, 84, 21, 110, 16, 247, 225, 142, 112, 244, 161, 185, 62, 172, 195, 154, 63, 220,
                250, 47, 16, 163, 33, 57, 161, 185, 126, 182, 65, 88, 245, 221, 183, 102, 182, 198, 67, 21, 216, 151, 141, 184, 210, 58, 216, 125, 75, 170, 98, 87, 23, 210, 119, 59, 181, 219, 67, 174, 109, 227, 19, 253, 248, 232, 46, 20, 195, 221, 78, 191, 89, 145, 223, 72, 223, 93, 222, 45, 172,
                109, 219, 197, 249, 86, 213, 46, 168, 3, 222, 229, 229, 71, 253, 201, 26, 148, 252, 165, 253, 49, 243, 22, 127, 249, 140, 93, 7, 246, 161, 12, 100, 47, 197, 207, 139, 104, 76, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 72, 233, 47,
                157, 120, 114, 82, 107, 53, 43, 118, 0, 0, 0, 0, 73, 69, 78, 68, 174, 66, 96, 130 };

            doc = new HtmlDocument();

            doc.DocumentNode.InnerHtml =

        @"<!DOCTYPE html>" +
        @"<html>" +
        @"<head>" +
        @"</head>" +
        @"<body>" +
        @"<script>" +
        @"var s='< div > " +
        @"	<span>Товар 4 deleteWord</span>" +
        @"	<img src='http://via.placeholder.com/300/09f.png' />" +
        @"	<div>10 грн</div>" +
        @"</div>';" +
        @"</script>" +
        @"<div>" +
        @"	<div>10 грн</div>" +
        @"</div>" +
        @"<div>" +
        @"	<span>Товар 1 deleteWord</span>" +
        @"	<img src='http://via.placeholder.com/300/09f.png' />" +
        @"	<div>10"+
        @" 0`0'" +
        @"0.0 0'0'1 грн </div>" +
        @"</div>" +
        @"<div><img src='http://via.placeholder.com/300/09f.png' /></div>" +
        @"<div>" +
        @"	<span>Товар 2 deleteWord</span>" +
        @"	<img src='http://via.placeholder.com/300/09f.png' />" +
        @"	<img src='http://via.placeholder.com/300/09f.png' />" +
        @"	<div>20 грн</div>" +
        @"</div>" +
        @"<div>" +
        @"" +
        @"<div><div><div><div>Товар 3 deleteWord</div></div></div></div>" +
        @"	<img src='http://via.placeholder.com/300/09f.png' />" +
        @"	<div>30 грн</div>" +
        @"</div>" +
        @"<div><div><div><div>Товар неликвидный</div></div></div></div>" +
        @"	<img src='http://via.placeholder.com/300/09f.png' />" +
        @"	<div>1005000 грн</div>" +
        @"</div>" +
        @"</body>" +
        @"</html>";
            decimalSeparator = ".";
            currencySeparators = new string[] { "`", "'", "\t", "\n",};
            CurRegexes = new Currencies(decimalSeparator, currencySeparators );
            currency = CurRegexes.GetAllCurRegexes().First();
            js = new JavaScriptSerializer();
            prTest = new List<Product>()
            {
                new Product("http://via.placeholder.com/","Товар 2",20,new HashSet<Image>(){new Image(img,"png"), new Image(img,"png") },currency),
                new Product("http://via.placeholder.com/","Товар 3",30,new HashSet<Image>(){new Image(img,"png") },currency),
                new Product("http://via.placeholder.com/","Товар 1",10000.0001m,new HashSet<Image>(){new Image(img,"png") },currency),
            };

            
        }

        [TestMethod]
        public void GetProductsHashSet_InnerLongest()
        {
            var pdp = new ParseDomainParams("http://via.placeholder.com/", new HashSet<string>() { " deleteWord", "\t" }, new HashSet<string>() { "неликвидный" }, currencySeparators, decimalSeparator, AgilityPackClasses.DescriptionGetKind.dgkLongest, AgilityPackClasses.SearchPriceKind.spkInner);
            var products = doc.GetProducts(pdp);
            Debug.WriteLine(js.Serialize(products));
            Debug.WriteLine(js.Serialize(prTest));
            Assert.IsNotNull(products);
            List<Product> pList = products.ToList();
            pList.Sort((x, y) =>
    x.Price.CompareTo(y.Price));
            Assert.AreEqual(js.Serialize(pList), js.Serialize(prTest));

        }

        [TestMethod]
        public void GetProductsHashSet_OuterLongest()
        {
            var pdp = new ParseDomainParams("http://via.placeholder.com/", new HashSet<string>() { " deleteWord", "\t" }, new HashSet<string>() { "неликвидный" }, currencySeparators, decimalSeparator, AgilityPackClasses.DescriptionGetKind.dgkLongest, AgilityPackClasses.SearchPriceKind.spkOuter);
            var products = doc.GetProducts(pdp);
            Debug.WriteLine(js.Serialize(products));
            Debug.WriteLine(js.Serialize(prTest));
            Assert.IsNotNull(products);
            List<Product> pList = products.ToList();
            pList.Sort((x, y) =>
                x.Price.CompareTo(y.Price));
            Assert.AreEqual(js.Serialize(pList), js.Serialize(prTest));

        }

        [TestMethod]
        public void GetProductsHashSet_InnerFull()
        {
            var pdp = new ParseDomainParams("http://via.placeholder.com/", new HashSet<string>() { " deleteWord", "\t" }, new HashSet<string>() { "неликвидный" }, currencySeparators, decimalSeparator, AgilityPackClasses.DescriptionGetKind.dgkFull, AgilityPackClasses.SearchPriceKind.spkInner);
            var products = doc.GetProducts(pdp);
            Debug.WriteLine(js.Serialize(products));
            Debug.WriteLine(js.Serialize(prTest));
            Assert.IsNotNull(products);
            List<Product> pList = products.ToList();
            pList.Sort((x, y) =>
                x.Price.CompareTo(y.Price));
            Assert.AreEqual(js.Serialize(pList), js.Serialize(prTest));

        }

        [TestMethod]
        public void GetProductsHashSet_OuterFull()
        {
            var pdp = new ParseDomainParams("http://via.placeholder.com/", new HashSet<string>() { " deleteWord", "\t" }, new HashSet<string>() { "неликвидный" }, currencySeparators, decimalSeparator, AgilityPackClasses.DescriptionGetKind.dgkFull, AgilityPackClasses.SearchPriceKind.spkOuter);
            var products = doc.GetProducts(pdp);
            Debug.WriteLine(js.Serialize(products));
            Debug.WriteLine(js.Serialize(prTest));
            Assert.IsNotNull(products);
            List<Product> pList = products.ToList();
            pList.Sort((x, y) =>
                x.Price.CompareTo(y.Price));
            Assert.AreEqual(js.Serialize(pList), js.Serialize(prTest));

        }
    }
}







