﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class OptionValue
{
	public virtual int idType
	{
		get;
		set;
	}

	public virtual string Value
	{
		get;
		set;
	}

	public virtual int idLookup
	{
		get;
		set;
	}

	public virtual int idProduct
	{
		get;
		set;
	}

	public virtual int id
	{
		get;
		set;
	}

	public virtual int idSku
	{
		get;
		set;
	}

	public virtual Product Product
	{
		get;
		set;
	}

	public virtual IEnumerable<LookupValue> LookupValue
	{
		get;
		set;
	}

	public virtual OptionType OptionType
	{
		get;
		set;
	}

	public virtual Lookup Lookup
	{
		get;
		set;
	}

}

