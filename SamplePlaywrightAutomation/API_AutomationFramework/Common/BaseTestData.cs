using API_AutomationFramework.Helpers;
using System.Text;
using System.Xml;
using System.Xml.Linq;


namespace API_AutomationFramework.Common
{
    public class BaseTestData
    {
        private IDictionary<string, string> testDataValues = new Dictionary<string, string>();

        public static string? TestDataFolderName = null!;
        public static string? ResourceFile = null!;

        public virtual string? TestEnvironment { get; set; }

        public string CurrentResourceFile
        {
            get
            {
                return @".\TestData\" + TestDataFolderName + @"\" + ResourceFile + (string.IsNullOrEmpty(TestEnvironment) ? "" : "." + TestEnvironment) + ".xml";
            }
        }

        public string BaseResourceFile
        {
            get
            {
                return @".\TestData\" + TestDataFolderName + @"\" + ResourceFile + ".xml";
            }
        }

        public IDictionary<string, string> CachedTestData
        {
            get
            {
                if (testDataValues == null)
                    testDataValues = new Dictionary<string, string>();

                return testDataValues;
            }

            set
            {
                testDataValues = value;
            }
        }

        public void CacheTestData()
        {
            PopulateTestData(CurrentResourceFile);
            PopulateTestData(BaseResourceFile);
        }

        private void PopulateTestData(string inputXml)
        {
            using (XmlReader reader = XmlReader.Create(inputXml))
            {
                XElement root = XElement.Load(reader);

                // Get a sequence of users
                IEnumerable<XElement> users = root.Elements("data");

                reader.MoveToContent();
                reader.Read();
                while (!reader.EOF && reader.ReadState == ReadState.Interactive)
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.HasAttributes)
                    {
                        string? keyValue = reader.GetAttribute("name");

                        if ("true".Equals(("" + reader.GetAttribute("encrypt")).ToLower()))
                        {
                            reader.Read();
                            testDataValues.Add(keyValue!, Encoding.ASCII.GetString(Convert.FromBase64String(reader.Value)));
                        }
                        else
                        {
                            reader.Read();
                            testDataValues.Add(keyValue!, reader.Value);
                        }
                    }

                    reader.Read();
                }
            }
        }

        public object? GetTestData(string resourceKey)
        {
            if (File.Exists(CurrentResourceFile))
            {
                using (XmlReader reader = XmlReader.Create(CurrentResourceFile))
                {
                    reader.MoveToContent(); // will not advance reader if already on a content node; if successful, ReadState is Interactive
                    reader.Read();          // this is needed, even with MoveToContent and ReadState.Interactive
                    while (!reader.EOF && reader.ReadState == ReadState.Interactive)
                    {
                        if (reader.NodeType == XmlNodeType.Element && reader.HasAttributes && reader.GetAttribute("name")!.Equals(resourceKey))
                        {
                            if ("true".Equals(("" + reader.GetAttribute("encrypt")).ToLower()))
                            {
                                reader.Read();
                                return Encoding.ASCII.GetString(Convert.FromBase64String(reader.Value));
                            }
                            if (!string.IsNullOrEmpty("" + reader.GetAttribute("baseurl")))
                            {
                                string? baseUrl = "" + reader.GetAttribute("baseurl");
                                reader.Read();
                                return "" + TestRunSettings.getBaseUrl(baseUrl) + reader.Value;
                            }
                            else if ("true".Equals(("" + reader.GetAttribute("collection")).ToLower()))
                            {
                                List<string> colValue = new List<string>();
                                while (true)
                                {
                                    if (reader.NodeType == XmlNodeType.EndElement)
                                    {
                                        reader.Read();
                                        while (reader.NodeType == XmlNodeType.Whitespace)
                                            reader.Read();

                                        if (reader.NodeType == XmlNodeType.EndElement)
                                            break;
                                    }

                                    if (reader.NodeType == XmlNodeType.Text)
                                    {
                                        colValue.Add(reader.Value);
                                        reader.Read();
                                        while (reader.NodeType == XmlNodeType.Whitespace)
                                            reader.Read();
                                    }
                                    else
                                    {
                                        reader.Read();
                                        while (reader.NodeType == XmlNodeType.Whitespace)
                                            reader.Read();
                                    }
                                }

                                return colValue;
                            }
                            else
                            {
                                reader.Read();
                                return reader.Value;
                            }
                        }
                        else
                            reader.Read();
                    }
                }
            }

            using (XmlReader reader = XmlReader.Create(BaseResourceFile))
            {
                reader.MoveToContent(); // will not advance reader if already on a content node; if successful, ReadState is Interactive
                reader.Read();          // this is needed, even with MoveToContent and ReadState.Interactive
                while (!reader.EOF && reader.ReadState == ReadState.Interactive)
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.HasAttributes && reader.GetAttribute("name")!.Equals(resourceKey))
                    {
                        if ("true".Equals(("" + reader.GetAttribute("encrypt")).ToLower()))
                        {
                            reader.Read();
                            return Encoding.ASCII.GetString(Convert.FromBase64String(reader.Value));
                        }
                        else if ("true".Equals(("" + reader.GetAttribute("collection")).ToLower()))
                        {
                            List<string> colValue = new List<string>();
                            while (true)
                            {
                                if (reader.NodeType == XmlNodeType.EndElement)
                                {
                                    reader.Read();
                                    while (reader.NodeType == XmlNodeType.Whitespace)
                                        reader.Read();

                                    if (reader.NodeType == XmlNodeType.EndElement)
                                        break;
                                }

                                if (reader.NodeType == XmlNodeType.Text)
                                {
                                    colValue.Add(reader.Value);
                                    reader.Read();
                                    while (reader.NodeType == XmlNodeType.Whitespace)
                                        reader.Read();
                                }
                                else
                                {
                                    reader.Read();
                                    while (reader.NodeType == XmlNodeType.Whitespace)
                                        reader.Read();
                                }
                            }

                            return colValue;
                        }
                        else
                        {
                            reader.Read();
                            return reader.Value;
                        }
                    }
                    else
                        reader.Read();
                }
            }

            return null;
        }

    }
}
