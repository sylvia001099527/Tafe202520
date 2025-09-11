using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Calculator
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MortgageCalculator : Page
	{
		public MortgageCalculator()
		{
			this.InitializeComponent();
		}

		private async void calculateButton_Click(object sender, RoutedEventArgs e)
		{
			double principal, annualInterestRate, monthlyInterestRate, monthlyInterestRateDecimal, monthlyRepayment;
			int years, months, totalMonths;
			// validate principal borrowed field is a double
			try
			{
				principal = double.Parse(principalTextBox.Text);
			}
			catch
			{
				var dialog = new MessageDialog("Error. Please enter a number in the Principal Borrowed box.");
				await dialog.ShowAsync();
				principalTextBox.Focus(FocusState.Programmatic);
				principalTextBox.SelectAll();
				return;
			}
			// validate years field is an int
			try
			{
				years = int.Parse(yearsTextBox.Text);
			}
			catch
			{
				var dialog = new MessageDialog("Error. Please enter a number in the Years box.");
				await dialog.ShowAsync();
				yearsTextBox.Focus(FocusState.Programmatic);
				yearsTextBox.SelectAll();
				return;
			}
			// validate months field is an int
			try
			{
				months = int.Parse(monthsTextBox.Text);
			}
			catch
			{
				var dialog = new MessageDialog("Error. Please enter a number in the Months box.");
				await dialog.ShowAsync();
				monthsTextBox.Focus(FocusState.Programmatic);
				monthsTextBox.SelectAll();
				return;
			}
			// validate annual interest field is a double
			try
			{
				annualInterestRate = double.Parse(annualInterestTextBox.Text);
			}
			catch
			{
				var dialog = new MessageDialog("Error. Please enter a number in the Annual Interest Rate box.");
				await dialog.ShowAsync();
				annualInterestTextBox.Focus(FocusState.Programmatic);
				annualInterestTextBox.SelectAll();
				return;
			}
			// validate number in principal borrowed is greater than zero
			if (principal <= 0)
			{
				var dialog = new MessageDialog("Error. Please enter a number higher than 0 in the Principal box.");
				await dialog.ShowAsync();
				principalTextBox.Focus(FocusState.Programmatic);
				principalTextBox.SelectAll();
				return;
			}
			// validate number in years field is a positive number
			if (years < 0)
			{
				var dialog = new MessageDialog("Error. Please enter a positive number in the Years box.");
				await dialog.ShowAsync();
				yearsTextBox.Focus(FocusState.Programmatic);
				yearsTextBox.SelectAll();
				return;
			}
			// validate number in months field is a positive number
			if (months < 0)
			{
				var dialog = new MessageDialog("Error. Please enter a positive number in the Months box.");
				await dialog.ShowAsync();
				monthsTextBox.Focus(FocusState.Programmatic);
				monthsTextBox.SelectAll();
				return;
			}
			// validate number in annual interest field is greater than zero
			if (annualInterestRate <= 0)
			{
				var dialog = new MessageDialog("Error. Please enter a number higher than 0 in the Annual Interest Rate box.");
				await dialog.ShowAsync();
				annualInterestTextBox.Focus(FocusState.Programmatic);
				annualInterestTextBox.SelectAll();
				return;
			}
			// validate that both years and months fields aren't each set to zero
			if (years == 0 && months == 0)
			{
				var dialog = new MessageDialog("Error. Please enter a number higher than 0 in the Years or Months box.");
				await dialog.ShowAsync();
				yearsTextBox.Focus(FocusState.Programmatic);
				yearsTextBox.SelectAll();
				return;
			}
			// convert years to months
			totalMonths = (years * 12) + months;
			// convert annual interest rate to monthly interest rate
			monthlyInterestRate = annualInterestRate / 12;
			// send monthly interest rate to text box
			monthlyInterestTextBox.Text = monthlyInterestRate.ToString("F2");
			// convert monthly interest rate to decimal from percentage
			monthlyInterestRateDecimal = monthlyInterestRate / 100;
			// calculate monthly repayment
			monthlyRepayment = principal * (monthlyInterestRateDecimal * Math.Pow(1 + monthlyInterestRateDecimal, totalMonths)) / (Math.Pow(1 + monthlyInterestRateDecimal, totalMonths) - 1);
			// send monthly repayment to text box
			repaymentTextBox.Text = monthlyRepayment.ToString("C2");
		}

		private void exitButton_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(MainMenu));
		}
	}
}
