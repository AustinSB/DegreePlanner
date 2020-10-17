using DegreePlanner.Models;
using Plugin.LocalNotifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DegreePlanner.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssessmentEntryPage : ContentPage
    {
        Assessment assessment;
        Course course;
        public Course SelectedCourse
        {
            get { return course; }
            set { course = value; }
        }

        public AssessmentEntryPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            assessment = (Assessment)BindingContext;

            DeleteButtonEnable();
            PopulateTypePicker();
            SetSwitchToggles();
            Validate.SetMinimumDate(startPicker, endPicker);
        }

        //Methods
        private void SetSwitchToggles()
        {
            if (assessment.StartAlertSet) { startDateSwitch.IsToggled = true; }
            if (assessment.EndAlertSet) { endDateSwitch.IsToggled = true; }
        }

        private void PopulateTypePicker()
        {
            typePicker.Items.Add("Performance");
            typePicker.Items.Add("Objective");

            ChangeTypeOptions("Performance");
            ChangeTypeOptions("Objective");

            if (assessment.Id != 0)
            {
                typePicker.Items.Add(assessment.AssessType);
                typePicker.SelectedItem = assessment.AssessType;
            }
            else
            {
                typePicker.SelectedIndex = 0;
            }
        }

        //If other assessment type was already added to selected course, remove that option from the picker
        async void ChangeTypeOptions(string assessType)
        {
            if (await App.Database.IsAssessmentTypeAdded(SelectedCourse, assessType))
            {
                typePicker.Items.Remove(assessType);
            }
        }

        //Create or cancel alerts based on switch's toggle and whether alerts are already set
        private void ShowCancelAssessmentAlerts()
        {
            if (startDateSwitch.IsToggled == true && assessment.StartAlertSet == false)
            {
                CrossLocalNotifications.Current.Show("Assessment Starting", 
                    $"'{assessment.Name}' starts today", assessment.Id + 1000, assessment.Start);
                assessment.StartAlertSet = true;
            }
            else if (startDateSwitch.IsToggled == false && assessment.StartAlertSet == true)
            {
                CrossLocalNotifications.Current.Cancel(assessment.Id + 1000);
                assessment.StartAlertSet = false;
            }

            if (endDateSwitch.IsToggled == true && assessment.EndAlertSet == false)
            {
                CrossLocalNotifications.Current.Show("Assessment Ending", 
                    $"{assessment.Name} ends today", assessment.Id + 2000, assessment.End);
                assessment.EndAlertSet = true;
            }
            else if (endDateSwitch.IsToggled == false && assessment.EndAlertSet == true)
            {
                CrossLocalNotifications.Current.Cancel(assessment.Id + 2000);
                assessment.EndAlertSet = false;
            }
        }

        private void DeleteButtonEnable()
        {
            if (assessment.Id <= 0)
            {

                deleteAssessmentButton.IsEnabled = false;
            }
            else
            {
                deleteAssessmentButton.IsEnabled = true;
            }
        }

        //View Controls
        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            if (await ValidateAssessment() == false)
            {
                return;
            }

            assessment.Name = nameEntry.Text;
            assessment.CourseId = SelectedCourse.Id;
            assessment.Start = startPicker.Date;
            assessment.End = endPicker.Date;
            assessment.AssessType = typePicker.SelectedItem.ToString();

            ShowCancelAssessmentAlerts();

            await App.Database.SaveAssessmentAsync(assessment);
            await Navigation.PopAsync();
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if(assessment.StartAlertSet == true) 
            { 
                CrossLocalNotifications.Current.Cancel(assessment.Id + 1000); 
                assessment.StartAlertSet = false; 
            }
            if(assessment.EndAlertSet == true) 
            { 
                CrossLocalNotifications.Current.Cancel(assessment.Id + 2000); 
                assessment.EndAlertSet = false; 
            }

            await App.Database.DeleteAssessmentAsync(assessment);
            await Navigation.PopAsync();
        }

        //Validate
        async Task<bool> ValidateAssessment()
        {
            if (Validate.IsStartDateBeforeEndDate(startPicker, endPicker) == false)
            {
                await DisplayAlert("Invalid Dates", "Start date must be before End date", "Ok");
                return false;
            }

            if (Validate.DoesEntryContainText(nameEntry.Text) == false)
            {
                await DisplayAlert("Invalid Name", "Assessment must have a Name", "Ok");
                return false;
            }

            return true;
        }
    }
}