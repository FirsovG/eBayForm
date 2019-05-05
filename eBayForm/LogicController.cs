using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using eBayForm.LogicUnits.Exceptions;
using eBayForm.LogicUnits.HtmlTags;
using HtmlAgilityPack;
using Microsoft.Win32;

namespace eBayForm
{
    public sealed class LogicController
    {
        // Html agility pack
        // Class which gonna contain HtmlCode
        HtmlDocument document = new HtmlDocument();
        string templatename;

        public string Document { get => document.DocumentNode.OuterHtml; }

        // TODO: Check if the IE meta-tag at the beginn of the file
        public string ImportHtml()
        {
            // string which contain htmlCode
            string htmlCode;
            // FileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            // Only html files
            openFileDialog.Filter = "html files (*.html)|*.html";
            Nullable<bool> result = openFileDialog.ShowDialog();
            if (result == true)
            {
                // Read the file
                using (StreamReader reader = new StreamReader(openFileDialog.FileName))
                {
                    // Insert content into htmlCode
                    htmlCode = reader.ReadToEnd();
                }
                // Compress htmlHode
                htmlCode = Regex.Replace(htmlCode, @"( |\t|\r?\n)\1+", "$1");
                // Loading the Code
                document.LoadHtml(htmlCode);
                // To diplay the content rightly we need to check compatibility to InternetExplorer

                // Searching for Template meta tag
                HtmlNode templateTag = document.DocumentNode.SelectSingleNode("//head/meta/@itemprop");
                if (templateTag == null)
                {
                    throw new NotATemplateException("Doesn't find meta tag, which contain the Templatename");
                }
                else if (templateTag.Attributes["content"].Value == "Tea")
                {
                    templatename = "Tea";
                }
                else
                {
                    throw new UnknownTemplateException("Unknown Template");
                }

                //// Searching IE meta tag 
                //foreach (var element in document.DocumentNode.SelectNodes("//meta/@http-equiv"))
                //{
                //    // If found we don't to change htmlCode
                //    if (element.Attributes["content"].Value == "IE=10")
                //    {
                //        return htmlCode;
                //    }
                //}
                // Select the head-tag
                HtmlNode head = document.DocumentNode.SelectSingleNode("//head");
                // Creating meta-tag
                HtmlNode meta = document.CreateElement("meta");
                // Adding meta-tag at the beginn of head-tag
                head.PrependChild(meta);
                // Adding IE Attributes
                meta.SetAttributeValue("http-equiv", "X-UA-Compatible");
                meta.SetAttributeValue("content", "IE=11");
                // Returning the HtmlCode as string
                return document.DocumentNode.OuterHtml;
            }
            return null;
        }


        public List<IHtmlTagElement> GetTags()
        {
            List<IHtmlTagElement> htmlTags = new List<IHtmlTagElement>();

            // Getting company logo at the top
            HtmlNode element = document.DocumentNode.SelectSingleNode("//img[@class='logo']");
            htmlTags.Add(new HtmlTagElement(false, "TopCompanyLogo", element.Attributes["src"].Value));

            // Getting 
            int i = 1;
            foreach (HtmlNode node in document.DocumentNode.SelectNodes("//nav//li//a"))
            {
                htmlTags.Add(new HtmlLinkTagElement(true, "SimilarProductsLink" + i++, node.InnerText, node.Attributes["href"].Value));
            }

            // Getting source of the product main image
            element = document.DocumentNode.SelectSingleNode("//img[@class='product_image']");
            htmlTags.Add(new HtmlTagElement(false, "ProductMainImage", element.Attributes["src"].Value));

            // Getting name of the product
            element = document.DocumentNode.SelectSingleNode("//h1[@class='productname']");
            htmlTags.Add(new HtmlTagElement(false, "ProductName", element.InnerText));

            // Getting arguments of the product
            i = 1;
            foreach (HtmlNode node in document.DocumentNode.SelectNodes("//ul[@class='arguments']/li"))
            {
                htmlTags.Add(new HtmlTagElement(true, "Argument" + i++, node.InnerText));
            }

            i = 1;
            foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@class='info_item']"))
            {
                element = node.SelectSingleNode("h2[@class='headline']");
                htmlTags.Add(new HtmlTagElement(true, "Headline" + i, element.InnerText));

                element = node.SelectSingleNode("p[@class='text']");
                htmlTags.Add(new HtmlTagElement(true, "Text" + i, element.InnerText));
                i++;
            }

            i = 1;
            foreach (HtmlNode node in document.DocumentNode.SelectNodes("//footer//li//a"))
            {
                    htmlTags.Add(new HtmlLinkTagElement(true, "FooterNavigationLink" + i++, node.InnerText, node.Attributes["href"].Value));
            }

            element = document.DocumentNode.SelectSingleNode("//p[@class='copyright']/a");
            htmlTags.Add(new HtmlLinkTagElement(false, "CompanyLinkName", element.InnerText, element.Attributes["href"].Value));

            foreach (IHtmlTagElement tag in htmlTags)
            {
                tag.Value = tag.Value.Trim();
            }

            return htmlTags;
        }

        public bool IsInList(List<StackPanel> spList, string menuName, out StackPanel panel)
        {
            foreach (StackPanel currentPanel in spList)
            {
                if (currentPanel.Name == "sp" + menuName)
                {
                    panel = currentPanel;
                    return true;
                }
            }
            panel = null;
            return false;
        }

        public string SaveChanges(List<IHtmlTagElement> htmlTags)
        {
            if (templatename == "Tea")
            {
                int counter = 0;
                // Getting company logo at the top
                HtmlNode element = document.DocumentNode.SelectSingleNode("//img[@class='logo']");
                element.SetAttributeValue("src", htmlTags[counter++].Value);

                // Getting 
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//nav//li//a"))
                {
                    HtmlLinkTagElement htmlNavLinkTag = (HtmlLinkTagElement)htmlTags[counter++];
                    node.InnerHtml = htmlNavLinkTag.Value;
                    node.SetAttributeValue("href", htmlNavLinkTag.Link);
                }

                // Getting source of the product main image
                element = document.DocumentNode.SelectSingleNode("//img[@class='product_image']");
                element.SetAttributeValue("src", htmlTags[counter++].Value);

                // Getting name of the product
                element = document.DocumentNode.SelectSingleNode("//h1[@class='productname']");
                element.InnerHtml = htmlTags[counter++].Value;

                // Getting arguments of the product
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//ul[@class='arguments']/li"))
                {
                    node.InnerHtml = htmlTags[counter++].Value;
                }

                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@class='info_item']"))
                {
                    element = node.SelectSingleNode("h2[@class='headline']");
                    element.InnerHtml = htmlTags[counter++].Value;

                    element = node.SelectSingleNode("p[@class='text']");
                    element.InnerHtml = htmlTags[counter++].Value;
                }

                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//footer//li//a"))
                {
                    HtmlLinkTagElement htmlFooterLinkTag = (HtmlLinkTagElement)htmlTags[counter++];
                    node.InnerHtml = htmlFooterLinkTag.Value;
                    node.SetAttributeValue("href", htmlFooterLinkTag.Link);
                }

                element = document.DocumentNode.SelectSingleNode("//p[@class='copyright']/a");
                HtmlLinkTagElement htmlCopyrightLinkTag = (HtmlLinkTagElement)htmlTags[counter];
                element.InnerHtml = htmlCopyrightLinkTag.Value;
                element.SetAttributeValue("href", htmlCopyrightLinkTag.Link);
            }
            return document.DocumentNode.OuterHtml;
        }
    }
}
