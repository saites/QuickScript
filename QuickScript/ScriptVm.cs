using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using Microsoft.Win32;
using QuickScript.Annotations;
using MvvmFoundation.Wpf;

namespace QuickScript
{
    public class ScriptVm : INotifyPropertyChanged
    {
        public ScriptVm(string scriptLocation)
        {
            ScriptsLocation = scriptLocation;
            Scripts = new ObservableCollection<Script>();
            ReadScripts.Execute(null);
        }

        public Script SelectedScript { get; set; }

        private string _scriptsLocation;
        private string _errorMsg;

        public string ScriptsLocation
        {
            get { return _scriptsLocation; }
            set
            {
                _scriptsLocation = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Script> Scripts { get; private set; }

        public ICommand ReadScripts
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        try
                        {
                            var serializer = new XmlSerializer(typeof(ObservableCollection<Script>));
                            using (var fs = new FileStream(ScriptsLocation, FileMode.Open))
                            {
                                Scripts = (ObservableCollection<Script>)serializer.Deserialize(fs);
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorMsg = ex.Message;
                        }
                        OnPropertyChanged("Scripts");
                    });
            }
        }

        public ICommand SaveScripts
        {
            get
            {
                return new RelayCommand(() =>
                {
                    try
                    {
                        var serializer = new XmlSerializer(typeof(ObservableCollection<Script>));
                        using (var fs = new FileStream(ScriptsLocation, FileMode.Create))
                        {
                            serializer.Serialize(fs, Scripts);
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorMsg = ex.Message;
                    }
                });
            }
        }

        public ICommand RemoveScriptCmd
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        Scripts.Remove(SelectedScript);
                        SaveScripts.Execute(null);
                        OnPropertyChanged("Scripts");
                    });
            }
        }

        public ICommand AddScriptCmd
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        var odw = new OpenDialogWindow();
                        var fld = new OpenFileDialog();
                        odw.Show();
                        var res = fld.ShowDialog(odw);
                        odw.Close();
                        if (res != true) return;
                        odw.Close();
                        Scripts.Add(new Script(fld.FileName));
                        SaveScripts.Execute(null);
                        OnPropertyChanged("Scripts"); ;
                    });
            }
        }

        public string ErrorMsg
        {
            get { return _errorMsg; }
            set
            {
                _errorMsg = value;
                OnPropertyChanged();
            }
        }

        public ICommand RunCmd
        {
            get
            {
                return new RelayCommand(() =>
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(SelectedScript.Filename);
                        }
                        catch (Exception ex)
                        {
                            ErrorMsg = ex.Message;
                        }
                    });
            }
        }

        public ICommand QuitCmd
        {
            get
            {
                return new RelayCommand(() => Environment.Exit(0));
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
