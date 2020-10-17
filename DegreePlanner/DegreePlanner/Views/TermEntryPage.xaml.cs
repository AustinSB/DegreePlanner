using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DegreePlanner.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DegreePlanner.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermEntryPage : ContentPage
    {
        Term term;

        public TermEntryPage()
        {
            InitializeComponent();
        }
        
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            term = (Term)BindingContext;
            
            coursesView.ItemsSource = await App.Database.GetCoursesAsync(term);

            DeleteButtonEnable();
            CourseCount();
            Validate.SetMinimumDate(startPicker, endPicker);
        }

        //Methods
        //If there are six courses associated with the term, disable the "Add Course" button
        async void CourseCount()
        {
            if (await App.Database.IsCourseCountSix(term))
            {
                addCourseButton.IsEnabled = false;
            }
            else
            {
                addCourseButton.IsEnabled = true;
            }
        }

        private void DeleteButtonEnable()
        {
            if (term.Id <= 0)
            {
                deleteTermButton.IsEnabled = false;
            }
            else
            {
                deleteTermButton.IsEnabled = true;
            }
        }

        //View Controls
        

        //Add new Course, Navigate to Course Entry Page
        async void OnAddCourseClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CourseEntryPage
            {
                BindingContext = new Course(),
                SelectedTermId = term.Id
            });
        }

        //Edit Course, Navigate to Course Entry Page
        async void OnCoursesViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                await Navigation.PushAsync(new CourseEntryPage
                {
                    BindingContext = e.SelectedItem as Course,
                    SelectedTermId = term.Id
                });
            }
        }

        //Save
        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            if (await ValidateTerm() == false)
            {
                return;
            }

            term.Title = titleEditor.Text;
            term.Start = startPicker.Date;
            term.End = endPicker.Date;

            await App.Database.SaveTermAsync(term);
            await Navigation.PopAsync();
        }

        //Delete
        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            await App.Database.DeleteTermAsync(term);
            await Navigation.PopAsync();
        }

        //Validate
        async Task<bool> ValidateTerm()
        {
            if (Validate.IsStartDateBeforeEndDate(startPicker, endPicker) == false)
            {
                await DisplayAlert("Invalid Dates", "Start date must be before End date", "Ok");
                return false;
            }

            if (Validate.DoesEntryContainText(titleEditor.Text) == false)
            {
                await DisplayAlert("Invalid Title", "Title must have a value", "Ok");
                return false;
            }

            return true;
        }
    }
}