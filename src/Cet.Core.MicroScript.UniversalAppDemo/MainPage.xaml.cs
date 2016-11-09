using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Cet.Core.MicroScript.UniversalAppDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }


        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            this.TxtOutput.Text= string.Empty;

            var engine = new ScriptEngine();
            engine.CodeStream = Compiler.Compile(this.TxtInput.Text ?? string.Empty);

            var xdata = new MyDataAccess()
            {
                TxtOutput = this.TxtOutput,
            };

            RunResult engres = engine.Execute(xdata);

            this.TxtOutput.Text += $"Execution complete: {engres.Result}" + Environment.NewLine;
        }


        class MyDataAccess : IDataAccess
        {
            public TextBox TxtOutput;
            public Dictionary<string, object> Parameters = new Dictionary<string, object>();

            public object GetData(string name)
            {
                return this.Parameters[name];
            }

            public void SetData(string name, object value)
            {
                if (name == "out")
                {
                    this.TxtOutput.Text += $"{value}" + Environment.NewLine;
                }
            }
        }

    }
}
