using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using System.Net;
using EBZShared.Models;

namespace EBZClient.Scripts
{
    /// <summary>
    /// This is a class which handles custom validation of a username.
    /// It is used to show a validation error within an <EditForm> whenever
    /// a username tries to register an already existing User.
    /// 
    /// However due to switching to MudBlazor mid development this class could be removed,
    /// as there's an easier way to do this with MudForms.
    /// This still works though so I decided to leave it here for future reference.
    /// </summary>
    public class UserDataValidator : ComponentBase
    {
        [CascadingParameter]
        public EditContext? EditContext { get; set; }


        private ValidationMessageStore? _messageStore;

        #region Methods

        /// <summary>
        /// Initializes the message store for handling validation messages
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (EditContext is null)
                throw new ArgumentNullException("Component used outside an edit context!");

            _messageStore = new ValidationMessageStore(EditContext);
        }

        /// <summary>
        /// Validates if a user exists in the DB or not
        /// </summary>
        /// <param name="response"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Validate(HttpStatusCode response, object model)
        {
            bool isValid = true;
            FieldIdentifier fieldIdentifier = new FieldIdentifier(model, nameof(User.Username));

            _messageStore!.Clear();

            if (response == HttpStatusCode.BadRequest)
            {
                isValid = false;
                _messageStore.Add(fieldIdentifier, "This user already exists!");
            }
                
            EditContext!.NotifyValidationStateChanged();
            return isValid;
        }

        /// <summary>
        /// Clears validation messages of this validator
        /// </summary>
        /// <param name="model"></param>
        public void Clear(object model)
        {
            _messageStore!.Clear();
            EditContext!.NotifyValidationStateChanged();
        }

        #endregion
    }
}
