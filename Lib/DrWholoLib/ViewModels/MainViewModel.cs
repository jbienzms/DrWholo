// using Adept;
using GalaSoft.MvvmLight;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrWholoLib
{
    public class MainViewModel : ViewModelBase
    {
        #region Member Variables
        private GraphServiceClient client;
        private Profile userGraph;
        #endregion // Member Variables

        #region Constructors
        /// <summary>
        /// Initialzies a new <see cref="MainViewModel"/>.
        /// </summary>
        public MainViewModel()
        {
            
        }
        #endregion // Constructors

        #region Internal Methods
        /// <summary>
        /// Creates a profile from a user object including loading the photo stream and direct reports.
        /// </summary>
        /// <param name="user">
        /// The user to load into a profile.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that yields the profile.
        /// </returns>
        private async Task<Profile> LoadProfile(User user)
        {
            var profile = new Profile()
            {
                DisplayName = user.DisplayName,
                Photo = await client.Users[user.Id].Photo.Content.Request().GetAsync(),
            };

            return profile;
        }
        #endregion // Internal Methods

        #region Public Methods
        /// <summary>
        /// Loads data from the service.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        public async Task LoadDataAsync()
        {
            client = DrWholoService.Instance.GraphClient;
            var user = await client.Me.Request().GetAsync();
            UserGraph = await LoadProfile(user);
        }
        #endregion // Public Methods

        #region Public Properties
        /// <summary>
        /// Gets or sets the user graph for the view.
        /// </summary>
        /// <value>
        /// The user graph.
        /// </value>
        public Profile UserGraph
        {
            get { return userGraph; }
            set { Set(ref userGraph, value); }
        }
        #endregion // Public Properties
    }
}
