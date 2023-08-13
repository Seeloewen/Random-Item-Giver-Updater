﻿/////////////////////////////////////////////////////////////////////
//                                                                 //
// SealLib vDev                                                    //
// Sunday, 13th August 2023                                        //
// Created by Seeloewen                                            //
//                                                                 //
// Simple library that contains some code that is used by my apps. //
// You are free to use this library in your apps if you desire.    //
//                                                                 //
/////////////////////////////////////////////////////////////////////

using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System;
using Random_Item_Giver_Updater;
using System.Windows.Media.Animation;
using System.Windows.Forms.VisualStyles;
using System.Text;
using System.Windows.Markup;

namespace SeeloewenLib
{

    public class SeeloewenLibTools
    {
        public string ConvertListToString(List<string> list)
        {
            //Set the default output string
            string outputString = "";

            //Add each item in the list as a new line in the string
            foreach (string item in list)
            {
                outputString = string.Format("{0}{1}\n", outputString, item);
            }

            //Output the string
            return outputString;
        }
    }

    public class Wizard
    {
        //Attributes
        public List<WizardPage> pages = new List<WizardPage>();
        public GroupBox gbWizard;
        public Button btnContinue;
        public Button btnBack;
        public int currentPage = 1;
        public int pagesAmount = 0;
        public Action codeCancel = null;
        public Action codeFinish = null;

        //-- Constructor --//

        public Wizard(int pagesAmount, int height, int width, Button btnContinue, Button btnBack, Action codeCancel, Action codeFinish, Thickness margin)
        {
            //Create references
            this.btnContinue = btnContinue;
            this.btnBack = btnBack;
            this.pagesAmount = pagesAmount;
            this.codeCancel = codeCancel;
            this.codeFinish = codeFinish;

            //Create Wizard GroupBox
            gbWizard = new GroupBox();
            gbWizard.Width = width;
            gbWizard.Height = height;
            gbWizard.Margin = margin;

            //Setup buttons
            this.btnContinue.Click += new RoutedEventHandler(btnContinue_Click);
            this.btnBack.Click += new RoutedEventHandler(btnBack_Click);

            //Create pages based on amount
            for (int i = 0; i < pagesAmount; i++)
            {
                pages.Add(new WizardPage(i + 1, string.Format("Step {0}", i + 1), defaultRequirement, true, true, "Cannot continue to the next page because the requirements are not fulfilled."));
            }

            //Show first page
            ShowPage(1);
        }

        //-- Custom Methods --//

        public void ShowPage(int pageNum)
        {
            if (pages[pageNum - 1].requirements() == true)
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
                if (pages[pageNum - 1].canGoBack == true)
                {
                    btnBack.IsEnabled = true;
                }
                else
                {
                    btnBack.IsEnabled = false;
                }
                if (pages[pageNum - 1].canContinue == true)
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
            }
            else
            {
                //Show error message
                MessageBox.Show(pages[pageNum - 1].requirementsNotFulfilledMsg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

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

        public bool defaultRequirement()
        {
            //Default requirement used by the pages, always returns true
            return true;
        }

        //-- Event Handlers --//
        private void btnContinue_Click(object sender, RoutedEventArgs e)
        {
            //Show the next page
            ShowNextPage();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            //Show the previous page
            ShowPreviousPage();
        }
    }

    public class WizardPage
    {
        //Attributes
        public Grid grdContent;
        public Action code = null;
        public string header;
        public int pageNum;
        public Func<bool> requirements;
        public bool canGoBack;
        public bool canContinue;
        public string requirementsNotFulfilledMsg;

        //-- Constructor --//

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

        //-- Custom Methods --//

        public void ExecuteCode()
        {
            //Run the code that is assigned to this page
            code?.Invoke();
        }
    }
}