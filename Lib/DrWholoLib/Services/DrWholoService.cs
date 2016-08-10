using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Security.Authentication.Web.Core;
using Windows.Security.Credentials;
using Windows.Storage;
using Microsoft.Graph;
using Microsoft.Identity.Client;

namespace DrWholoLib
{
    public class DrWholoService
    {
        #region Static Version
        #region Member Variables
        static private DrWholoService instance;
        #endregion // Member Variables

        #region Public Properties
        /// <summary>
        /// Gets or sets the singleton instance of the DrWholo Service
        /// </summary>
        static public DrWholoService Instance
        {
            get
            {
                if (instance == null)
                {
                    throw new InvalidOperationException($"{nameof(DrWholoService)}.{nameof(Instance)} has not been set.");
                }
                return instance;
            }
            set
            {
                instance = value;
            }
        }
        #endregion // Public Properties
        #endregion // Static Version


        #region Instance Version
        #region Constants
        static private string[] scopes = { "https://graph.microsoft.com/User.Read", "https://graph.microsoft.com/User.ReadBasic.All", "https://graph.microsoft.com/People.Read", "https://graph.microsoft.com/Mail.Send" }; //, "https://graph.microsoft.com/Directory.AccessAsUser.All" };
        #endregion // Constants

        #region Member Variables
        private bool allowInteractiveSignIn = true;
        private GraphServiceClient graphClient;
        private PublicClientApplication identityClientApp;
        private DateTimeOffset tokenExpiration;
        private string userToken = null;
        #endregion // Member Variables

        #region Constructors
        /// <summary>
        /// Initializes a new Dr. Wholo service.
        /// </summary>
        /// <param name="clientId">
        /// The Client ID used by the application to uniquely identify itself to the v2.0 authentication endpoint.
        /// </param>
        public DrWholoService(string clientId)
        {
            // Validate
            if (string.IsNullOrEmpty(clientId)) { throw new ArgumentNullException(nameof(clientId)); }

            // Create
            identityClientApp = new PublicClientApplication(clientId);
        }
        #endregion // Constructors

        #region Internal Methods
        /// <summary>
        /// Get an access token for the given context and resourceId. An attempt is first made to 
        /// acquire the token silently. If that fails, then we try to acquire the token by prompting the user. 
        /// </summary>
        private void EnsureGraphClient()
        {
            if (graphClient == null)
            {
                // Create Microsoft Graph client.
                try
                {
                    graphClient = new GraphServiceClient(
                        "https://graph.microsoft.com/beta", // "https://graph.microsoft.com/v1.0",
                        new DelegateAuthenticationProvider(
                            async (requestMessage) =>
                            {
                                var token = await GetTokenForUserAsync();
                                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);

                            }));
                }

                catch (Exception ex)
                {
                    graphClient = null;
                    Debug.WriteLine("Could not create a graph client: " + ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// Get Token for User.
        /// </summary>
        /// <returns>Token for user.</returns>
        private async Task<string> GetTokenForUserAsync()
        {
            if (userToken == null || tokenExpiration <= DateTimeOffset.UtcNow.AddMinutes(5))
            {
                AuthenticationResult authResult = null;
                try
                {
                    // Try silently
                    authResult = await identityClientApp.AcquireTokenSilentAsync(scopes);
                }
                catch
                {
                    // Try again with UI?
                    if (allowInteractiveSignIn)
                    {
                        authResult = await identityClientApp.AcquireTokenAsync(scopes);
                    }
                    else
                    {
                        throw;
                    }
                }

                // Store
                userToken = authResult.Token;
                tokenExpiration = authResult.ExpiresOn;
            }

            return userToken;
        }
        #endregion // Internal Methods

        #region Public Methods
        /// <summary>
        /// Signs the user into the service.
        /// </summary>
        /// <returns>
        /// A <see cref="Task"/> that represents the operation.
        /// </returns>
        /// <remarks>
        /// The service will first attempt to sign in silently using cached credentials. 
        /// If cached credentials are not available and <see cref="AllowInteractiveSignIn"/> 
        /// is set to <c>true</c>, the user will be prompted to sign in.
        /// </remarks>
        public async Task SignInAync()
        {
            EnsureGraphClient();
            await graphClient.Me.Request().GetAsync();
            if (SignedIn != null) { SignedIn(this, EventArgs.Empty); }
        }

        /// <summary>
        /// Signs the user out of the service.
        /// </summary>
        public void SignOut()
        {
            foreach (var user in identityClientApp.Users)
            {
                user.SignOut();
            }
            graphClient = null;
            userToken = null;
            if (SignedOut != null) { SignedOut(this, EventArgs.Empty); }
        }
        #endregion // Public Methods

        #region Public Properties
        /// <summary>
        /// Gets or sets a value that indicates if interactive sign-in is allowed.
        /// </summary>
        public bool AllowInteractiveSignIn
        {
            get
            {
                return allowInteractiveSignIn;
            }
            set
            {
                allowInteractiveSignIn = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="GraphServiceClient"/> managed by the service.
        /// </summary>
        public GraphServiceClient GraphClient => graphClient;
        #endregion // Public Properties

        #region Public Events
        /// <summary>
        /// Occurs when the user has signed in.
        /// </summary>
        public event EventHandler SignedIn;

        /// <summary>
        /// Occurs when the user has signed out.
        /// </summary>
        public event EventHandler SignedOut;
        #endregion // Public Events
        #endregion // Instance Version
    }
}
