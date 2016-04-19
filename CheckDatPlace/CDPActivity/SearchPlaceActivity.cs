using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using CheckDatPlace.Adapter;
using CheckDatPlace.BLL;
using CheckDatPlace.DAL.DatabaseMigrationScript;
using CheckDatPlace.Model;
using CheckDatPlace.Model.StaticData;
using Newtonsoft.Json;

namespace CheckDatPlace
{
    [Activity(Label = "CDP", MainLauncher = true)]
    public class SearchPlaceActivity : Activity
    {
        #region pptes

        private ListView searchList;
        private Place[] filteredItems;
        private List<PlaceCategory> placeCategory;
        private Button addPlaceButton;
        private Spinner popupSpinner;
        private View popUpFilterWindow;
        private Spinner popUpFilterSpinner;
        private RatingBar popUpFilterRatingBar;

        private string lastQuery = string.Empty;

        #endregion pptes

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.InitApp();

            SetContentView(Resource.Layout.SearchPlaceLayout);
            popUpFilterWindow = LayoutInflater.Inflate(Resource.Layout.PopUpFilterLayout, null);

            #region component recovery

            searchList = FindViewById<ListView>(Resource.Id.SearchResultList);
            addPlaceButton = FindViewById<Button>(Resource.Id.addPlaceButton);

            popUpFilterSpinner = popUpFilterWindow.FindViewById<Spinner>(Resource.Id.popupFilterSpinner);
            popUpFilterRatingBar = popUpFilterWindow.FindViewById<RatingBar>(Resource.Id.popupFilterRating);

            #endregion component recovery

            #region events handle binding

            searchList.ItemClick += searchList_ItemClick;
            addPlaceButton.Click += addPlaceButton_Click;
            popUpFilterSpinner.ItemSelected += filterSpinner_ItemSelected;
            popUpFilterRatingBar.RatingBarChange += gradeFilter_RatingBarChange;

            #endregion events handle binding

            placeCategory = PlaceBLL.Instance.GetAllPlaceCategory();

            popUpFilterSpinner.Adapter = PlaceBLL.Instance.GetPlaceCategoryAdapter(this);

            LoadAllPlacesinList();
        }

        protected override void OnRestart()
        {
            base.OnRestart();
            //LoadAllPlacesinList();
            ExecuteSearch(lastQuery);
        }

        private void InitApp()
        {
            var isMigrationNeeded = DatabaseMigrator.CheckForMigration(this);
            if (isMigrationNeeded)
            {
                DatabaseMigrator.ApplyMigration(this);
            }
        }

        #region menu

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.searchMenu, menu);

            SearchManager searchManager = (SearchManager)GetSystemService(Context.SearchService);
            SearchView searchView = (SearchView)menu.FindItem(Resource.Id.search).ActionView;
            searchView.SetSearchableInfo(searchManager.GetSearchableInfo(ComponentName));
            searchView.SetQueryHint("Search places");
            searchView.QueryTextSubmit += searchView_QueryTextSubmit;
            searchView.QueryTextChange += searchView_QueryTextChange;

            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.menu_ShowAll:
                    var mapIntent = new Intent(this, typeof(MapViewActivity));
                    string[] adresses = PlaceBLL.Instance.ReadAllPlace().Select(p => p.Address).ToArray();
                    mapIntent.PutExtra("EditedAddress", adresses);
                    StartActivity(mapIntent);
                    break;

                case Resource.Id.popFilterButton:
                    ShowFilterPopUp();
                    break;

                default:
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        #endregion menu

        #region Component events

        private void addPlaceButton_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(CreatePlaceActivity));
        }

        private void showAllButton_Click()
        {
            var allPlaces = PlaceBLL.Instance.ReadAllPlace();
            var adresses = allPlaces.Select(p => p.Address).ToArray();
            var mapIntent = new Intent(this, typeof(MapViewActivity));
            mapIntent.PutExtra("EditedAddress", adresses);
            StartActivity(mapIntent);
        }

        private void searchList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var editActivity = new Intent(this, typeof(CreatePlaceActivity));
            var item = filteredItems.ElementAt(e.Position);
            var editedPlace = JsonConvert.SerializeObject(item);
            editActivity.PutExtra("EditedPlace", editedPlace);
            StartActivity(editActivity);
        }

        private void filterSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            ExecuteSearch(lastQuery);
        }

        private void gradeFilter_RatingBarChange(object sender, RatingBar.RatingBarChangeEventArgs e)
        {
            ExecuteSearch(lastQuery);
        }

        #endregion Component events

        #region search

        private void searchView_QueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.NewText))
            {
                ExecuteSearch(string.Empty);
            }
        }

        private void searchView_QueryTextSubmit(object sender, SearchView.QueryTextSubmitEventArgs e)
        {
            ExecuteSearch(e.Query);
        }

        private void ExecuteSearch(string query)
        {
            var minimalRate = popUpFilterRatingBar.Rating;
            var position = popUpFilterSpinner.SelectedItemPosition;
            if (position >= 0)
            {
                var categotyToFilter = placeCategory.ElementAt(position);

                filteredItems = PlaceBLL.Instance.FilterPlaces(minimalRate, categotyToFilter, query);
                searchList.Adapter = new PlaceAdapter(this, filteredItems);
                lastQuery = query;
            }
        }

        private void LoadAllPlacesinList()
        {
            filteredItems = PlaceBLL.Instance.ReadAllPlace().ToArray();
            searchList.Adapter = new PlaceAdapter(this, ApplicationContext, filteredItems);
        }

        private void ShowFilterPopUp()
        {
            PopupWindow window = new PopupWindow(popUpFilterWindow, 1000, 500, true);
            Button applyButton = popUpFilterWindow.FindViewById<Button>(Resource.Id.popupFilterApplyButton);
            applyButton.Click += (args0, arg1) => window.Dismiss();
            window.ShowAtLocation(popUpFilterWindow, GravityFlags.Center, 0, 0);
        }

        #endregion search
    }
}