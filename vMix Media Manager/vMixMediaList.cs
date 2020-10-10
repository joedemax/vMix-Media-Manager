using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace vMix_Media_Manager
{
    class vMixMediaList
    {
        BindingList<vMixInput> inputs = new BindingList<vMixInput>();
        XmlDocument vmixXml;

        string vMixXmlPath;
        
        public BindingList<vMixInput> Inputs
        {
            get
            {
                return inputs;
            }
        }

        public void Open(string path)
        {
            readXml(path);
            vMixXmlPath = path;
        }

        public void Save()
        {
            try
            {
                vmixXml.Save(Path.GetDirectoryName(vMixXmlPath) + "/" + Path.GetFileNameWithoutExtension(vMixXmlPath) + " - Fixed Path.vmix");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void readXml(string path)
        {
            vmixXml = new XmlDocument();
            vmixXml.Load(path);

            XmlNodeList nodes = vmixXml.GetElementsByTagName("Input");

            foreach (XmlElement input in nodes)
            {

                if(input.HasAttribute("Type"))
                {
                    int type = int.Parse(input.Attributes.GetNamedItem("Type").InnerText);

                    switch (type)
                    {
                        case 0:
                            addInput(input.Attributes.GetNamedItem("OriginalTitle").InnerText, input.InnerText, input, vMixInputType.Video);
                            break;
                        case 1:
                            addInput(input.Attributes.GetNamedItem("OriginalTitle").InnerText, input.InnerText, input, vMixInputType.Video);
                            break;
                        case 13:
                            addInput(input.Attributes.GetNamedItem("OriginalTitle").InnerText, input.InnerText, input, vMixInputType.Video);
                            break;
                        case 14:
                            addList(input.Attributes.GetNamedItem("OriginalTitle").InnerText, input.Attributes.GetNamedItem("Videos").InnerText, input);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void addList(string Title, string videos, XmlElement input)
        {
            string[] videoList = videos.Split('\n');

            foreach (var video in videoList)
            {
                string[] videoParams = video.Split('|');
                addInput(Title,videoParams[0], input, vMixInputType.List);
            }
        }

        private void addInput(string Title, string path, XmlElement input, vMixInputType type)
        {
            vMixInput vmixInput = new vMixInput();
            vmixInput.InputType = type;
            vmixInput.Name = Title;
            vmixInput.Path = path;
            vmixInput.Online = File.Exists(path);
            vmixInput.XmlElement = input;
            inputs.Add(vmixInput);
        }

    }
}
