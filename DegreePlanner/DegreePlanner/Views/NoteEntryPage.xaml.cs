using DegreePlanner.Models;
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
    public partial class NoteEntryPage : ContentPage
    {
        int selectedCourseId;
        public int SelectedCourseId
        {
            get { return selectedCourseId; }
            set { selectedCourseId = value; }
        }

        Note note;

        public NoteEntryPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            note = (Note)BindingContext;

            DeleteButtonEnable();
        }

        private void DeleteButtonEnable()
        {
            if (note.Id <= 0)
            {

                deleteNoteButton.IsEnabled = false;
            }
            else
            {
                deleteNoteButton.IsEnabled = true;
            }
        }

        //View Controls
        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            if (await ValidateNote() == false)
            {
                return;
            }

            note.Content = contentEditor.Text;
            note.CourseId = selectedCourseId;

            await App.Database.SaveNoteAsync(note);
            await Navigation.PopAsync();
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            await App.Database.DeleteNoteAsync(note);
            await Navigation.PopAsync();
        }

        //Share the note
        async void OnShareNoteClicked(object sender, EventArgs e)
        {
            await App.Database.ShareNote(note.Content, "");
        }

        //Validate
        async Task<bool> ValidateNote()
        {
            if (Validate.DoesEntryContainText(contentEditor.Text) == false)
            {
                await DisplayAlert("Invalid Note", "Note cannot be empty", "Ok");
                return false;
            }

            return true;
        }
    }
}