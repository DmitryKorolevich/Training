using System;
using System.Reflection;
using System.Collections.Generic;
using VitalChoice.Validation.Models.Interfaces;

namespace VitalChoice.Validation.Helpers
{
    public class ControllerSettings
    {
        public static ControllerSettings Create()
        {
            return new ControllerSettings(Guid.Empty);
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

        //private SettingTP GetSettings(Guid userId)
        //{
        //    var user = UserManager.GetUserById(userId);
        //    if (user != null) {
        //        return user.UserSettings;
        //    }
        //    return null;
        //}

        public IMode ValidationMode { get; private set; }

        public void SetMode(Type viewModeType, object mode, string action)
        {
            if (!typeof(IMode).IsAssignableFrom(viewModeType))
                throw new ArgumentException("viewModeType should implement IMode");

            var viewMode = (IMode)Activator.CreateInstance(viewModeType);
            viewMode.Mode = mode;
            ValidationMode = viewMode;
        }

        internal void Clear()
        {
            _viewModes.Clear();
        }
    }
}