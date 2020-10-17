using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DegreePlanner.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DegreePlanner.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermsPage : ContentPage
    {
        public TermsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await App.Database.AddSampleData();

            termsView.ItemsSource = await App.Database.GetTermsAsync();
        }
        
        //Navigate to Terms Entry Page
        async void OnAddTermClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TermEntryPage
            {
                BindingContext = new Term()
            });
        }

        async void OnTermsViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                await Navigation.PushAsync(new TermEntryPage
                {
                    BindingContext = e.SelectedItem as Term
                });
            }
        }
    }
}