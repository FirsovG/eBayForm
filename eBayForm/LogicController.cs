using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using eBayForm.LogicUnits.Exceptions;
using eBayForm.LogicUnits.HtmlTags;
using HtmlAgilityPack;

namespace eBayForm
{
    public sealed class LogicController
    {
        // Html agility pack
        // Class which gonna contain HtmlCode
        HtmlDocument document = new HtmlDocument();
        string templatename;

        public string Document { get => document.DocumentNode.InnerText == "" ? null : document.DocumentNode.OuterHtml; }

        // TODO: Check if the IE meta-tag at the beginn of the file
        public async Task<string> ImportHtmlAsync(string path)
        {
            // string which contain htmlCode
            string htmlCode;
            // Read the file
            using (StreamReader reader = new StreamReader(path))
            {
                // Insert content into htmlCode
                htmlCode = await reader.ReadToEndAsync();
            }
            // Compress htmlHode
            htmlCode = Regex.Replace(htmlCode, @"\r\n?|\n", "");
            // Loading the Code
            document.LoadHtml(htmlCode);

            // To diplay the content rightly we need to check compatibility to InternetExplorer
            if (document.DocumentNode.SelectSingleNode("//meta/@http-equiv") == null)
            {
                // Select the head-tag
                HtmlNode head = document.DocumentNode.SelectSingleNode("//head");
                // Creating meta-tag
                HtmlNode meta = document.CreateElement("meta");
                // Adding meta-tag at the beginn of head-tag
                head.PrependChild(meta);
                // Adding IE Attributes
                meta.SetAttributeValue("http-equiv", "X-UA-Compatible");
                meta.SetAttributeValue("content", "IE=11");
            }

            // Searching for Template meta tag
            HtmlNode templateTag = document.DocumentNode.SelectSingleNode("//body");
            if (templateTag.Attributes["id"] == null)
            {
                throw new TemplateException("Not a Template");
            }
            else
            {
                foreach(string configTemplatename in ConfigurationManager.AppSettings["Templates"].Split(','))
                {
                    if (configTemplatename == templateTag.Attributes["id"].Value)
                    {
                        templatename = templateTag.Attributes["id"].Value;
                        // Returning the HtmlCode as string
                        return document.DocumentNode.OuterHtml;
                    }
                }
            }
            throw new TemplateException("Unknown Template");
        }


        public List<IHtmlTagElement> GetTags()
        {
            List<IHtmlTagElement> htmlTags = new List<IHtmlTagElement>();
            
            // Getting name of the product
            HtmlNode element = document.DocumentNode.SelectSingleNode("//h1[@id='productname']");
            htmlTags.Add(new HtmlTagElement(false, "ProductName", element.InnerText));

            // Getting 
            int i = 1;
            if (templatename == "Tea")
            {

                // Getting company logo at the top
                element = document.DocumentNode.SelectSingleNode("//header/img");
                htmlTags.Add(new HtmlTagElement(false, "TopCompanyLogo", element.Attributes["src"].Value));

                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//nav//li//a"))
                {
                    htmlTags.Add(new HtmlLinkTagElement(true, "SimilarProductsLink" + i++, node.InnerText, node.Attributes["href"].Value));
                }

                // Getting source of the product main image
                element = document.DocumentNode.SelectSingleNode("//img[@id='product_image']");
                htmlTags.Add(new HtmlTagElement(false, "ProductMainImage", element.Attributes["src"].Value));

                // Getting arguments of the product
                i = 1;
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//ul[@id='arguments']/li"))
                {
                    htmlTags.Add(new HtmlTagElement(true, "Argument" + i++, node.InnerText));
                }

                i = 1;
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@id='info_items']/div"))
                {
                    element = node.SelectSingleNode("h2");
                    htmlTags.Add(new HtmlTagElement(true, "Headline" + i, element.InnerText));

                    element = node.SelectSingleNode("p");
                    htmlTags.Add(new HtmlTagElement(true, "Text" + i, element.InnerText));
                    i++;
                }
            }
            else if (templatename == "CoffeeBean")
            {
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//ul[@id='image-list']/li/img"))
                {
                    htmlTags.Add(new HtmlTagElement(true, "GallaryImage" + i++, node.Attributes["src"].Value));
                }

                i = 1;
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//ul[@id='arguments']/li"))
                {
                    htmlTags.Add(new HtmlTagElement(true, "Argument" + i++, node.InnerText));
                }

                i = 1;
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@id='buttons']/button"))
                {
                    htmlTags.Add(new HtmlTagElement(true, "Button" + i++, node.InnerText));
                }

                i = 1;
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@id='info_tabs']/div"))
                {
                    htmlTags.Add(new HtmlTagElement(true, "Tab" + i++, node.InnerText));
                }

                i = 1;
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@id='similar_products']/a"))
                {
                    element = node.SelectSingleNode("img");
                    string imageLink = element.Attributes["src"].Value;
                    element = node.SelectSingleNode("div[@class='similar_product_name_wrapper']/p");
                    htmlTags.Add(new HtmlLinkTagElement(true, "SimilarProductLink" + i++, element.InnerText, imageLink));
                }
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

            int counter = 0;

            // Getting name of the product
            HtmlNode element = document.DocumentNode.SelectSingleNode("//h1[@id='productname']");
            element.InnerHtml = htmlTags[counter++].Value;

            if (templatename == "Tea")
            {
                // Getting company logo at the top
                element = document.DocumentNode.SelectSingleNode("//header/img");
                element.SetAttributeValue("src", htmlTags[counter++].Value);

                // Getting 
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//nav//li//a"))
                {
                    HtmlLinkTagElement htmlNavLinkTag = (HtmlLinkTagElement)htmlTags[counter++];
                    node.InnerHtml = htmlNavLinkTag.Value;
                    node.SetAttributeValue("href", htmlNavLinkTag.Link);
                }

                // Getting source of the product main image
                element = document.DocumentNode.SelectSingleNode("//img[@id='product_image']");
                element.SetAttributeValue("src", htmlTags[counter++].Value);

                // Getting arguments of the product
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//ul[@id='arguments']/li"))
                {
                    node.InnerHtml = htmlTags[counter++].Value;
                }

                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@id='info_items']/div"))
                {
                    element = node.SelectSingleNode("h2");
                    element.InnerHtml = htmlTags[counter++].Value;

                    element = node.SelectSingleNode("p");
                    element.InnerHtml = htmlTags[counter++].Value;
                }
            }
            else if (templatename == "CoffeeBean")
            {
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//ul[@id='image-list']/li/img"))
                {
                    node.SetAttributeValue("src", htmlTags[counter++].Value);
                }
                
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//ul[@id='arguments']/li"))
                {
                    node.InnerHtml = htmlTags[counter++].Value;
                }
                
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@id='buttons']/button"))
                {
                    node.InnerHtml = htmlTags[counter++].Value;
                }

                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@id='info_tabs']/div"))
                {
                    node.InnerHtml = htmlTags[counter++].Value;
                }

                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@id='similar_products']/a"))
                {
                    HtmlLinkTagElement htmlSimilarLinkTag = (HtmlLinkTagElement)htmlTags[counter++];
                    element = node.SelectSingleNode("img");
                    element.SetAttributeValue("src", htmlSimilarLinkTag.Link);
                    element = node.SelectSingleNode("div[@class='similar_product_name_wrapper']/p");
                    element.InnerHtml = htmlSimilarLinkTag.Value;
                }
            }

            element = document.DocumentNode.SelectSingleNode("//p[@class='copyright']/a");
            HtmlLinkTagElement htmlCopyrightLinkTag = (HtmlLinkTagElement)htmlTags[counter];
            element.InnerHtml = htmlCopyrightLinkTag.Value;
            element.SetAttributeValue("href", htmlCopyrightLinkTag.Link);

            return document.DocumentNode.OuterHtml;
        }

        public string LoadTemplate(string templateName, Dictionary<string, string> configurationKeyValues)
        {
            templatename = templateName;
            string htmlCode;

            using (StreamReader reader = new StreamReader("../../HtmlTemplates/" + templateName + ".html"))
            {
                htmlCode = reader.ReadToEnd();
            }

            document.LoadHtml(htmlCode);

            HtmlNode element = document.DocumentNode.SelectSingleNode("//header/img");
            element.SetAttributeValue("src", configurationKeyValues["LogoLink"]);

            HtmlNode parentElement = document.DocumentNode.SelectSingleNode("//nav/ul");

            int count = Convert.ToInt16(configurationKeyValues["NavLinkCount"]);

            for (int i = 0; i < count; i++)
            {
                element = HtmlNode.CreateNode("<li><a href=''>Navigation Item</a></li>");
                parentElement.AppendChild(element);
            }

            element = document.DocumentNode.SelectSingleNode("//img[@id='product_image']");
            element.SetAttributeValue("src", configurationKeyValues["ProductImageLink"]);

            element = document.DocumentNode.SelectSingleNode("//h1[@id='productname']");
            element.InnerHtml = configurationKeyValues["ProductName"];

            parentElement = document.DocumentNode.SelectSingleNode("//ul[@id='arguments']");

            count = Convert.ToInt16(configurationKeyValues["ArgumentsCount"]);

            for (int i = 0; i < count; i++)
            {
                element = HtmlNode.CreateNode("<li>Argument</li>");
                parentElement.AppendChild(element);
            }

            parentElement = document.DocumentNode.SelectSingleNode("//div[@id='info_items_row_1']/div[@class='wrapper']/div[@id='info_items']");
            HtmlNode parentElementSecond = document.DocumentNode.SelectSingleNode("//div[@id='info_items_row_2']/div[@class='wrapper']/div[@id='info_items']");

            count = Convert.ToInt16(configurationKeyValues["TexboxCount"]);

            for (int i = 0; i < count; i++)
            {
                element = HtmlNode.CreateNode("<div><h2>Headline</h2>" +
                                              "<p>Text</p></div>");
                if (i % 2 == 0)
                {
                    parentElement.AppendChild(element);
                }
                else
                {
                    parentElementSecond.AppendChild(element);
                }
            }

            element = document.DocumentNode.SelectSingleNode("//p[@class='copyright']/a");
            element.InnerHtml = configurationKeyValues["CompanyName"];
            element.SetAttributeValue("href", configurationKeyValues["CompanyLink"]);

            return document.DocumentNode.OuterHtml;
        }

        public void Export(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine(Document);
            }
        }
    }
}
