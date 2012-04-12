using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace WP7LDBStorage.Model
{
    public class DataContextClass : DataContext
    {
        // Pass the connection string to the base class.
        public DataContextClass(string connectionString)
            : base(connectionString)
        { }

        // Specify a table for the items.
        public Table<Information> Clients;
    }

    [Table]
    public class Information : INotifyPropertyChanged, INotifyPropertyChanging
    {
        // Define ID: Private field, public property, and database column
        private int _informationID;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int InformationID
        {
            get { return _informationID; }
            set
            {
                if (_informationID != value)
                {
                    NotifyPropertyChanging("InformationID");
                    _informationID = value;
                    NotifyPropertyChanged("InformationID");
                }
            }
        }

        // Define item name: private field, public property, and database column
        private string _firstName;

        [Column]
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                if (_firstName != value)
                {
                    NotifyPropertyChanging("FirstName");
                    _firstName = value;
                    NotifyPropertyChanged("FirstName");
                }
            }
        }

        // Define item name: private field, public property, and database column
        private string _middleName;

        [Column]
        public string MiddleName
        {
            get { return _middleName; }
            set
            {
                if (_middleName != value)
                {
                    NotifyPropertyChanging("MiddleName");
                    _middleName = value;
                    NotifyPropertyChanged("MiddleName");
                }
            }
        }

        // Define item name: private field, public property, and database column
        private string _lastName;

        [Column]
        public string LastName
        {
            get { return _lastName; }
            set
            {
                if (_lastName != value)
                {
                    NotifyPropertyChanging("LastName");
                    _lastName = value;
                    NotifyPropertyChanged("LastName");
                }
            }
        }

        // Define item name: private field, public property, and database column
        private string _address1;

        [Column]
        public string Address1
        {
            get { return _address1; }
            set
            {
                if (_address1 != value)
                {
                    NotifyPropertyChanging("Address1");
                    _address1 = value;
                    NotifyPropertyChanged("Address1");
                }
            }
        }

        // Define item name: private field, public property, and database column
        private string _address2;

        [Column]
        public string Address2
        {
            get { return _address2; }
            set
            {
                if (_address2 != value)
                {
                    NotifyPropertyChanging("Address2");
                    _address2 = value;
                    NotifyPropertyChanged("Address2");
                }
            }
        }

        // Define item name: private field, public property, and database column
        private string _city;

        [Column]
        public string City
        {
            get { return _city; }
            set
            {
                if (_city != value)
                {
                    NotifyPropertyChanging("City");
                    _city = value;
                    NotifyPropertyChanged("City");
                }
            }
        }

        // Define item name: private field, public property, and database column
        private string _province;

        [Column]
        public string Province
        {
            get { return _province; }
            set
            {
                if (_province != value)
                {
                    NotifyPropertyChanging("Province");
                    _province = value;
                    NotifyPropertyChanged("Province");
                }
            }
        }

        // Define item name: private field, public property, and database column
        private string _postalCode;

        [Column]
        public string PostalCode
        {
            get { return _postalCode; }
            set
            {
                if (_postalCode != value)
                {
                    NotifyPropertyChanging("PostalCode");
                    _postalCode = value;
                    NotifyPropertyChanged("PostalCode");
                }
            }
        }

        // Define item name: private field, public property, and database column
        private string _country;

        [Column]
        public string Country
        {
            get { return _country; }
            set
            {
                if (_country != value)
                {
                    NotifyPropertyChanging("Country");
                    _country = value;
                    NotifyPropertyChanged("Country");
                }
            }
        }

        // Define item name: private field, public property, and database column
        private string _phone;

        [Column]
        public string Phone
        {
            get { return _phone; }
            set
            {
                if (_phone != value)
                {
                    NotifyPropertyChanging("Phone");
                    _phone = value;
                    NotifyPropertyChanged("Phone");
                }
            }
        }

        // Define item name: private field, public property, and database column
        private string _email;

        [Column]
        public string Email
        {
            get { return _email; }
            set
            {
                if (_email != value)
                {
                    NotifyPropertyChanging("Email");
                    _email = value;
                    NotifyPropertyChanged("Email");
                }
            }
        }

        // Version column aids update performance.
        [Column(IsVersion = true)]
        private Binary _version;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify that a property is about to change
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }
}
