using System;
using System.Collections.Generic;
using Templates.Helpers;

namespace VitalChoice.Domain.Workflow
{
    public class ActionItem: IEquatable<ActionItem>
    {
        public ActionItem(string actionType, string actionName)
        {
            ReflectionHelper.ResolveType(actionType);
            ActionName = actionName;
        }

        public ActionItem(Type actionType, string actionName)
        {
            ActionType = actionType;
            ActionName = actionName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == GetType() && Equals((ActionItem) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ActionType?.GetHashCode() ?? 0)*397) ^ (ActionName?.GetHashCode() ?? 0);
            }
        }

        public Type ActionType { get; }
        public string ActionName { get; }

        public HashSet<ActionItem> OptionalDependentTree { get; set; }

        public bool Equals(ActionItem other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (other == null)
                return false;
            return other.ActionName == ActionName && other.ActionType == ActionType;
        }

        public static bool operator ==(ActionItem one, ActionItem other)
        {
            if (ReferenceEquals(one, other))
                return true;
            if ((object) one == null)
                return false;
            return one.Equals(other);
        }

        public static bool operator !=(ActionItem one, ActionItem other)
        {
            if (ReferenceEquals(one, other))
                return false;
            if ((object)one == null)
                return true;
            return !one.Equals(other);
        }
    }
}