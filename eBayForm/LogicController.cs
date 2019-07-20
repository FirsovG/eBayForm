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
        private string[] templates;

        public string Document { get => document.DocumentNode.InnerText == "" ? null : document.DocumentNode.OuterHtml; }
        public string Templatename { get => templatename; }
        public string[] Templates { get => templates; set => templates = value; }

        public LogicController()
        {
            if (Directory.Exists("HtmlTemplates"))
            {
                templates = Directory.GetFiles("HtmlTemplates", "*.html");
                for (int i = 0; i < templates.Length; i++)
                {
                    templates[i] = templates[i].Split('\\')[1].Split('.')[0];
                }
            }
        }

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

            element = document.DocumentNode.SelectSingleNode("//p[@id='price']");
            htmlTags.Add(new HtmlTagElement(false, "Productprice", element.InnerText));

            element = document.DocumentNode.SelectSingleNode("//div[@id='short_description']/span");
            htmlTags.Add(new HtmlTagElement(false, "Mobile: short description", element.InnerText));

            // Getting 
            int i = 1;
            if (templatename == "Tea")
            {
                // Getting company logo at the top
                element = document.DocumentNode.SelectSingleNode("//header/div/img");
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
                    htmlTags.Add(new HtmlTagElement(true, "Bulletpoint " + i++, node.InnerText));
                }

                i = 1;
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@id='info_items']/div"))
                {
                    element = node.SelectSingleNode("h2");
                    string headline = element.InnerText;

                    element = node.SelectSingleNode("p");
                    htmlTags.Add(new HtmlTagElement(true, "Textblock " + i, headline, element.InnerText));
                    i++;
                }
            }
            else if (templatename == "CoffeeBean")
            {
                // Getting company logo at the top
                element = document.DocumentNode.SelectSingleNode("//header/div/img");
                htmlTags.Add(new HtmlTagElement(false, "Company logo", element.Attributes["src"].Value));

                int galleryImageCounter = 0;
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@class='gallery']/input"))
                {
                    galleryImageCounter++;
                }

                element = document.DocumentNode.SelectSingleNode("//style[@id='gallery_pictures']");

                i = 1;
                for (int j = 0; j < galleryImageCounter; j++)
                {
                    htmlTags.Add(new HtmlTagElement(true, "Gallary image " + i++, element.InnerHtml.Split('{')[j + 1].Split('}')[0].Split(';')[0].Split('\'')[1]));
                }

                i = 1;
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//ul[@id='arguments']/li"))
                {
                    htmlTags.Add(new HtmlTagElement(true, "Bulletpoint " + i++, node.InnerText));
                }

                i = 1;
                List<string> tmpList = new List<string>();
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@id='description_items']/label"))
                {
                    tmpList.Add(node.InnerText);
                }

                i = 1;
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//p[@id='tab_p']"))
                {
                    htmlTags.Add(new HtmlTagElement(true, "Textblock " + i, tmpList[i++ - 1], node.InnerText));
                }

                i = 1;
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@id='similar_products']/a"))
                {
                    string link = node.Attributes["href"].Value;
                    element = node.SelectSingleNode("img");
                    string imageLink = element.Attributes["src"].Value;
                    element = node.SelectSingleNode("p");
                    htmlTags.Add(new HtmlTagElement(true, "Similar product " + i++, element.InnerText, link, imageLink));
                }
            }

            element = document.DocumentNode.SelectSingleNode("//p[@class='copyright']/a");
            htmlTags.Add(new HtmlTagElement(false, "Companyname", element.InnerText, element.Attributes["href"].Value));

            return htmlTags;
        }
        
        public List<StyleTagElement> GetStyle()
        {
            List<StyleTagElement> styleElements = new List<StyleTagElement>();

            HtmlNode element = document.DocumentNode.SelectSingleNode("//h1[@id='productname']");
            string[] style = element.Attributes["style"].Value.Split(':');
            styleElements.Add(new StyleTagElement("Producttitle color", style[1].Replace(";", "")));

            element = document.DocumentNode.SelectSingleNode("//p[@id='price']");
            style = element.Attributes["style"].Value.Split(':');
            styleElements.Add(new StyleTagElement("Productprice color", style[1].Replace(";", "")));

            if (templatename == "Tea")
            {
                element = document.DocumentNode.SelectSingleNode("//div[@class='header']");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new StyleTagElement("Background of header", style[1].Replace(";", "")));

                element = document.DocumentNode.SelectSingleNode("//nav//li//a");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new StyleTagElement("Similar products color", style[1].Replace(";", "")));

                element = document.DocumentNode.SelectSingleNode("//style[@id='similar_products_style']");
                styleElements.Add(new StyleTagElement("Similar products border color", "#" + element.InnerHtml.Split('{')[1].Split('#')[1].Split(';')[0]));

                element = document.DocumentNode.SelectSingleNode("//div[@class='product_arguments']");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new StyleTagElement("Background of productimage and bulletpoints", style[1].Replace(";", "")));

                element = document.DocumentNode.SelectSingleNode("//ul[@id='arguments']/li");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new StyleTagElement("Bulletpoints color", style[1].Replace(";", "")));

                element = element.SelectSingleNode("i");
                styleElements.Add(new StyleTagElement("Bulletpoints symbol", element.GetAttributeValue("class", "")));

                element = document.DocumentNode.SelectSingleNode("//div[@class='description']");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new StyleTagElement("Background of description", style[1].Replace(";", "")));

                element = document.DocumentNode.SelectSingleNode("//div[@id='info_items']/div/h2");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new StyleTagElement("Textblock headline color", style[1].Replace(";", "")));

                element = document.DocumentNode.SelectSingleNode("//div[@id='info_items']/div/p");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new StyleTagElement("Textblock text color", style[1].Replace(";", "")));
            }
            else if(templatename == "CoffeeBean")
            {
                element = document.DocumentNode.SelectSingleNode("//div[@class='header']");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new StyleTagElement("Background of header", style[1].Replace(";", "")));

                element = document.DocumentNode.SelectSingleNode("//header");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new StyleTagElement("Parting line", style[1].Replace(";", "")));

                element = document.DocumentNode.SelectSingleNode("//div[@class='product_arguments']");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new StyleTagElement("Background of productgallery and bulletpoints", style[1].Replace(";", "")));

                //element = document.DocumentNode.SelectSingleNode("//ul[@id='image_list']");
                //style = element.Attributes["style"].Value.Split(':');
                //styleElements.Add(new StyleTagElement("Image gallery background", style[1].Replace(";", "")));

                element = document.DocumentNode.SelectSingleNode("//ul[@id='arguments']/li");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new StyleTagElement("Bulletpoints color", style[1].Replace(";", "")));

                element = element.SelectSingleNode("i");
                styleElements.Add(new StyleTagElement("Bulletpoints symbol", element.GetAttributeValue("class", "")));

                element = document.DocumentNode.SelectSingleNode("//h3[@id='mobile_header']");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new StyleTagElement("Mobile: Color of container titles", style[1].Replace(";", "")));

                element = document.DocumentNode.SelectSingleNode("//div[@class='description']");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new StyleTagElement("Background of description", style[1].Replace(";", "")));

                //--Tab menu

                element = document.DocumentNode.SelectSingleNode("//style[@id='tab_item_style']");
                //string[] properties = element.Attributes["style"].Value.Split(';');
                //style = properties[1].Split(':');
                styleElements.Add(new StyleTagElement("Tab button text color", element.InnerHtml.Split('{')[1].Split('}')[0].Split(';')[0].Split(':')[1]));

                //style = properties[2].Split(':');
                styleElements.Add(new StyleTagElement("Tab button color", element.InnerHtml.Split('{')[1].Split('}')[0].Split(';')[1].Split(':')[1]));

                styleElements.Add(new StyleTagElement("Tab background color", element.InnerHtml.Split('{')[2].Split('}')[0].Split(';')[0].Split(':')[1]));

                element = document.DocumentNode.SelectSingleNode("//div[@id='info_tabs']/h4[@id='mobile_title']");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new StyleTagElement("Mobile: Title color", style[1].Replace(";", "")));

                element = document.DocumentNode.SelectSingleNode("//div[@id='info_tabs']/p[@id='tab_p']");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new StyleTagElement("Tab text color", style[1].Replace(";", "")));
                

                element = document.DocumentNode.SelectSingleNode("//div[@class='similar']");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new StyleTagElement("Background of similarproductsgallery", style[1].Replace(";", "")));

                element = document.DocumentNode.SelectSingleNode("//div[@id='similar_products']/a");

                string[] properties = element.Attributes["style"].Value.Split(';');
                style = properties[0].Split(':');
                styleElements.Add(new StyleTagElement("Similar product container background", style[1]));

                style = properties[1].Split(':');
                styleElements.Add(new StyleTagElement("Similar product border color", "#" + style[1].Split('#')[1]));

                element = document.DocumentNode.SelectSingleNode("//div[@id='similar_products']/a/p");
                style = element.Attributes["style"].Value.Split(':');
                styleElements.Add(new StyleTagElement("Similar product name color", style[1].Replace(";", "")));
            }

            element = document.DocumentNode.SelectSingleNode("//footer");
            style = element.Attributes["style"].Value.Split(':');
            styleElements.Add(new StyleTagElement("Footer background color", style[1].Replace(";", "")));

            element = document.DocumentNode.SelectSingleNode("//footer/p[@class='copyright']");
            style = element.Attributes["style"].Value.Split(':');
            styleElements.Add(new StyleTagElement("Copyright color", style[1].Replace(";", "")));

            element = document.DocumentNode.SelectSingleNode("//footer/p[@class='copyright']/a");
            style = element.Attributes["style"].Value.Split(':');
            styleElements.Add(new StyleTagElement("Company color", style[1].Replace(";", "")));

            return styleElements;
        }

        public string SaveChanges(List<HtmlTagElement> htmlTags, List<StyleTagElement> styleElements)
        {
            int tagsCounter = 0;
            // 0 For the Argument ComboBox
            int styleCounter = 1;

            // Getting name of the product
            HtmlNode element = document.DocumentNode.SelectSingleNode("//h1[@id='productname']");
            element.InnerHtml = htmlTags[tagsCounter++].Values[0];
            element.SetAttributeValue("style", "color: " + styleElements[styleCounter++].Value + ";");

            element = document.DocumentNode.SelectSingleNode("//p[@id='price']");
            element.InnerHtml = htmlTags[tagsCounter++].Values[0];
            element.SetAttributeValue("style", "color: " + styleElements[styleCounter++].Value + ";");

            element = document.DocumentNode.SelectSingleNode("//div[@id='short_description']/span");
            element.InnerHtml = htmlTags[tagsCounter++].Values[0];

            if (templatename == "Tea")
            {
                element = document.DocumentNode.SelectSingleNode("//div[@class='header']");
                element.SetAttributeValue("style", "background-color: " + styleElements[styleCounter++].Value + ";");
                // Getting company logo at the top
                element = document.DocumentNode.SelectSingleNode("//header/div/img");
                element.SetAttributeValue("src", htmlTags[tagsCounter++].Values[0]);

                // Getting 
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//nav//li//a"))
                {
                    node.InnerHtml = htmlTags[tagsCounter].Values[0];
                    node.SetAttributeValue("href", htmlTags[tagsCounter++].Values[1]);
                    node.SetAttributeValue("style", "color: " + styleElements[styleCounter].Value + ";");
                }
                styleCounter++;

                element = document.DocumentNode.SelectSingleNode("//style[@id='similar_products_style']");
                element.InnerHtml = "nav ul li a:hover {border-top: 1px solid " + styleElements[styleCounter].Value + ";" +
                                    "border-bottom: 1px solid " + styleElements[styleCounter++].Value + ";}";

                // Getting source of the product main image
                element = document.DocumentNode.SelectSingleNode("//img[@id='product_image']");
                element.SetAttributeValue("src", htmlTags[tagsCounter++].Values[0]);

                element = document.DocumentNode.SelectSingleNode("//div[@class='product_arguments']");
                element.SetAttributeValue("style", "background-color: " + styleElements[styleCounter++].Value + ";");

                // Getting arguments of the product
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//ul[@id='arguments']/li"))
                {
                    node.InnerHtml = "<i class='" + styleElements[0].Value + "'></i>" + htmlTags[tagsCounter++].Values[0];
                    node.SetAttributeValue("style", "color: " + styleElements[styleCounter].Value + ";");
                }
                styleCounter++;

                element = document.DocumentNode.SelectSingleNode("//div[@class='description']");
                element.SetAttributeValue("style", "background-color: " + styleElements[styleCounter++].Value + ";");

                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@id='info_item']"))
                {
                    element = node.SelectSingleNode("h2");
                    element.InnerHtml = htmlTags[tagsCounter].Values[0];
                    element.SetAttributeValue("style", "color: " + styleElements[styleCounter].Value + ";");

                    element = node.SelectSingleNode("p");
                    element.InnerHtml = htmlTags[tagsCounter++].Values[1];
                    element.SetAttributeValue("style", "color: " + styleElements[styleCounter + 1].Value + ";");
                }
                styleCounter += 2;
            }
            else if (templatename == "CoffeeBean")
            {
                element = document.DocumentNode.SelectSingleNode("//div[@class='header']");
                element.SetAttributeValue("style", "background-color: " + styleElements[styleCounter++].Value + ";");

                element = document.DocumentNode.SelectSingleNode("//header");
                element.SetAttributeValue("style", "border-color: " + styleElements[styleCounter++].Value + ";");
                
                element = document.DocumentNode.SelectSingleNode("//header/div/img");
                element.SetAttributeValue("src", htmlTags[tagsCounter++].Values[0]);

                element = document.DocumentNode.SelectSingleNode("//div[@class='product_arguments']");
                element.SetAttributeValue("style", "background-color: " + styleElements[styleCounter++].Value + ";");
                
                int galleryImageCounter = 0;
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@class='gallery']/input"))
                {
                    galleryImageCounter++;
                }

                element = document.DocumentNode.SelectSingleNode("//style[@id='gallery_pictures']");

                string gallery_images = "";
                string gallery_tech = "";

                for (int j = 0; j < galleryImageCounter; j++)
                {
                    gallery_images += ".gallery #image_" + (j + 1) + " {background-image: url('" + htmlTags[tagsCounter].Values[0] + "');}";
                    gallery_tech += "#image_input_" + (j + 1) + ":checked~#selected_image {background-image: url('" + htmlTags[tagsCounter++].Values[0] + "');}";
                }

                element = document.DocumentNode.SelectSingleNode("//style[@id='gallery_pictures']");
                element.InnerHtml = gallery_images;

                element = document.DocumentNode.SelectSingleNode("//style[@id='gallery_tech']");
                element.InnerHtml = gallery_tech;


                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//ul[@id='arguments']/li"))
                {
                    node.InnerHtml = "<i class='" + styleElements[0].Value + "'></i>" + htmlTags[tagsCounter++].Values[0];
                    node.SetAttributeValue("style", "color: " + styleElements[styleCounter].Value + ";");
                }
                styleCounter++;

                //-- Tab menu
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//h3[@id='mobile_header']"))
                {
                    node.SetAttributeValue("style", "color: " + styleElements[styleCounter].Value + ";");
                }
                styleCounter++;

                element = document.DocumentNode.SelectSingleNode("//div[@class='description']");
                element.SetAttributeValue("style", "background-color: " + styleElements[styleCounter++].Value + ";");

                int i = tagsCounter;
                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//style[@id='tab_item_style']"))
                {
                    node.InnerHtml = "#description_items label {color: " + styleElements[styleCounter].Value + "; background-color: " + styleElements[styleCounter + 1].Value + ";}" +
                                     "#description_items {background-color:" + styleElements[styleCounter + 2].Value + ";}" +
                                     "#description_items [id^='tab_button']:checked + label {color: " + styleElements[styleCounter + 1].Value + "; background-color: " + styleElements[styleCounter].Value + ";}";
                }
                styleCounter += 3;

                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@id='description_items']/label"))
                {
                    node.InnerHtml = htmlTags[tagsCounter++].Values[0];
                }

                tagsCounter = i;

                foreach (HtmlNode node in element.SelectNodes("//div[@id='info_tabs']/h4[@id='mobile_title']"))
                {
                    node.InnerHtml = htmlTags[tagsCounter++].Values[0];
                    node.SetAttributeValue("style", "color: " + styleElements[styleCounter].Value + ";");
                }

                foreach (HtmlNode node in element.SelectNodes("//div[@id='info_tabs']/p[@id='tab_p']"))
                {
                    node.InnerHtml = htmlTags[i++].Values[1];
                    node.SetAttributeValue("style", "color: " + styleElements[styleCounter + 1].Value + ";");
                }

                styleCounter += 2;

                element = document.DocumentNode.SelectSingleNode("//div[@class='similar']");
                element.SetAttributeValue("style", "background-color: " + styleElements[styleCounter++].Value + ";");

                foreach (HtmlNode node in document.DocumentNode.SelectNodes("//div[@id='similar_products']/a"))
                {
                    node.SetAttributeValue("style", "background-color: " + styleElements[styleCounter].Value + ";border: 1px solid " + styleElements[styleCounter + 1].Value + ";");
                    node.SetAttributeValue("href", htmlTags[tagsCounter].Values[1]);
                    element = node.SelectSingleNode("img");
                    element.SetAttributeValue("src", htmlTags[tagsCounter].Values[2]);
                    element = node.SelectSingleNode("p");
                    element.SetAttributeValue("style", "color: " + styleElements[styleCounter + 2].Value + ";");
                    element.InnerHtml = htmlTags[tagsCounter++].Values[0];
                }
                styleCounter += 3;
            }

            element = document.DocumentNode.SelectSingleNode("//footer");
            element.SetAttributeValue("style", "background-color: " + styleElements[styleCounter++].Value + ";");

            element = document.DocumentNode.SelectSingleNode("//footer/p[@class='copyright']");
            element.SetAttributeValue("style", "color: " + styleElements[styleCounter++].Value + ";");

            element = document.DocumentNode.SelectSingleNode("//footer/p[@class='copyright']/a");
            element.SetAttributeValue("style", "color: " + styleElements[styleCounter].Value + ";");
            element.InnerHtml = htmlTags[tagsCounter].Values[0];
            element.SetAttributeValue("href", htmlTags[tagsCounter].Values[1]);

            return document.DocumentNode.OuterHtml;
        }

        public string LoadTemplate(string templatename, Dictionary<string, string> configurationKeyValues)
        {
            this.templatename = templatename;
            string htmlCode;

            using (StreamReader reader = new StreamReader("HtmlTemplates/" + templatename + ".html"))
            {
                htmlCode = reader.ReadToEnd();
            }

            document.LoadHtml(htmlCode);

            HtmlNode element = document.DocumentNode.SelectSingleNode("//h1[@id='productname']");
            element.InnerHtml = configurationKeyValues["ProductName"];

            element = document.DocumentNode.SelectSingleNode("//p[@class='copyright']/a");
            element.InnerHtml = configurationKeyValues["CompanyName"];
            element.SetAttributeValue("href", configurationKeyValues["CompanyLink"] == "" ? "#" : configurationKeyValues["CompanyLink"]);

            element = document.DocumentNode.SelectSingleNode("//p[@id='price']");
            element.InnerHtml = configurationKeyValues["ProductPrice"];

            if (templatename == "Tea")
            {
                element = document.DocumentNode.SelectSingleNode("//img[@id='product_image']");
                element.SetAttributeValue("src", configurationKeyValues["ProductImageLink"] == "" ? "https://i.imgur.com/ko8F6LC.png" : configurationKeyValues["ProductImageLink"]);

                element = document.DocumentNode.SelectSingleNode("//header/div/img");
                element.SetAttributeValue("src", configurationKeyValues["LogoLink"] == "" ? "https://i.imgur.com/ko8F6LC.png" : configurationKeyValues["LogoLink"]);

                HtmlNode parentElement = document.DocumentNode.SelectSingleNode("//nav/ul");

                int count = Convert.ToInt16(configurationKeyValues["NavLinkCount"]);

                for (int i = 0; i < count; i++)
                {
                    element = HtmlNode.CreateNode("<li><a href='#' style='color: #D4AA00;'>Navigation item</a></li>");
                    parentElement.AppendChild(element);
                }

                parentElement = document.DocumentNode.SelectSingleNode("//ul[@id='arguments']");

                count = Convert.ToInt16(configurationKeyValues["ArgumentsCount"]);

                for (int i = 0; i < count; i++)
                {
                    element = HtmlNode.CreateNode("<li style='color: #4e4e4e;'><i class='fas fa-certificate'></i>Argument</li>");
                    parentElement.AppendChild(element);
                }

                parentElement = document.DocumentNode.SelectSingleNode("//div[@id='info_items_row_1']/div[@class='wrapper']/div[@id='info_items']");
                HtmlNode parentElementSecond = document.DocumentNode.SelectSingleNode("//div[@id='info_items_row_2']/div[@class='wrapper']/div[@id='info_items']");

                count = Convert.ToInt16(configurationKeyValues["TexboxCount"]);

                for (int i = 0; i < count; i++)
                {
                    element = HtmlNode.CreateNode("<div id='info_item'><h2 style='color: #D4AA00;'>Headline</h2>" +
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
                element = document.DocumentNode.SelectSingleNode("//header/div/img");
                element.SetAttributeValue("src", configurationKeyValues["LogoLink"] == "" ? "https://i.imgur.com/ko8F6LC.png" : configurationKeyValues["LogoLink"]);

                HtmlNode parentElement = document.DocumentNode.SelectSingleNode("//div[@class='gallery']");
                int count = Convert.ToInt16(configurationKeyValues["GallaryImageCount"]);

                string styleString = "";
                string imageSourceString = "";
                int back_i = count;
                for (int i = 0; i < count; i++)
                {
                    element = HtmlNode.CreateNode("<label  style='width:" + (90 / count) + "%' for='image_input_" + (back_i) + "' id='image_" + (back_i) + "'></label>");
                    parentElement.PrependChild(element);
                    if (back_i == 1)
                    {
                        element = HtmlNode.CreateNode("<input type='radio' name ='image_list' id='image_input_" + (back_i) + "' checked>");
                        styleString += "#image_input_" + (i + 1) + ":checked~#selected_image{background-image: url('" + (configurationKeyValues["ProductImageLink"] == "" ? "https://i.imgur.com/ko8F6LC.png" : configurationKeyValues["ProductImageLink"]) + "');}";
                        imageSourceString += ".gallery #image_" + (i + 1) + " {background-image: url('" + (configurationKeyValues["ProductImageLink"] == "" ? "https://i.imgur.com/ko8F6LC.png" : configurationKeyValues["ProductImageLink"]) + "');}";
                    }
                    else
                    {
                        element = HtmlNode.CreateNode("<input type='radio' name ='image_list' id='image_input_" + (back_i) + "'>");
                        styleString += "#image_input_" + (i + 1) + ":checked~#selected_image{background-image: url('https://i.imgur.com/ko8F6LC.png');}";
                        imageSourceString += ".gallery #image_" + (i + 1) + " {background-image: url('https://i.imgur.com/ko8F6LC.png');}";
                    }
                    back_i--;
                    parentElement.PrependChild(element);
                }

                parentElement = document.DocumentNode.SelectSingleNode("//style[@id='gallery_tech']");
                parentElement.InnerHtml = styleString;

                parentElement = document.DocumentNode.SelectSingleNode("//style[@id='gallery_pictures']");
                parentElement.InnerHtml = imageSourceString;

                parentElement = document.DocumentNode.SelectSingleNode("//ul[@id='arguments']");

                count = Convert.ToInt16(configurationKeyValues["ArgumentsCount"]);

                for (int i = 0; i < count; i++)
                {
                    element = HtmlNode.CreateNode("<li style='color: #3b3b3b;'><i class='fas fa-certificate'></i>Argument</li>");
                    parentElement.AppendChild(element);
                }

                parentElement = document.DocumentNode.SelectSingleNode("//div[@id='description_items']");
                HtmlNode tabElement = parentElement.SelectSingleNode("div[@id='info_tabs']");

                count = Convert.ToInt16(configurationKeyValues["TexboxCount"]);
                styleString = "";
                back_i = count;
                for (int i = 0; i < count; i++)
                {
                    element = HtmlNode.CreateNode("<label for='tab_button_" + (back_i) + "' style='width: " + 100 / count + "%;'>Title</label>");
                    parentElement.PrependChild(element);

                    if (back_i == 1)
                    {
                        element = HtmlNode.CreateNode("<input type='radio' name='tabs' id='tab_button_" + (back_i) + "' checked>");
                    }
                    else
                    {
                        element = HtmlNode.CreateNode("<input type='radio' name='tabs' id='tab_button_" + (back_i) + "'>");
                    }

                    parentElement.PrependChild(element);
                    back_i--;

                    element = HtmlNode.CreateNode("<h4 id='mobile_title' style='color: #D4AA00;'>Title</h4>");
                    tabElement.AppendChild(element);

                    element = HtmlNode.CreateNode("<p class='tab_div_" + (i + 1) + "' id='tab_p' style='color: #3b3b3b;'> " +
                                                          "quis voluptatum obcaecati deserunt unde odit expedita, ratione numquam " +
                                                          "sunt quidem adipisci animi saepe impedit nam repellendus? Ipsum, modi. " +
                                                          "Lorem ipsum, dolor sit amet consectetur adipisicing elit. " +
                                                          "Blanditiis rerum possimus cum, sapiente quibusdam, " +
                                                          "optio ab provident earum animi ipsum ipsa reprehenderit hic! " +
                                                          "Numquam, doloribus recusandae cumque quibusdam aliquam natus?" +
                                                   "</p>");
                    tabElement.AppendChild(element);

                    styleString += "#tab_button_" + (i + 1) + ":checked ~ #info_tabs .tab_div_" + (i + 1) + ",";
                }

                // Cut the last comma
                styleString = styleString.Substring(0, styleString.Length - 1);

                parentElement = document.DocumentNode.SelectSingleNode("//style[@id='tab_item_tech']");
                parentElement.InnerHtml = "#description_items [class^='tab_div_']{display: none;}" +
                                          styleString + "{display:block}";

                parentElement = document.DocumentNode.SelectSingleNode("//div[@id='similar_products']");

                count = Convert.ToInt16(configurationKeyValues["NavLinkCount"]);

                for (int i = 0; i < count; i++)
                {
                    element = HtmlNode.CreateNode("<a href='#' style='background-color: rgba(255,255,246,0.02);border: 1px solid #000000;'>" +
                                                    "<img src='https://i.imgur.com/ko8F6LC.png'>" +
                                                    "<p style='color: #3B3B3B;'>" +
                                                        "Blanditiis rerum possimus cum, sapiente quibusdam" +
                                                    "</p>" +
                                                  "</a>");
                    parentElement.AppendChild(element);
                }
            }

            return document.DocumentNode.OuterHtml;
        }

        public void Export(string path)
        {
            string exportDocument = document.DocumentNode.SelectSingleNode("//head").InnerHtml + document.DocumentNode.SelectSingleNode("//body").OuterHtml;
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine(exportDocument);
            }
        }
    }
}
