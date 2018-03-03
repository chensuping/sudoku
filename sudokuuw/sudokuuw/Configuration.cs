using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Sudokuuw
{
    public class Scenario
    {
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }

    public partial class MainPage : Page
    {
        public const string FEATURE_NAME = "Resize app view";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title="Directional navigation", ClassType=typeof(Sudokuuw.Views.GamePad)},
            new Scenario() { Title="Engaging focus", ClassType=typeof(Sudokuuw.Views.Help)},
            new Scenario() { Title="Legacy controls", ClassType=typeof(Sudokuuw.Views.Settings)},
        };
    }

    partial class App
    {
        partial void Construct()
        {
            // Setting RequiresPointerMode to WhenRequested sets the default behavior for
            // the entire app to enter mouse mode only when requested. This is typically
            // set in the App.xaml markup, but we do it here explicitly in code so that
            // the step is easier to see.
            this.RequiresPointerMode = Windows.UI.Xaml.ApplicationRequiresPointerMode.WhenRequested;
        }
    }

    
}
