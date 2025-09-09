using System;
using System.Diagnostics;
using System.Threading;
using Windows.Gaming.Input.ForceFeedback;
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
using System.Threading.Tasks;
using System.Globalization;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Calculator
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
    public sealed partial class CurrencyCalc : Page
    {
        public CurrencyCalc()
        {
            this.InitializeComponent();
        }

		/// <summary>
		/// Static dictionary for currency conversion rates
		/// </summary>
		private static readonly Dictionary<string, Dictionary<string, double>> rates = new Dictionary<string, Dictionary<string, double>>
		{
			["USD"] = new Dictionary<string, double>
			{
				["EUR"] = 0.85189982,
				["GBP"] = 0.72872436,
				["INR"] = 74.257327,
				["USD"] = 1.0
			},
			["EUR"] = new Dictionary<string, double>
			{
				["USD"] = 1.1739732,
				["GBP"] = 0.8556672,
				["INR"] = 87.00755,
				["EUR"] = 1.0
			},
			["GBP"] = new Dictionary<string, double>
			{
				["USD"] = 1.371907,
				["EUR"] = 1.1686692,
				["INR"] = 101.68635,
				["GBP"] = 1.0
			},
			["INR"] = new Dictionary<string, double>
			{
				["USD"] = 0.011492628,
				["EUR"] = 0.013492774,
				["GBP"] = 0.0098339397,
				["INR"] = 1.0
			}
		};

		/// <summary>
		/// Calculates the converted currency amount, the forward rate, and the inverse rate
		/// </summary>
		/// <param name="amount"></param>
		/// <param name="fromCurrency"></param>
		/// <param name="toCurrency"></param>
		/// <returns></returns>
		public (double convertedAmount, double forwardRate, double inverseRate) GetConversionRates(double amount, string fromCurrency, string toCurrency)
		{
			double forwardRate = rates[fromCurrency][toCurrency];
			double inverseRate = rates[toCurrency][fromCurrency];

			double convertedAmount = amount * forwardRate;

			return (convertedAmount, forwardRate, inverseRate);
		}

		/// <summary>
		/// Gets the currency symbol based on the currency code
		/// </summary>
		/// <param name="currencyCode"></param>
		/// <returns></returns>
		private string GetCurrencySymbol(string currencyCode)
		{
			switch (currencyCode)
			{
				case "USD":
					return new CultureInfo("en-US").NumberFormat.CurrencySymbol;
				case "EUR":
					return new CultureInfo("en-IE").NumberFormat.CurrencySymbol;
				case "GBP":
					return new CultureInfo("en-GB").NumberFormat.CurrencySymbol;
				case "INR":
					return new CultureInfo("en-IN").NumberFormat.CurrencySymbol;
				default:
					return "";
			}
		}

		/// <summary>
		/// Gets the currency name based on the currency code
		/// </summary>
		/// <param name="currencyCode"></param>
		/// <returns></returns>
		private string GetCurrencyName(string currencyCode)
		{
			switch (currencyCode)
			{
				case "USD":
					return "US Dollar";
				case "EUR":
					return "Euro";
				case "GBP":
					return "British Pound";
				case "INR":
					return "Indian Rupee";
				default:
					return currencyCode;
			}
		}

		/// <summary>
		/// Handles the click event for the calculate button
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void calculateButton_Click(object sender, RoutedEventArgs e)
		{
			// Validate input
			if (string.IsNullOrWhiteSpace(inputDollar.Text))
			{
				var dialogMessage = new MessageDialog("Error! Please enter a number.");
				await dialogMessage.ShowAsync();
				inputDollar.Focus(FocusState.Programmatic);
				inputDollar.SelectAll();
				return;
			}
			// Check if input is a valid number
			if (!double.TryParse(inputDollar.Text, out double amount))
			{
				var dialogMessage = new MessageDialog("Error! Invalid number.");
				await dialogMessage.ShowAsync();
				inputDollar.Focus(FocusState.Programmatic);
				inputDollar.SelectAll();
				return;
			}

			// Get selected currencies and their names
			string fromCurrency = ((ComboBoxItem)CurrencyFrom.SelectedItem)?.Tag.ToString();
			string toCurrency = ((ComboBoxItem)CurrencyTo.SelectedItem)?.Tag.ToString();
			string fromName = GetCurrencyName(fromCurrency);
			string toName = GetCurrencyName(toCurrency);

			// Parse input amount, default to 0 if parsing fails
			double inputAmount = double.TryParse(inputDollar.Text, out inputAmount) ? inputAmount : 0;

			// Ensure both currencies are selected
			if (fromCurrency == null || toCurrency == null)
			{
				var dialogMessage = new MessageDialog("Error! Please select both currencies.");
				await dialogMessage.ShowAsync();
				return;
			}

			// Get conversion rates and converted amount
			var result = GetConversionRates(amount, fromCurrency, toCurrency);

			// format dollarAmount and convertedDollar textboxes
			dollarAmount.Text = GetCurrencySymbol(fromCurrency) + amount.ToString("N2") + (" ") + fromName;
			convertedDollar.Text = GetCurrencySymbol(toCurrency) + result.convertedAmount.ToString("N2") + (" ") + toName;

			// format toFrom textbox based on selected currency
			switch (fromCurrency)
			{
				case "EUR":
					toFromAmount.Text = GetCurrencySymbol(toCurrency) + ("1 " + toName + " = ") + GetCurrencySymbol(fromCurrency) + result.forwardRate.ToString() + (" ") + fromName;
					break;
				case "GBP":
					toFromAmount.Text = GetCurrencySymbol(toCurrency) + ("1 " + toName + " = ") + GetCurrencySymbol(fromCurrency) + result.forwardRate.ToString() + (" ") + fromName;
					break;
				case "INR":
					toFromAmount.Text = GetCurrencySymbol(toCurrency) + ("1 " + toName + " = ") + GetCurrencySymbol(fromCurrency) + result.forwardRate.ToString() + (" ") + fromName;
					break;
				case "USD":
				default:
					// Default to US Dollars or the system's local currency
					toFromAmount.Text = GetCurrencySymbol(toCurrency) + ("1 " + toName + " = ") + GetCurrencySymbol(fromCurrency) + result.forwardRate.ToString() + (" ") + fromName;
					break;
			}

			// format fromTo textbox based on selected currency
			switch (toCurrency)
			{
				case "EUR":
					fromToAmount.Text = GetCurrencySymbol(fromCurrency) + ("1 " + fromName + " = ") + GetCurrencySymbol(toCurrency) + result.inverseRate.ToString() + (" ") + toName;
					break;
				case "GBP":
					fromToAmount.Text = GetCurrencySymbol(fromCurrency) + ("1 " + fromName + " = ") + GetCurrencySymbol(toCurrency) + result.inverseRate.ToString() + (" ") + toName;
					break;
				case "INR":
					fromToAmount.Text = GetCurrencySymbol(fromCurrency) + ("1 " + fromName + " = ") + GetCurrencySymbol(toCurrency) + result.inverseRate.ToString() + (" ") + toName;
					break;
				case "USD":
				default:
					// Default to US Dollars or the system's local currency
					fromToAmount.Text = GetCurrencySymbol(fromCurrency) + ("1 " + fromName + " = ") + GetCurrencySymbol(toCurrency) + result.inverseRate.ToString() + (" ") + toName;
					break;
			}
		}

		// Handles the click event for the exit button
		private void exitButton_Click(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(MainMenu));
		}
	}
}
