using UnityEngine;
using UnityEngine.UI;

namespace VRUIP
{
    public class LoginController : A_Canvas
    {
        [Header("Components")]
        [SerializeField] private Image background;
        [SerializeField] private InputController usernameInputField;
        [SerializeField] private InputController passwordInputField;
        [SerializeField] private ToggleController showPasswordToggle;
        [SerializeField] private ButtonController loginButton;
        [SerializeField] private ButtonController signUpButton;
        [SerializeField] private ButtonController signUpNewUserButton;
        [SerializeField] private IconController backIcon;
        [SerializeField] private GameObject loginPage;
        [SerializeField] private GameObject signUpPage;

        private void Awake()
        {
            SetupLogin();
        }

        private void SetupLogin()
        {
            showPasswordToggle.RegisterOnToggleChanged(ToggleHidePassword);
            signUpButton.RegisterOnClick(() =>
            {
                loginPage.SetActive(false);
                signUpPage.SetActive(true);
            });
            signUpNewUserButton.RegisterOnClick(BackToLoginPage);
            backIcon.RegisterOnClick(BackToLoginPage);
            //loginButton.RegisterOnClick(Login); // UNCOMMENT WHEN YOU HAVE LOGIN FUNCTIONALITY DONE.
        }

        private void ToggleHidePassword(bool hide)
        {
            passwordInputField.ToggleTextHidden(hide);
        }

        /// <summary>
        /// Function for logging in.
        /// </summary>
        public void Login()
        {
            // Implement your login logic here.
            //var username = usernameInputField.Text;
            //var password = passwordInputField.Text;
        }

        /// <summary>
        /// Function for signing up.
        /// </summary>
        public void SignUp()
        {
            // Implement your sign up logic here.
        }

        private void BackToLoginPage()
        {
            loginPage.SetActive(true);
            signUpPage.SetActive(false);
        }
        
        protected override void SetColors(ColorTheme theme)
        {
            background.color = theme.primaryColor;
        }
    }
}
