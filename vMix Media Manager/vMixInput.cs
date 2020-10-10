using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace vMix_Media_Manager
{
    class vMixInput : INotifyPropertyChanged
    {
        string name;
        vMixInputType type;
        string path;
        bool online;
        XmlElement xmlElement;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        { 
            get
            {
                return name;
            }
            set
            {
                name = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }

        public string FileName
        {
            get
            {
                return System.IO.Path.GetFileName(path);
            }
        }

        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Path"));
            }
        }

        public bool Online
        {
            get
            {
                return online;
            }
            set
            {
                online = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Online"));
            }
        }

        public vMixInputType InputType
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("InputType"));
            }
        }

        public XmlElement XmlElement { get => xmlElement; set => xmlElement = value; }

        public void SetNewPath(string newPath)
        {
            if(type == vMixInputType.Video)
            {
                Path = newPath;
                Online = (File.Exists(newPath));
                xmlElement.InnerText = newPath;
            }
            else if(type == vMixInputType.List)
            {
                //slightly different
                string oldPath = path;
                Path = newPath;
                Online = File.Exists(newPath);
                string newValue = xmlElement.GetAttribute("Videos").Replace(oldPath, newPath);
                xmlElement.SetAttribute("Videos", newValue);
            }

        }
    }
}
