﻿/////////////////////////////////////////////////////////////////////
//                                                                 //
// SeeloewenLib v1.2.0                                             //
// Tuesday, 12th November 2024                                     //
// Created by Seeloewen                                            //
//                                                                 //
// Simple library that contains some code that is used by my apps. //
// You are free to use this library in your apps if you wish.      //
// Make sure to acknowledge the license at the bottom of the file. //
//                                                                 //
// Find the library on GitHub:                                     //
// https://github.com/Seeloewen/SeeloewenLib                       //
//                                                                 //
/////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace SeeloewenLib;

/// <summary>
/// Contains several useful tools that can be used on their own
/// </summary>
public static class Tools
{
    /// <summary>
    /// Gets the parent of the specified element and (depending on the control) sets its content to null
    /// </summary>
    public static void RemoveFromParent(UIElement element) //Possibly incomplete
    {
        //Get the parent of the specified UI element and remove the element from its parent
        DependencyObject parent = VisualTreeHelper.GetParent(element);

        //Check the possible variations
        if (parent != null)
        {
            Panel parentAsPanel = parent as Panel;
            if (parentAsPanel != null)
            {
                parentAsPanel.Children.Remove(element);
            }
            ContentControl parentAsContentControl = parent as ContentControl;
            if (parentAsContentControl != null)
            {
                parentAsContentControl.Content = null; //Warning when using this: might also remove other content
            }
            Decorator parentAsDecorator = parent as Decorator;
            if (parentAsDecorator != null)
            {
                parentAsDecorator.Child = null;
            }
        }
    }

    /// <summary>
    /// Starts a timer that ticks once after the specified amount of seconds. The action will be invoked when the timer is done.
    /// </summary>
    public static void Delay(int seconds, Action function)
    {
        DispatcherTimer delayTimer = new DispatcherTimer();

        delayTimer.Interval = new TimeSpan(0, 0, seconds);
        delayTimer.Tick += (object sender, EventArgs args)
            =>
        {
            function.Invoke();
            delayTimer.Stop();
        };

        delayTimer.Start();
    }

    /// <summary>
    /// Takes a list and returns a string that contains all the items, seperated by line-breaks
    /// </summary>
    public static string ConvertListToString(List<string> list)
    {
        string output = "";

        //Add each item in the list as a new line in the string
        foreach (string item in list)
        {
            output = $"{output}{item}\n";
        }

        return output;
    }

    /// <summary>
    /// Gets the visual parent of an object, if possible
    /// </summary>
    public static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
    {
        //Get the parent object
        return VisualTreeHelper.GetParent(child) as T;
    }

    /// <summary>
    /// Gets the visual child of an object, if possible
    /// </summary>
    public static T FindVisualChild<T>(DependencyObject parent, int index) where T : DependencyObject
    {
        //Get the child corresponding to the index and parent
        DependencyObject child = VisualTreeHelper.GetChild(parent, index);

        return child is T typedChild ? typedChild : null;
    }

    /// <summary>
    /// Converts a number into bigger units as long as possible and adds the unit
    /// </summary>
    private static string ConvertNumberUnit(double number)
    {
        string unit = "";

        if (number > 1000)
        {
            //Number is one thousand or more
            unit = "k";
            number /= 1000;

            if (number > 1000)
            {
                //Number is one million or more
                unit = "m";
                number /= 1000;

                if (number > 1000)
                {
                    //Number is one billion or more
                    unit = "b";
                    number /= 1000;

                    if (number > 1000)
                    {
                        //Number is one trillion ore more
                        unit = "t";
                        number /= 1000;
                    }
                }
            }
        }


        //Return the combination of number and unit
        return $"{number}{unit}";
    }
}

/// <summary>
/// Simple Save System that saves settings as text files and reads them into variables.
/// [!] Should only be used when absolutely necessary, no longer receives major updates.
/// </summary>
public class SaveSystem
{
    public List<SaveEntry> saveEntries = new List<SaveEntry>();

    public readonly string path;
    public readonly string saveFileHeader;
    public readonly string saveFileName;

    /// <summary>
    /// Creates an instance of a Save System. Make sure to create your SaveEntries afterwards.
    /// </summary>
    public SaveSystem(string path, string saveFileHeader, string saveFileName)
    {
        this.path = path;
        this.saveFileHeader = saveFileHeader;
        this.saveFileName = saveFileName;
    }

    /// <summary>
    /// Saves the settings entries to the path specified in the constructor
    /// </summary>
    public void Save()
    {
        List<string> file =
        [
            saveFileHeader,
        ];

        //Save the settings to the file, depending on whether it's a category or entry
        foreach (SaveEntry saveEntry in saveEntries)
        {
            if (saveEntry.isCategory)
            {
                file.Add($"\n#{saveEntry.name}");
            }
            else
            {
                file.Add($"{saveEntry.name}={saveEntry.content}");
            }
        }
        File.WriteAllLines($"{path}/{saveFileName}.txt", file);
    }

    /// <summary>
    /// Loads the settings from the path specified in the constructor into save entries
    /// </summary>
    public void Load()
    {
        //Read the settings from the file
        IEnumerable<string> file = File.ReadLines($"{path}/{saveFileName}.txt");
        IEnumerable<string> oldFile;

        bool oldFileSaveNeeded = false;

        foreach (string entry in file)
        {
            string[] entrySplit = entry.Split('=');

            //Check if the entry is not empty and not the header
            if (entry != saveFileHeader && !string.IsNullOrEmpty(entry))
            {
                foreach (SaveEntry saveEntry in saveEntries)
                {
                    //Match the loaded entry to an existing entry
                    if (saveEntry.name == entrySplit[0] && !saveEntry.isCategory)
                    {
                        //Check for corruption. If there is none, load the entry from the file
                        if (!IsCorrupted(saveEntry, entrySplit[1]))
                        {
                            saveEntry.content = entrySplit[1];
                        }
                        else
                        {
                            //If there is corruption, ask the user if they want to correct it
                            MessageBoxResult dialogResult = MessageBox.Show($"The save entry you are trying to load is corrupted and will most likely not work ({saveEntry.name} with content {entrySplit[1]}). Do you want to try to correct it?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                            if (dialogResult == MessageBoxResult.Yes)
                            {
                                //Correct the corruption
                                oldFile = file;
                                oldFileSaveNeeded = true;
                                saveEntry.content = saveEntry.possibleValues[0];
                                MessageBox.Show($"The save entry {saveEntry.name} was corrected to {entrySplit[0]}.", "Corrected", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                saveEntry.content = entrySplit[1];
                            }
                        }
                    }
                }
            }
        }

        //Save the old file and new file if a conversion because of corruption happended
        if (oldFileSaveNeeded)
        {
            File.WriteAllLines($"{path}/{saveFileName}-{DateTime.Now.ToFileTime()}.old", file);
            Save();
        }
    }

    /// <summary>
    /// Checks if an entry is corrupted by comparing it to a list of expected values (if needed)
    /// </summary>
    public bool IsCorrupted(SaveEntry EntryTemplate, string entry)
    {
        //Check if the entry even needs to be checked
        if (EntryTemplate.hasDefinedValues)
        {
            //Check if the entry is one of the possible values
            foreach (string possibleValue in EntryTemplate.possibleValues)
            {
                if (possibleValue == entry)
                {
                    return false;
                }
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Sets the value of an entry to the specified content
    /// </summary>
    public void SetEntry(string name, string content)
    {
        //Sets the given entry to the given content
        foreach (SaveEntry entry in saveEntries)
        {
            if (entry.name == name)
            {
                if (entry.isCategory)
                {
                    throw new Exception("Cannot set content for a SaveSystem category");
                }

                entry.content = content;
                return;
            }
        }

        throw new Exception($"Cannot set content for entry {name} as it doesn't exist");
    }

    /// <summary>
    /// Gets the value of the specified entry
    /// </summary>
    public string GetEntry(string name)
    {
        //Gets the content of the given entry
        foreach (SaveEntry entry in saveEntries)
        {
            if (entry.name == name)
            {
                if (entry.isCategory)
                {
                    throw new Exception("Cannot get content for a SaveSystem category");
                }

                return entry.content;
            }
        }

        throw new Exception($"Cannot get content for entry {name} as it doesn't exist");
    }
}

/// <summary>
/// Save Entry that goes along with the Save System
/// </summary>
public class SaveEntry
{
    public readonly string name;
    public string content;
    public readonly bool isCategory;
    public readonly bool hasDefinedValues; //If this is false, a corruption check will be skipped
    public string[] possibleValues;
    private int index;

    /// <summary>
    /// Creates an instance of a Save Entry
    /// </summary>
    public SaveEntry(string name, string defaultContent, bool isCategory, bool hasDefinedValues, string[] possibleValues, int index)
    {
        this.name = name;
        content = defaultContent;
        this.isCategory = isCategory;
        this.hasDefinedValues = hasDefinedValues;
        this.possibleValues = possibleValues;
        this.index = index;
    }
}

/// <summary>
/// Wizard that manages multiple pages that contain content, with the ability to navigate between them
/// </summary>
public class Wizard
{
    public List<WizardPage> pages = new List<WizardPage>();
    public GroupBox gbWizard;
    public Button btnContinue;
    public Button btnBack;

    public int currentPage = 1;
    public readonly int pagesAmount = 0;

    public Action codeCancel;
    public Action codeFinish;

    /// <summary>
    /// Creates an instance of a Wizard
    /// </summary>
    public Wizard(int pagesAmount, int height, int width, Button btnContinue, Button btnBack, Action codeCancel, Action codeFinish, Thickness margin)
    {
        this.btnContinue = btnContinue;
        this.btnBack = btnBack;
        this.pagesAmount = pagesAmount;
        this.codeCancel = codeCancel;
        this.codeFinish = codeFinish;

        //Setup controls
        gbWizard.Width = width;
        gbWizard.Height = height;
        gbWizard.Margin = margin;
        this.btnContinue.Click += new RoutedEventHandler(btnContinue_Click);
        this.btnBack.Click += new RoutedEventHandler(btnBack_Click);

        //Create pages based on amount
        for (int i = 0; i < pagesAmount; i++)
        {
            pages.Add(new WizardPage(i + 1, $"Step {i + 1}", defaultRequirement, true, true, "Cannot continue to the next page because the requirements are not fulfilled."));
        }

        //Show first page
        ShowPage(1);
    }

    /// <summary>
    /// Shows the specified page
    /// </summary>
    public void ShowPage(int pageNum)
    {
        if (pages[pageNum - 1].requirements())
        {
            //Set button text based on page number
            if (pageNum > 1 && pageNum < pagesAmount)
            {
                btnContinue.Content = "Continue";
                btnBack.Content = "Back";
            }
            else if (pageNum <= 1)
            {
                btnContinue.Content = "Continue";
                btnBack.Content = "Cancel";
            }
            else if (pageNum >= pagesAmount)
            {
                btnContinue.Content = "Finish";
                btnBack.Content = "Back";
            }

            //Set button state based on page settings
            if (pages[pageNum - 1].canGoBack)
            {
                btnBack.IsEnabled = true;
            }
            else
            {
                btnBack.IsEnabled = false;
            }

            if (pages[pageNum - 1].canContinue)
            {
                btnContinue.IsEnabled = true;
            }
            else
            {
                btnContinue.IsEnabled = false;
            }

            //Only show page if it's in range
            if (pageNum > 0 && pageNum <= pagesAmount)
            {
                //Show page based on the page number and execute code
                currentPage = pageNum;
                gbWizard.Content = pages[pageNum - 1].grdContent;
                gbWizard.Header = pages[pageNum - 1].header;
                pages[pageNum - 1].ExecuteCode();
            }
            else
            {
                throw new IndexOutOfRangeException("WizardPage number was out of range");
            }
        }
        else
        {
            //Show error message
            MessageBox.Show(pages[pageNum - 1].requirementsNotFulfilledMsg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Show the next page or invoke the finish code, if there is no next page
    /// </summary>
    public void ShowNextPage()
    {
        if (currentPage < pagesAmount)
        {
            //Shows the next page in the wizard
            ShowPage(currentPage + 1);
        }
        else
        {
            //Invoke code, that gets run when users tries to go to the next page, even though it was the last page      
            codeFinish?.Invoke();
        }
    }

    /// <summary>
    /// Show the previous page or invoke the cancel code, if there is no previous page
    /// </summary>
    public void ShowPreviousPage()
    {
        if (currentPage > 1)
        {
            //Show the previous page in the wizard
            ShowPage(currentPage - 1);
        }
        else
        {
            //Invoke code, that gets run when user tries to go to the previous page even though it was the first page
            codeCancel?.Invoke();
        }
    }

    /// <summary>
    /// Requirement that the pages use by default, always returns true
    /// </summary>
    public bool defaultRequirement() => true; //Default requirement used by the pages, always returns true

    /// <summary>
    /// Click handler for the continue button, invokes "ShowNextPage()"
    /// </summary>
    private void btnContinue_Click(object sender, RoutedEventArgs e) => ShowNextPage();

    /// <summary>
    /// Click handler for the back button, invokes "ShowPreviousPage()"
    /// </summary>
    private void btnBack_Click(object sender, RoutedEventArgs e) => ShowPreviousPage();
}

/// <summary>
/// Wizard Page that goes along with the Wizard
/// </summary>
public class WizardPage
{
    //Attributes
    public Grid grdContent;
    public Action code = null;
    public Func<bool> requirements;

    public string header;
    private int pageNum;
    public readonly bool canGoBack;
    public readonly bool canContinue;
    public string requirementsNotFulfilledMsg;

    /// <summary>
    /// Creates an instance of a Wizard Page
    /// </summary>
    public WizardPage(int pageNum, string header, Func<bool> requirements, bool canGoBack, bool canContinue, string requirementsNotFulfilledMsg)
    {
        //Create references
        this.pageNum = pageNum;
        this.header = header;
        this.requirements = requirements;
        this.canGoBack = canGoBack;
        this.canContinue = canContinue;
        this.requirementsNotFulfilledMsg = requirementsNotFulfilledMsg;

        //Create the grid that contains the page content. You can add whatever you want to that grid, it's up to you what the page contains
        grdContent = new Grid();
    }

    /// <summary>
    /// Invokes the code that was previously specified in the constructor of the page
    /// </summary>
    public void ExecuteCode()
    {
        //Run the code that is assigned to this page
        code?.Invoke();
    }
}

////////////////////////////////////////////////////////////////////////////
//                                                                        //
// SeeloewenLib - A simple but powerful C# WPF library                    //
// Copyright(C) 2024 Louis/Seeloewen                                      //
//                                                                        //
// This program is free software: you can redistribute it and/or modify   //
// it under the terms of the GNU General Public License as published by   //
// the Free Software Foundation, either version 3 of the License, or      //
// (at your option) any later version.                                    //
//                                                                        //
// This program is distributed in the hope that it will be useful,        //
// but WITHOUT ANY WARRANTY; without even the implied warranty of         //
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          //
// GNU General Public License for more details.                           //
//                                                                        //
// You should have received a copy of the GNU General Public License      //
// along with this program.  If not, see <https://www.gnu.org/licenses/>. //
//                                                                        //
////////////////////////////////////////////////////////////////////////////