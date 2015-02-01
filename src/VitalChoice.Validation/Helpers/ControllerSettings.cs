using System;
using System.Reflection;
using System.Collections.Generic;
using VitalChoice.Validation.Exceptions;
using VitalChoice.Validation.Models.Interfaces;

namespace VitalChoice.Validation.Helpers
{
    public class ControllerSettings
    {
        public static ControllerSettings Create()
        {
            throw new NotImplementedException();
            //if (AuthenticationHelper.UserIDForAgency.HasValue) {
            //    return new ControllerSettings(AuthenticationHelper.UserIDForAgency.Value);
            //}
            //throw new NotAgencyUserException();
        }

        public ControllerSettings(Guid userId)
        {
            //_userId = userId;
            //_triedToLoadUser = false;
            _viewModes = new Dictionary<string, IMode>();
        }

        private readonly Dictionary<string, IMode> _viewModes;

        //private Guid _userId;
        //private UserTP _user;
        //private bool _triedToLoadUser;

        //public SettingTP UserSettings
        //{
        //    get { return User.UserSettings; }
        //}

        //public UserTP User
        //{
        //    get
        //    {
        //        if (_user == null && !_triedToLoadUser)
        //        {
        //            _user = UserManager.GetUserById(_userId);
        //            _triedToLoadUser = true;
        //        }
        //        return _user;
        //    }
        //}

        public Dictionary<string, IMode> ViewModes
        {
            get { return _viewModes; }
        }

        //private SettingTP GetSettings(Guid userId)
        //{
        //    var user = UserManager.GetUserById(userId);
        //    if (user != null) {
        //        return user.UserSettings;
        //    }
        //    return null;
        //}

        public T GetValidationMode<T>(string actionName) 
            where T: IMode
        {
            if (ViewModes.ContainsKey(actionName))
            {
                return (T) ViewModes[actionName];
            }
            else
            {
                return default(T);
            }
        }

        public void SetMode(Type viewModeType, object mode, string action)
        {
            if (!typeof(IMode).IsAssignableFrom(viewModeType))
                throw new ArgumentException("viewModeType should implement IMode");
            IMode existingMode;
            if (ViewModes.TryGetValue(action, out existingMode)) {
                existingMode.Mode = mode;
            }
            else {
                var viewMode = (IMode)Activator.CreateInstance(viewModeType);
                viewMode.Mode = mode;
                ViewModes.Add(action, viewMode);
            }
        }

        internal void Clear()
        {
            _viewModes.Clear();
        }
    }
}