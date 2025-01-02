using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using TMPro;
using Unity.VisualScripting;
using System.Threading.Tasks;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using Firebase.Firestore;

public class LoginRegistrationScript : MonoBehaviour
{
    [Header("Firebase")]
    public Firebase.DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;

    [Space]
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;

    [Space]
    [Header("Registration")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField confirmpassRegisterField;

    [Space]
    [Header("Loading")]
    public GameObject loadingbuffer;
    public MenuButtons menuButtons;
    private const string DisableTutorialKey = "DisableTutorial";

    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                Debug.Log("all is ok");
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private bool IsInternetAvailable()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    private async void InitializeFirebase()
    {
        await Task.Delay(500); // Add a small delay to ensure Firebase is ready
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }

            user = auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }

    public void Login()
    {
        loadingbuffer.SetActive(true);
        if (IsInternetAvailable())
        {
            loadingbuffer.SetActive(false);
            StartCoroutine(LoginAsync(emailLoginField.text, passwordLoginField.text));
        }
        else
        {
            loadingbuffer.SetActive(false);
            menuButtons.ShowWarningPopup("No internet connection. Please connect to proceed.");
        }
    }

    private IEnumerator LoginAsync(string email, string password)
    {
        loadingbuffer.SetActive(true);
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            //Debug.LogError(loginTask.Exception);
            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;

            string failedMessage = "Login Failed! Because ";

            switch(authError)
            {
                case AuthError.InvalidEmail:
                    failedMessage += "Email is invalid";
                    break;
                case AuthError.WrongPassword:
                    failedMessage += "Wrong password";
                    break;
                case AuthError.MissingEmail:
                    failedMessage += "Email is missing";
                    break;
                case AuthError.MissingPassword:
                    failedMessage += "Password is missing";
                    break;
                default:
                    failedMessage = "Login Failed";
                    break;
            }
            loadingbuffer.SetActive(false);
            menuButtons.ShowWarningPopup(failedMessage);
            Debug.Log(failedMessage);
        }
        else
        {
            loadingbuffer.SetActive(false);
            user = loginTask.Result.User;
            Debug.LogFormat($"{user.DisplayName} Your are Successfully Logged in");
            menuButtons.GoMainMenu();
        }
    }

    public void Logout()
    {
        if (auth != null && user != null)
        {
            auth.SignOut();
        }
        PlayerPrefs.SetInt(DisableTutorialKey, 0);
        PlayerPrefs.Save();
    }

    public void Register()
    {
        loadingbuffer.SetActive(true);
        if (IsInternetAvailable())
        {
            loadingbuffer.SetActive(false);
            StartCoroutine(RegisterAsync(usernameRegisterField.text, emailRegisterField.text, passwordRegisterField.text, confirmpassRegisterField.text));
        }
        else
        {
            loadingbuffer.SetActive(false);
            menuButtons.ShowWarningPopup("No internet connection. Please connect to proceed.");
        }
    }

    private IEnumerator RegisterAsync(string username, string email, string password, string confirmpass)
    {
        if (string.IsNullOrEmpty(username))
        {
            //Debug.LogError("Username is empty");
            menuButtons.ShowWarningPopup("Username is empty");
        }
        else if (string.IsNullOrEmpty(email))
        {
            //Debug.LogError("Email field is empty");
            menuButtons.ShowWarningPopup("Email field is empty");
        }
        else if (password != confirmpass)
        {
            //Debug.LogError("Password does not match");
            menuButtons.ShowWarningPopup("Password does not match");
        }
        else
        {
            loadingbuffer.SetActive(true);

            // Check if the username is already taken
            var usernameQuery = FirebaseFirestore.DefaultInstance
                .Collection("userAnalytics")
                .WhereEqualTo("username", username)
                .GetSnapshotAsync();

            yield return new WaitUntil(() => usernameQuery.IsCompleted);

            if (usernameQuery.Exception != null)
            {
                //Debug.LogError(usernameQuery.Exception);
                menuButtons.ShowWarningPopup("Error checking username availability");
                loadingbuffer.SetActive(false);
            }
            else if (usernameQuery.Result.Count > 0)
            {
                //Debug.LogError("Username is already taken");
                menuButtons.ShowWarningPopup("Username is already taken");
                loadingbuffer.SetActive(false);
            }
            else
            {
                // If username is not taken, proceed with registration
                var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
                yield return new WaitUntil(() => registerTask.IsCompleted);

                if (registerTask.Exception != null)
                {
                    FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
                    AuthError authError = (AuthError)firebaseException.ErrorCode;

                    string failedMessage = "Registration Failed! Because ";
                    switch (authError)
                    {
                        case AuthError.EmailAlreadyInUse:
                            failedMessage += "Email is already in use";
                            break;
                        case AuthError.InvalidEmail:
                            failedMessage += "Email is invalid";
                            break;
                        case AuthError.MissingEmail:
                            failedMessage += "Email is missing";
                            break;
                        case AuthError.MissingPassword:
                            failedMessage += "Password is missing";
                            break;
                        default:
                            failedMessage = "Registration Failed";
                            break;
                    }

                    loadingbuffer.SetActive(false);
                    menuButtons.ShowWarningPopup(failedMessage);
                    Debug.Log(failedMessage);
                }
                else
                {
                    user = registerTask.Result.User;

                    UserProfile userProfile = new UserProfile { DisplayName = username };
                    var updateProfileTask = user.UpdateUserProfileAsync(userProfile);

                    yield return new WaitUntil(() => updateProfileTask.IsCompleted);

                    if (updateProfileTask.Exception != null)
                    {
                        user.DeleteAsync();
                        //Debug.LogError(updateProfileTask.Exception);
                        menuButtons.ShowWarningPopup("Failed to update user profile");
                        loadingbuffer.SetActive(false);
                    }
                    else
                    {
                        loadingbuffer.SetActive(false);
                        Debug.Log($"Registration Successful! Welcome {user.DisplayName}");
                        CreateUserAnalyticsDocument(user.UserId, username);
                        menuButtons.GoMainMenu();
                    }
                }
            }
        }
    }

    private void CreateUserAnalyticsDocument(string userId, string username)
    {
        DocumentReference userAnalyticsRef = FirebaseFirestore.DefaultInstance
            .Collection("userAnalytics")
            .Document(userId);

        Dictionary<string, object> initialData = new Dictionary<string, object>
        {
            { "username", username },
            { "lastGameTimestamp", FieldValue.ServerTimestamp }
        };

        userAnalyticsRef.SetAsync(initialData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("User analytics document created successfully for user: " + userId);
            }
            else
            {
                Debug.LogError("Failed to create user analytics document: " + task.Exception);
            }
        });
    }
}
