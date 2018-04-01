using System;
using System.IO;
using System.Xml;

namespace delete_hidden_text
{
    class Program
    {
        static void Main(string[] args)
        {
            string input_xml_filename = @"document.xml";
            string output_xml_filename = @"output.xml";
            FileStream SourceStream = File.Open(input_xml_filename, FileMode.Open, FileAccess.Read);
            FileStream OutputStream = File.Open(output_xml_filename, FileMode.OpenOrCreate, FileAccess.Write);
            NameTable nt = get_NameTable_for_docx();
            XmlNamespaceManager nsManager = get_nsManager_for_docx(nt);
            XmlDocument xdoc = get_xml_document_from_stream(nt, SourceStream);
            XmlDocument output_xdoc = DeleteUselessText(xdoc,nsManager);
            save_xml_document_to_stream(output_xdoc,OutputStream);
        }

        private static NameTable get_NameTable_for_docx()
        {
            NameTable nt = new NameTable();
            return nt;
        }

        private static XmlNamespaceManager get_nsManager_for_docx(NameTable nt)
        {
            const string wordmlNamespace = "http://schemas.openxmlformats.org/wordprocessingml/2006/main";
            XmlNamespaceManager nsManager = new XmlNamespaceManager(nt);
            nsManager.AddNamespace("w", wordmlNamespace);
            return nsManager;
        }

        private static XmlDocument get_xml_document_from_stream(NameTable nt, Stream SourceStream)
        {

            XmlDocument xdoc = new XmlDocument(nt);
            xdoc.Load(SourceStream);
            return xdoc;
        }

        private static void save_xml_document_to_stream(XmlDocument xdoc, Stream OutputStream)
        {
            xdoc.Save(OutputStream);
        }

        public static XmlDocument DeleteUselessText(XmlDocument xdoc, XmlNamespaceManager nsManager)
        {
            XmlNodeList hiddenNodes = xdoc.SelectNodes("//w:vanish", nsManager);
            foreach (System.Xml.XmlNode hiddenNode in hiddenNodes)
            {
                XmlNode topNode = hiddenNode.ParentNode.ParentNode;
                XmlNode topParentNode = topNode.ParentNode;
                topParentNode.RemoveChild(topNode);
                if (!(topParentNode.HasChildNodes))
                {
                    topParentNode.ParentNode.RemoveChild(topParentNode);
                }
            }
            return xdoc;
        }
    }
}
