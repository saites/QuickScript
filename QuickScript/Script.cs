using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using QuickScript.Annotations;

namespace QuickScript
{
    [Serializable]
    public class Script : INotifyPropertyChanged
    {
        public Script() {}
        public Script(string filename)
        {
            Filename = filename;
        }

        public override string ToString()
        {
            return Filename != null ? Path.GetFileNameWithoutExtension(Filename) : string.Empty;
        }

        private string _filename;

        public string Filename
        {
            get { return _filename; }
            set
            {
                _filename = value;
                OnPropertyChanged();
                OnPropertyChanged("ScriptText");
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public string ScriptText
        {
            get
            {
                if (!File.Exists(Filename)) return "File does not exist";

                string scriptText;
                try
                {
                    scriptText = File.ReadAllText(Filename);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
                return scriptText;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
