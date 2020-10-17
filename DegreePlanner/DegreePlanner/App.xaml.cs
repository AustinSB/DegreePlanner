using DegreePlanner.Data;
using DegreePlanner.Views;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DegreePlanner
{
    public partial class App : Application
    {
        static DegreePlanDatabase database;
        public static DegreePlanDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new DegreePlanDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DegreePlan.db"));
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new TermsPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
