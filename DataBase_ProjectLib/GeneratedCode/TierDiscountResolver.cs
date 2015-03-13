﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

public class TierDiscountResolver : IActionResolver<int>
{
    public KeyValuePair<IEquatable<int>, string> Params
    {
        get; set;
    }

	public virtual decimal Execute(object Context)
	{
	    var currentTier = Context.Order.ItemsCount;
        KeyValuePair<int, string> previousTier = null;
	    foreach (var pair in Params)
	    {
	        previousTier = pair;
            if (currentTier < pair.Key)
	        {
	            break;
	        }
	    }
	    return Container.Resolve<IAction>(previousTier.Value).Execute();
	}

}

