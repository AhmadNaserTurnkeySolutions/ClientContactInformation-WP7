using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

// Directive for the Data Model
using WP7LDBStorage.Model;


namespace WP7LDBStorage.ViewModel
{
    public class ViewModelClass : INotifyPropertyChanged
    {
        // Linq to SQL data context for the local database.
        private DataContextClass clientInfoDB;

        // Class constructor, create the data context object
        public ViewModelClass(string clientInfoDBConnectionString)
        {
            clientInfoDB = new DataContextClass(clientInfoDBConnectionString);
        }

        // All Client Information Items
        private ObservableCollection<Information> _allClientInfoItems;
        public ObservableCollection<Information> AllClientInfoItems
        {
            get { return _allClientInfoItems; }
            set
            {
                _allClientInfoItems = value;
                NotifyPropertyChanged("AllClientInfoItems");
            }
        }

        public void SaveChangesToDB()
        {
            clientInfoDB.SubmitChanges();
        }

        // Query database and load the collection and list used by the pivot pages
        public void LoadCollectionsFromDatabase()
        {
            // Specifiy the query for all Client Info Items in the database
            var ClientInfoInDB = from Information info in clientInfoDB.Clients
                                 select info;

            // Query the database and load all the Client Information Items
            AllClientInfoItems = new ObservableCollection<Information>(ClientInfoInDB);
        }

        // Add Client Information Item to the database and collection
        public void AddClientInfoItem(Information newClientInfoItem)
        {
            // Add a client info item to the data context.
            clientInfoDB.Clients.InsertOnSubmit(newClientInfoItem);

            // Save changes to the database
            clientInfoDB.SubmitChanges();

            // Add a client info item to the "all" observable collection.
            AllClientInfoItems.Add(newClientInfoItem);
        }

        // Remove a client info item from the database and collection
        public void DeleteClientInfoItem(Information clientInfoForDelete)
        {
            // Remove the client info item from the "all" observable collection.
            AllClientInfoItems.Remove(clientInfoForDelete);

            // Remove the client info item from the data context.
            clientInfoDB.Clients.DeleteOnSubmit(clientInfoForDelete);

            // Save changes to the database.
            clientInfoDB.SubmitChanges();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify Silverlight that a property has changed.
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
