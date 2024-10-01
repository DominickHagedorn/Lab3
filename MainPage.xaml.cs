using Lab3;

namespace Lab3;
    public partial class MainPage : ContentPage
    {
        IBusinessLogic businessLogic = new BusinessLogic();

        public MainPage()
        {
            InitializeComponent();
            //load in all the airports from the database
            foreach (Airport airport in businessLogic.GetAirports())
            {
                AddAirportToGrid(airport);
            }
        }

        // Helper method to create and add the airport row to the grid
        private void AddAirportToGrid(Airport airport)
        {
            Button deleteButton = new Button()
            {
                ImageSource = "trashcan.png",
                Background = Brush.Transparent,
                Scale = 0.1,
                VerticalOptions = LayoutOptions.Center
            };
            deleteButton.Clicked += DeleteAirport_Click; // Attach functionality to button

            // Add the elements to the grid row
            int currRow = Airport_Grid.Count;
            Airport_Grid.Add(CreateLabel(airport.Id),                     0, currRow);
            Airport_Grid.Add(CreateLabel(airport.City),                   1, currRow);
            Airport_Grid.Add(CreateLabel(airport.DateVisited.ToString()), 2, currRow);
            Airport_Grid.Add(CreateLabel(airport.Rating.ToString()),      3, currRow);
            Airport_Grid.Add(deleteButton,                                4, currRow);
        }

        // Helper method to create a Label
        private Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                VerticalOptions = LayoutOptions.Center
            };
        }

        private void AddAirport_Click(object sender, EventArgs e)
        {
        DateTime date; // parsed date from entry
        int rating = 0; // parsed rating from entry
        AirportInputError errorCode = AirportInputError.NoError;
        if (ratingEntry.Text == null || ratingEntry.Text.Equals("") || !Int32.TryParse(ratingEntry.Text, out rating)) // makes sure rating is valid
        {
            errorCode = AirportInputError.RatingOutOfBounds;
        }
        else if (dateEntry.Text == null || dateEntry.Text.Equals("") || !DateTime.TryParse(dateEntry.Text, out date)) // makes sure date is valid
        {
            errorCode = AirportInputError.InvalidDate;
        }
        else
        {
            // adding airport to database
            errorCode = businessLogic.AddAirport(
                idEntry.Text, cityEntry.Text, date, rating);
        }

        if (errorCode == AirportInputError.NoError)
            {
                // Reuse the AddAirportToGrid method
                AddAirportToGrid(new Airport(idEntry.Text, cityEntry.Text, DateTime.Parse(dateEntry.Text), int.Parse(ratingEntry.Text)));
            }
            else
            {
                // Handle errors
                HandleError(errorCode);
            }
        }

    /*
     * Error handling method
     */
    private void HandleError(AirportInputError errorCode)
        {
            string errorMessage = errorCode switch
            {
                AirportInputError.DuplicateAirportId => "Airport Already Exists",
                AirportInputError.InvalidIDLength => "Id should be a length of 3-4 characters",
                AirportInputError.RatingOutOfBounds => "Rating should be 1 - 5",
                AirportInputError.InvalidCityLength => "City should be greater than 0 and less than 25 characters",
                AirportInputError.InvalidDate => "Date is Malformed",
                AirportInputError.DBAdditionError => "Airport Not Added to Database",
                _ => "Error: unable to add airport"
            };

            DisplayAlert("Error", errorMessage, "Cancel");
        }

        private void EditAirport_Click(object sender, EventArgs e)
        {
        int rating;
            DateTime date;
            AirportEditError errorCode = AirportEditError.NoError;
            if (ratingEntry.Text == null || ratingEntry.Text.Equals("") || !Int32.TryParse(ratingEntry.Text, out rating)) // makes sure the rating is valid
            {
                errorCode = AirportEditError.RatingOutOfBounds;
            }
            else if (dateEntry.Text == null || dateEntry.Text.Equals("") || !DateTime.TryParse(dateEntry.Text, out date)) // makes sure the date is valid
            {
                errorCode = AirportEditError.InvalidDate;
            }
            else
            {
                // sends the airport edit to the backend
                errorCode = businessLogic.EditAirport(idEntry.Text, cityEntry.Text, date, rating);
            }
            if (errorCode == AirportEditError.NoError) // if id was found
            {
                // edit the entries in the grid
                int row = Airport_Grid.GetRow(Airport_Grid.Children.FirstOrDefault(c => Airport_Grid.GetColumn(c) == 0 && ((Label)c).Text.Equals(idEntry.Text))); // gets the row of the entered ID
                ((Label)Airport_Grid.Children.FirstOrDefault(c => Airport_Grid.GetRow(c) == row && Airport_Grid.GetColumn(c) == 0)).Text = idEntry.Text; // find and edit the id
                ((Label)Airport_Grid.Children.FirstOrDefault(c => Airport_Grid.GetRow(c) == row && Airport_Grid.GetColumn(c) == 1)).Text = cityEntry.Text;// find and edit the city
                ((Label)Airport_Grid.Children.FirstOrDefault(c => Airport_Grid.GetRow(c) == row && Airport_Grid.GetColumn(c) == 2)).Text = dateEntry.Text; // find and edit the date
                ((Label)Airport_Grid.Children.FirstOrDefault(c => Airport_Grid.GetRow(c) == row && Airport_Grid.GetColumn(c) == 3)).Text = ratingEntry.Text; // find and edit the rating
            }
            else if (errorCode == AirportEditError.IdNotPresent)
            {
                DisplayAlert("Error", "Id not found in database", "cancel");
            }
            else
            {
                DisplayAlert("Error","Error: unable to edit airport","cancel");
            }
        }

        /*
         * deletes an airport row from the UI and calls the bussiness logic to do the same
         */
        private void DeleteAirport_Click(object sender, EventArgs e)
        {

            var row = Airport_Grid.GetRow((Button)sender); // gets the row to be deleted
            Label idLabel = ((Label)Airport_Grid.Children.FirstOrDefault(c => Airport_Grid.GetColumn(c) == 0 && Airport_Grid.GetRow(c) == row)); // gets the ID of the current row
            if (idLabel != null) { // if ID was found
                AirportDeletionError errorCode = businessLogic.DeleteAirport(idLabel.Text); // deletes the ID from the database
                if (errorCode == AirportDeletionError.NoError) // ensures deletion was successful
                {
                    for (int col = 0; col < Airport_Grid.ColumnDefinitions.Count; col++) // iterate through all columns in the grid
                    {
                        var child = Airport_Grid.Children.FirstOrDefault(c => Airport_Grid.GetRow(c) == row && Airport_Grid.GetColumn(c) == col); // finds element to delete
                        if (child != null) // if element found
                        {
                            Airport_Grid.Children.Remove(child); // delete

                        }
                    }
                }
                else if(errorCode == AirportDeletionError.AirportNotFound)
                {
                    DisplayAlert("Error", "Airport not present in database", "cancel");
                }
                else if (errorCode == AirportDeletionError.FailedToDeleteError)
                {
                    DisplayAlert("Error", "Failed to Delete Airport", "cancel");
                }
            }
            else
            {
                DisplayAlert("Error", "No ID Provided", "cancel");
            }
        }
        private void AirportStats_Click(object sender, EventArgs e)
        {
            airportStatsLabel.Text = businessLogic.CalculateStatistics();
        }
    }