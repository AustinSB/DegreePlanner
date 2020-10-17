using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DegreePlanner.Models;
using Plugin.LocalNotifications;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DegreePlanner.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CourseEntryPage : ContentPage
    {
        //Value of this field is set when new CourseEntryPage is opened
        int selectedTermId;
        public int SelectedTermId
        {
            get { return selectedTermId; }
            set { selectedTermId = value; }
        }

        Course course;

        public CourseEntryPage()
        {
            InitializeComponent();
        }

        //On Page Load
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            course = (Course)BindingContext;
            
            notesView.ItemsSource = await App.Database.GetNotesAsync(course);
            assessmentView.ItemsSource = await App.Database.GetAssessmentsAsync(course);

            DeleteButtonEnable();
            PopulateStatusPicker();
            DisableAddNoteButton();
            DisableAddAssessmentButton();
            SetSwitchToggles();
            Validate.SetMinimumDate(startPicker, endPicker);
        }

        //Methods
        private void PopulateStatusPicker()
        {
            if (statusPicker.Items.Count <= 0) 
            {
                statusPicker.Items.Add("In Progress");
                statusPicker.Items.Add("Completed");
                statusPicker.Items.Add("Dropped");
                statusPicker.Items.Add("Plan To Take");
            }
            
            try
            {
                if (course.Status.Length <= 0)
                {
                    statusPicker.SelectedIndex = 0;
                }
                else
                {
                    statusPicker.SelectedItem = course.Status.ToString();
                }
            }
            catch (Exception)
            {
                statusPicker.SelectedIndex = 0;
            }

            
        }
        //If Course does not have an id yet (new course that is unsaved), disable the adding of assessments/notes
        private void DisableAddNoteButton()
        {
            if (course.Id <= 0)
            {
                addNotesButton.IsEnabled = false;
            }
            else
            {
                addNotesButton.IsEnabled = true;
            }
        }

        //If assessments already added, disable 'add' button
        async void DisableAddAssessmentButton()
        {
            if (course.Id <= 0)
            {
                addAssessButton.IsEnabled = false;
                return;
            }

            if (await App.Database.IsAssessmentTypeAdded(course, "Performance")
                && await App.Database.IsAssessmentTypeAdded(course, "Objective"))
            {
                addAssessButton.IsEnabled = false;
            }
            else
            {
                addAssessButton.IsEnabled = true;
            }
        }

        //Alerts
        private void SetSwitchToggles()
        {
            if (course.StartAlertSet) { startDateSwitch.IsToggled = true; }
            if (course.EndAlertSet) { endDateSwitch.IsToggled = true; }
        }

        //Create or cancel alerts based on switch's toggle and whether alerts are already set
        private void ShowCancelCourseAlerts()
        {
            //Start
            if (startDateSwitch.IsToggled == true && course.StartAlertSet == false)
            {
                CrossLocalNotifications.Current.Show("Course Starting", $"{course.Title} starts today", course.Id + 100, course.Start);
                course.StartAlertSet = true;
            }
            else if (startDateSwitch.IsToggled == false && course.StartAlertSet == true)
            {
                CrossLocalNotifications.Current.Cancel(course.Id + 100);
                course.StartAlertSet = false;
            }
            //End
            if (endDateSwitch.IsToggled == true && course.EndAlertSet == false)
            {
                CrossLocalNotifications.Current.Show("Course Ending", $"{course.Title} ends today", course.Id + 200, course.End);
                course.EndAlertSet = true;
            }
            else if (endDateSwitch.IsToggled == false && course.EndAlertSet == true)
            {
                CrossLocalNotifications.Current.Cancel(course.Id + 200);
                course.EndAlertSet = false;
            }
        }

        private void DeleteButtonEnable()
        {
            if (course.Id <= 0)
            {
                
                deleteCourseButton.IsEnabled = false;
            }
            else
            {
                deleteCourseButton.IsEnabled = true;
            }
        }

        //View Controls
        //Add Note, Navigate to Note Entry Page
        async void OnAddNoteClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NoteEntryPage
            {
                BindingContext = new Note(),
                SelectedCourseId = course.Id
            });
        }

        //Update Note, Navigate to Note Entry Page
        async void OnNotesViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if(e.SelectedItem != null)
            {
                await Navigation.PushAsync(new NoteEntryPage 
                { 
                    BindingContext = e.SelectedItem as Note,
                    SelectedCourseId = course.Id
                });
            }
        }

        //Assessment Controls
        async void OnAddAssessmentButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AssessmentEntryPage
            {
                BindingContext = new Assessment(),
                SelectedCourse = course
            });
        }

        async void OnAssessmentViewItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if(e.SelectedItem != null)
            {
                await Navigation.PushAsync(new AssessmentEntryPage
                {
                    BindingContext = e.SelectedItem as Assessment,
                    SelectedCourse = course
                });
            }
        }

        //Save
        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            if (await ValidateCourse() == false)
            {
                return;
            }
            
            course.TermId = selectedTermId;
            course.Title = titleEntry.Text;
            course.Start = startPicker.Date;
            course.End = endPicker.Date;
            course.Status = statusPicker.SelectedItem.ToString();
            course.InstructorName = nameEntry.Text;
            course.InstructorPhone = phoneEntry.Text;
            course.InstructorEmail = emailEntry.Text;

            ShowCancelCourseAlerts();

            await App.Database.SaveCourseAsync(course);
            await Navigation.PopAsync();
        }

        //Delete
        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (course.StartAlertSet == true)
            {
                CrossLocalNotifications.Current.Cancel(course.Id + 100); 
                course.StartAlertSet = false;
            }
            if (course.EndAlertSet == true)
            {
                CrossLocalNotifications.Current.Cancel(course.Id + 200); 
                course.EndAlertSet = false;
            }

            await App.Database.DeleteCourseAsync(course);
            await Navigation.PopAsync();
        }

        //Validate
        async Task<bool> ValidateCourse()
        {
            if (Validate.IsStartDateBeforeEndDate(startPicker, endPicker) == false)
            {
                await DisplayAlert("Invalid Dates", "Start date must be before End date", "Ok");
                return false;
            }

            if (Validate.DoesEntryContainText(titleEntry.Text) == false)
            {
                await DisplayAlert("Invalid Title","Title must have a value", "Ok");
                return false;
            }

            if (Validate.DoesEntryContainText(nameEntry.Text) == false)
            {
                await DisplayAlert("Invalid Name", "Must enter Instructor's name", "Ok");
                return false;
            }
            
            if (Validate.IsPhoneValid(phoneEntry.Text) == false)
            {
                await DisplayAlert("Invalid Phone", "Must enter 10-digit phone #", "Ok");
                return false;
            }

            if (Validate.IsEmailValid(emailEntry.Text) == false)
            {
                await DisplayAlert("Invalid Email", "Must enter valid email", "Ok");
                return false;
            }

            return true;
        }
    }
}