﻿using System;
using System.Collections.Generic;
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

namespace SudokuSolverUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Model.CellModel cell;

        public MainPage()
        {
            this.InitializeComponent();
            this.cell = new Model.CellModel(this.Dispatcher, new SudokuSolverLib.SudokuCell(3, 4));
            this.cell.SetPossible(new int[] { 5, 7 });
            DataContext = this;
        }

        public Model.CellModel Cell1
        {
            get { return cell; }
            set { this.cell = value; }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.cell.SolvedValue = 7;
        }
    }
}
