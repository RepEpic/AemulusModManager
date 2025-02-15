﻿using System;
using System.Windows;
using AemulusModManager.Utilities;

namespace AemulusModManager.Windows
{
    /// <summary>
    /// Interaction logic for UpdateFileBox.xaml
    /// </summary>
    public partial class PackageFolderBox : Window
    {
        public string chosenFolder;
        public PackageFolderBox(string[] folders, string packageName)
        {
            InitializeComponent();
            FileGrid.ItemsSource = folders;
            FileGrid.SelectedIndex = 0;
            Title = $"Aemulus Package Manager - {packageName}";
            Platform.PlayNotificationSound();
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {

            string selectedItem = (string)FileGrid.SelectedItem;
            chosenFolder = selectedItem;
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }

}
