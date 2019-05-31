using System;
using System.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using eBayForm.LogicUnits.Exceptions;
using HtmlAgilityPack;
using eBayForm.LogicUnits;

namespace eBayForm
{
    public sealed class LogicController
    {
        // Html agility pack
        // Class which gonna contain HtmlCode
        HtmlDocument document = new HtmlDocument();
        private string templatename;

        public string Document { get => document.DocumentNode.InnerText == "" ? null : document.DocumentNode.OuterHtml; }
        public string Templatename { get => templatename; }

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

        public List<HtmlTagElement> GetTags()
        {
            List<HtmlTagElement> htmlTags = new List<HtmlTagElement>();
            
            // Getting name of the product
            HtmlNode element = document.DocumentNode.SelectSingleNode("//h1[@id='productname']");
            htmlTags.Add(new HtmlTagElement(false, "Productname", element.InnerText));

            // Getting 
            int i = 1;
            if (templatename == "Tea")
            {
                // Getting company logo at the top
                element = document.DocumentNode.SelectSingleNode("//header/img");
                htmlTags.Add(new HtmlTagElement(false, "Company logo", element.Attributes["src"].Value));

                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//nav//li//a"))
                {
                    htmlTags.Add(new HtmlTagElement(true, "Similar product " + i++, node.InnerText, node.Attributes["href"].Value));
                }

                // Getting source of the product main image
                element = document.DocumentNode.SelectSingleNode("//img[@id='product_image']");
                htmlTags.Add(new HtmlTagElement(false, "Product image", element.Attributes["src"].Value));

                // Getting arguments of the product
                i = 1;
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//ul[@id='arguments']/li"))
                {
                    htmlTags.Add(new HtmlTagElement(true, "Argument " + i++, node.InnerText));
                }

                i = 1;
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@id='info_items']/div"))
                {
                    element = node.SelectSingleNode("h2");
                    htmlTags.Add(new HtmlTagElement(true, "Headline " + i, element.InnerText));

                    element = node.SelectSingleNode("p");
                    htmlTags.Add(new HtmlTagElement(true, "Text " + i, element.InnerText));
                    i++;
                }
            }
            else if (templatename == "CoffeeBean")
            {
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//ul[@id='image_list']/li/img"))
                {
                    htmlTags.Add(new HtmlTagElement(true, "Gallary image " + i++, node.Attributes["src"].Value));
                }

                i = 1;
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//ul[@id='arguments']/li"))
                {
                    htmlTags.Add(new HtmlTagElement(true, "Argument " + i++, node.InnerText));
                }

                i = 1;
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@id='buttons']/button"))
                {
                    htmlTags.Add(new HtmlTagElement(true, "Button " + i++, node.InnerText));
                }

                i = 1;
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@id='info_tabs']/div"))
                {
                    htmlTags.Add(new HtmlTagElement(true, "Tab " + i++, node.InnerText));
                }

                i = 1;
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@id='similar_products']/a"))
                {
                    element = node.SelectSingleNode("img");
                    string imageLink = element.Attributes["src"].Value;
                    element = node.SelectSingleNode("div[@class='similar_product_name_wrapper']/p");
                    htmlTags.Add(new HtmlTagElement(true, "Similar product " + i++, element.InnerText, imageLink));
                }
            }

            element = document.DocumentNode.SelectSingleNode("//p[@class='copyright']/a");
            htmlTags.Add(new HtmlTagElement(false, "Companyname", element.InnerText, element.Attributes["href"].Value));

            return htmlTags;
        }
        
        public List<TagStyleElement> GetStyle()
        {
            List<TagStyleElement> styleElements = new List<TagStyleElement>();

            HtmlNode element = document.DocumentNode.SelectSingleNode("//h1[@id='productname']");
            string[] style = element.Attributes["style"].Value.Split(':');
            styleElements.Add(new TagStyleElement("Productname color", style[1].Replace(";", "")));

            element = document.DocumentNode.SelectSingleNode("//body");
            style = element.Attributes["style"].Value.Split(':');
            styleElements.Add(new TagStyleElement("Background of page", style[1].Replace(";", "")));

            if (templatename == "Tea")
            {
                element = document.DocumentNode.SelectSingleNode("//nav//li//a");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new TagStyleElement("Similar products color", style[1].Replace(";", "")));

                element = document.DocumentNode.SelectSingleNode("//ul[@id='arguments']/li");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new TagStyleElement("Arguments color", style[1].Replace(";", "")));

                element = document.DocumentNode.SelectSingleNode("//div[@id='info_items']/div/h2");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new TagStyleElement("Textblock headline color", style[1].Replace(";", "")));

                element = document.DocumentNode.SelectSingleNode("//div[@id='info_items']/div/p");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new TagStyleElement("Textblock text color", style[1].Replace(";", "")));
            }
            else if(templatename == "CoffeeBean")
            {
                element = document.DocumentNode.SelectSingleNode("//header");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new TagStyleElement("Parting line", style[1].Replace(";", "")));

                element = document.DocumentNode.SelectSingleNode("//div[@id='selected_image']");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new TagStyleElement("Selected image background", style[1].Replace(";", "")));

                element = document.DocumentNode.SelectSingleNode("//ul[@id='image_list']");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new TagStyleElement("Image gallery background", style[1].Replace(";", "")));

                element = document.DocumentNode.SelectSingleNode("//ul[@id='arguments']/li");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new TagStyleElement("Arguments color", style[1].Replace(";", "")));

                element = document.DocumentNode.SelectSingleNode("//div[@id='buttons']/button");
                string[] properties = element.Attributes["style"].Value.Split(';');
                style = properties[1].Split(':');
                styleElements.Add(new TagStyleElement("Button text color", style[1].Replace(";", "")));

                style = properties[2].Split(':');
                styleElements.Add(new TagStyleElement("Button color", style[1].Replace(";", "")));

                element = document.DocumentNode.SelectSingleNode("//div[@id='info_tabs']/div");
                properties = element.Attributes["style"].Value.Split(';');
                style = properties[0].Split(':');
                styleElements.Add(new TagStyleElement("Tab text color", style[1].Replace(";", "")));

                style = properties[1].Split(':');
                styleElements.Add(new TagStyleElement("Tab color", style[1].Replace(";", "")));

                element = document.DocumentNode.SelectSingleNode("//div[@id='similar_products']/a");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new TagStyleElement("Similar product image background", style[1].Replace(";", "")));

                element = document.DocumentNode.SelectSingleNode("//div[@id='similar_products']/a/div/p");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new TagStyleElement("Similar product name color", style[1].Replace(";", "")));
            }

            element = document.DocumentNode.SelectSingleNode("//footer");
            style = element.Attributes["style"].Value.Split(':');
            styleElements.Add(new TagStyleElement("Footer background color", style[1].Replace(";", "")));

            element = document.DocumentNode.SelectSingleNode("//footer/p[@class='copyright']");
            style = element.Attributes["style"].Value.Split(':');
            styleElements.Add(new TagStyleElement("Copyright color", style[1].Replace(";", "")));

            element = document.DocumentNode.SelectSingleNode("//footer/p[@class='copyright']/a");
            style = element.Attributes["style"].Value.Split(':');
            styleElements.Add(new TagStyleElement("Company color", style[1].Replace(";", "")));

            return styleElements;
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

        public string SaveChanges(List<HtmlTagElement> htmlTags, List<TagStyleElement> styleElements)
        {

            int tagsCounter = 0;
            int styleCounter = 0;

            // Getting name of the product
            HtmlNode element = document.DocumentNode.SelectSingleNode("//h1[@id='productname']");
            element.InnerHtml = htmlTags[tagsCounter++].Value;
            element.SetAttributeValue("style", "color: " + styleElements[styleCounter++].Value + ";");

            element = document.DocumentNode.SelectSingleNode("//body");
            element.SetAttributeValue("style", "background-color: " + styleElements[styleCounter++].Value + ";");

            if (templatename == "Tea")
            {
                // Getting company logo at the top
                element = document.DocumentNode.SelectSingleNode("//header/img");
                element.SetAttributeValue("src", htmlTags[tagsCounter++].Value);

                // Getting 
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//nav//li//a"))
                {
                    node.InnerHtml = htmlTags[tagsCounter].Value;
                    node.SetAttributeValue("href", htmlTags[tagsCounter++].Link);
                    node.SetAttributeValue("style", "color: " + styleElements[styleCounter].Value + ";");
                }
                styleCounter++;

                // Getting source of the product main image
                element = document.DocumentNode.SelectSingleNode("//img[@id='product_image']");
                element.SetAttributeValue("src", htmlTags[tagsCounter++].Value);

                // Getting arguments of the product
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//ul[@id='arguments']/li"))
                {
                    node.InnerHtml = htmlTags[tagsCounter++].Value;
                    node.SetAttributeValue("style", "color: " + styleElements[styleCounter].Value + ";");
                }
                styleCounter++;

                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@id='info_items']/div"))
                {
                    element = node.SelectSingleNode("h2");
                    element.InnerHtml = htmlTags[tagsCounter++].Value;
                    element.SetAttributeValue("style", "color: " + styleElements[styleCounter++].Value + ";");

                    element = node.SelectSingleNode("p");
                    element.InnerHtml = htmlTags[tagsCounter++].Value;
                    element.SetAttributeValue("style", "color: " + styleElements[styleCounter--].Value + ";");
                }
                styleCounter += 2;
            }
            else if (templatename == "CoffeeBean")
            {
                element = document.DocumentNode.SelectSingleNode("//header");
                element.SetAttributeValue("style", "border-color: " + styleElements[styleCounter++].Value + ";");

                element = document.DocumentNode.SelectSingleNode("//div[@id='selected_image']");
                element.SetAttributeValue("style", "background-color: " + styleElements[styleCounter++].Value + ";");

                element = document.DocumentNode.SelectSingleNode("//ul[@id='image_list']");
                element.SetAttributeValue("style", "background-color: " + styleElements[styleCounter++].Value + ";");

                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//ul[@id='image_list']/li"))
                {
                    element = node.SelectSingleNode("img");
                    element.SetAttributeValue("src", htmlTags[tagsCounter++].Value);
                }

                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//ul[@id='arguments']/li"))
                {
                    node.InnerHtml = htmlTags[tagsCounter++].Value;
                    node.SetAttributeValue("style", "color: " + styleElements[styleCounter].Value + ";");
                }
                styleCounter++;

                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@id='buttons']/button"))
                {
                    node.InnerHtml = htmlTags[tagsCounter++].Value;
                    string widthAttribute = node.GetAttributeValue("style", "").Split(';')[0];
                    node.SetAttributeValue("style", widthAttribute + "; " +
                                                    "color: " + styleElements[styleCounter++].Value + ";" +
                                                    "background-color:" + styleElements[styleCounter--].Value + ";");
                }
                styleCounter += 2;

                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@id='info_tabs']/div"))
                {
                    node.InnerHtml = htmlTags[tagsCounter++].Value;
                    node.SetAttributeValue("style", "color: " + styleElements[styleCounter++].Value + ";" +
                                                    "background-color:" + styleElements[styleCounter--].Value + ";");
                }
                styleCounter += 2;

                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@id='similar_products']/a"))
                {
                    node.SetAttributeValue("style", "background-color: " + styleElements[styleCounter++].Value + ";");
                    element = node.SelectSingleNode("img");
                    element.SetAttributeValue("src", htmlTags[tagsCounter].Link);
                    element = node.SelectSingleNode("div[@class='similar_product_name_wrapper']/p");
                    element.SetAttributeValue("style", "color: " + styleElements[styleCounter--].Value + ";");
                    element.InnerHtml = htmlTags[tagsCounter++].Value;
                }
                styleCounter += 2;
            }

            element = document.DocumentNode.SelectSingleNode("//footer");
            element.SetAttributeValue("style", "background-color: " + styleElements[styleCounter++].Value + ";");

            element = document.DocumentNode.SelectSingleNode("//footer/p[@class='copyright']");
            element.SetAttributeValue("style", "color: " + styleElements[styleCounter++].Value + ";");

            element = document.DocumentNode.SelectSingleNode("//footer/p[@class='copyright']/a");
            element.SetAttributeValue("style", "color: " + styleElements[styleCounter].Value + ";");
            element.InnerHtml = htmlTags[tagsCounter].Value;
            element.SetAttributeValue("href", htmlTags[tagsCounter].Link);

            return document.DocumentNode.OuterHtml;
        }

        public string LoadTemplate(string templatename, Dictionary<string, string> configurationKeyValues)
        {
            this.templatename = templatename;
            string htmlCode;

            using (StreamReader reader = new StreamReader("../../HtmlTemplates/" + templatename + ".html"))
            {
                htmlCode = reader.ReadToEnd();
            }

            document.LoadHtml(htmlCode);

            HtmlNode element = document.DocumentNode.SelectSingleNode("//header/img");
            element.SetAttributeValue("src", configurationKeyValues["LogoLink"]);
            
            element = document.DocumentNode.SelectSingleNode("//img[@id='product_image']");
            element.SetAttributeValue("src", configurationKeyValues["ProductImageLink"]);

            element = document.DocumentNode.SelectSingleNode("//h1[@id='productname']");
            element.InnerHtml = configurationKeyValues["ProductName"];

            element = document.DocumentNode.SelectSingleNode("//p[@class='copyright']/a");
            element.InnerHtml = configurationKeyValues["CompanyName"];
            element.SetAttributeValue("href", configurationKeyValues["CompanyLink"]);

            if (templatename == "Tea")
            {
                HtmlNode parentElement = document.DocumentNode.SelectSingleNode("//nav/ul");

                int count = Convert.ToInt16(configurationKeyValues["NavLinkCount"]);

                for (int i = 0; i < count; i++)
                {
                    element = HtmlNode.CreateNode("<li><a href='' style='color: #D4AA00;'>Navigation item</a></li>");
                    parentElement.AppendChild(element);
                }

                parentElement = document.DocumentNode.SelectSingleNode("//ul[@id='arguments']");

                count = Convert.ToInt16(configurationKeyValues["ArgumentsCount"]);

                for (int i = 0; i < count; i++)
                {
                    element = HtmlNode.CreateNode("<li style='color: #4e4e4e;'>Argument</li>");
                    parentElement.AppendChild(element);
                }

                parentElement = document.DocumentNode.SelectSingleNode("//div[@id='info_items_row_1']/div[@class='wrapper']/div[@id='info_items']");
                HtmlNode parentElementSecond = document.DocumentNode.SelectSingleNode("//div[@id='info_items_row_2']/div[@class='wrapper']/div[@id='info_items']");

                count = Convert.ToInt16(configurationKeyValues["TexboxCount"]);

                for (int i = 0; i < count; i++)
                {
                    element = HtmlNode.CreateNode("<div><h2 style='color: #D4AA00;'>Headline</h2>" +
                                                  "<p style='color: #2e2e2e;'>" +
                                                  "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, " +
                                                  "sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, " +
                                                  "sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. " +
                                                  "Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet." +
                                                  "</p></div>");
                    if (i % 2 == 0)
                    {
                        parentElement.AppendChild(element);
                    }
                    else
                    {
                        parentElementSecond.AppendChild(element);
                    }
                }
            }
            else if (templatename == "CoffeeBean")
            {
                HtmlNode parentElement = document.DocumentNode.SelectSingleNode("//ul[@id='image_list']");

                int count = Convert.ToInt16(configurationKeyValues["GallaryImageCount"]);
                count--;

                for (int i = 0; i < count; i++)
                {
                    element = HtmlNode.CreateNode("<li><img src='https://cdn.pixabay.com/photo/2016/02/19/11/35/make-up-1209798_960_720.jpg'></li>");
                    parentElement.AppendChild(element);
                }

                parentElement = document.DocumentNode.SelectSingleNode("//ul[@id='arguments']");

                count = Convert.ToInt16(configurationKeyValues["ArgumentsCount"]);

                for (int i = 0; i < count; i++)
                {
                    element = HtmlNode.CreateNode("<li style='color: #3b3b3b;'>Argument</li>");
                    parentElement.AppendChild(element);
                }

                parentElement = document.DocumentNode.SelectSingleNode("//div[@id='info_tabs']");
                HtmlNode buttonParentElement = document.DocumentNode.SelectSingleNode("//div[@id='buttons']");

                count = Convert.ToInt16(configurationKeyValues["TexboxCount"]);

                for (int i = 0; i < count; i++)
                {
                    element = HtmlNode.CreateNode("<button style='width: " + 100 / count + "%; color: #ffffff; background-color: #D4AA00;'>Title</button>");
                    buttonParentElement.AppendChild(element);

                    element = HtmlNode.CreateNode("<div style='color: #3b3b3b; background-color: rgba(255,255,246,0.02); '> " +
                                                      "quis voluptatum obcaecati deserunt unde odit expedita, ratione numquam " +
                                                      "sunt quidem adipisci animi saepe impedit nam repellendus? Ipsum, modi. " +
                                                      "Lorem ipsum, dolor sit amet consectetur adipisicing elit. " +
                                                      "Blanditiis rerum possimus cum, sapiente quibusdam, " +
                                                      "optio ab provident earum animi ipsum ipsa reprehenderit hic! " +
                                                      "Numquam, doloribus recusandae cumque quibusdam aliquam natus?" +
                                                  "</div>");
                    parentElement.AppendChild(element);
                }

                parentElement = document.DocumentNode.SelectSingleNode("//div[@id='similar_products']");

                count = Convert.ToInt16(configurationKeyValues["NavLinkCount"]);

                for (int i = 0; i < count; i++)
                {
                    element = HtmlNode.CreateNode("<a href='' style='background-color: rgba(255,255,246,0.02);'>" +
                                                    "<img src='https://cdn.pixabay.com/photo/2016/12/06/18/27/milk-1887237_960_720.jpg'>" +
                                                    "<div class='similar_product_name_wrapper'>" +
                                                        "<p style='color: #ffffff;'>" +
                                                            "Productname" +
                                                        "</p>" +
                                                    "</div>" +
                                                  "</a>");
                    parentElement.AppendChild(element);
                }
            }

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
