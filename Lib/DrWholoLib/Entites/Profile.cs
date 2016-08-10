using GalaSoft.MvvmLight;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrWholoLib
{
    /// <summary>
    /// Represents a users profile
    /// </summary>
    public class Profile : ObservableObject
    {
        #region Member Variables
        private string displayName;
        private Stream photo;
        #endregion // Member Variables

        #region Public Properties
        /// <summary>
        /// Gets or sets the displayName of the <see cref="Profile"/>.
        /// </summary>
        /// <value>
        /// The displayName of the <c>Profile</c>.
        /// </value>
        public string DisplayName
        {
            get { return displayName; }
            set { Set(ref displayName, value); }
        }

        /// <summary>
        /// Gets or sets the photo of the <see cref="Profile"/>.
        /// </summary>
        /// <value>
        /// The photo of the <c>Profile</c>.
        /// </value>
        public Stream Photo
        {
            get { return photo; }
            set { Set(ref photo, value); }
        }
        #endregion // Public Properties
    }
}
