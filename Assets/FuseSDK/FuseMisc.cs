﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace FuseMisc
{
	public static class Constants
	{
		/// <summary>Use with <c>false.ToString()</c> value to disable pre rewarded ad alert.</summary>
		public static readonly string RewardedAdOptionKey_ShowPreRoll = "FuseRewardedAdOptionKey_ShowPreRoll";

		/// <summary>Use with <c>false.ToString()</c> value to disable post rewarded ad alert.</summary>
		public static readonly string RewardedAdOptionKey_ShowPostRoll = "FuseRewardedAdOptionKey_ShowPostRoll";

		/// <summary>Use to specify the pre rewarded ad alert accept text.</summary>
		public static readonly string RewardedOptionKey_PreRollYesButtonText = "FuseRewardedOptionKey_PreRollYesButtonText";

		/// <summary>Use to specify the pre rewarded ad alert decline text.</summary>
		public static readonly string RewardedOptionKey_PreRollNoButtonText = "FuseRewardedOptionKey_PreRollNoButtonText";

		/// <summary>Use to specify the post rewarded ad alert confirmation button text.</summary>
		public static readonly string RewardedOptionKey_PostRollContinueButtonText = "FuseRewardedOptionKey_PostRollContinueButtonText";
	}

	/// <summary>Representation of another player that is contained in the user's friend list.</summary>
	public struct Friend
	{
		/// <summary>A unique id generated by the Fuse system.</summary>
		public string FuseId;

		/// <summary>Id that the user used to log in.</summary>
		public string AccountId;

		/// <summary>Alias that the user used to log in.</summary>
		public string Alias;
		public bool Pending;

#if UNITY_EDITOR
		public static implicit operator Friend(FuseSDKDotNETLite.Util.Friend f)
		{
			return new Friend() { FuseId = f.FuseId, AccountId = f.AccountId, Alias = f.Alias, Pending = f.Pending };
		}

		public static implicit operator FuseSDKDotNETLite.Util.Friend(Friend f)
		{
			return new FuseSDKDotNETLite.Util.Friend() { FuseId = f.FuseId, AccountId = f.AccountId, Alias = f.Alias, Pending = f.Pending };
		}
#endif

		public override string ToString()
		{
			return string.Join("\0", new string[] { FuseId, AccountId, Alias, Pending?"1":"0" });
		}
	}

	/// <summary>Representation of an item that the user is able to purchase from the App Store. IOS ONLY.</summary>
	public struct Product
	{
		/// <summary>The ID of the product used by the App Store.</summary>
		public string ProductId;

		/// <summary>The price of the product.</summary>
		public float Price;

		/// <summary>The currency that the price is in.</summary>
		public string PriceLocale;

#if UNITY_EDITOR
		public static implicit operator Product(FuseSDKDotNETLite.Util.Product p)
		{
			return new Product() { ProductId = p.ProductId, Price = p.Price, PriceLocale = p.PriceLocale };
		}

		public static implicit operator FuseSDKDotNETLite.Util.Product(Product p)
		{
			return new FuseSDKDotNETLite.Util.Product() { ProductId = p.ProductId, Price = p.Price, PriceLocale = p.PriceLocale };
		}
#endif

		public override string ToString()
		{
			return string.Join("\0", new string[] { ProductId, Price.ToString(), PriceLocale });
		}
	}

	/// <summary>Representation of the reward that a player will recieve for watching a rewarded video.</summary>
	public struct RewardedInfo
	{
		/// <summary>The message displayed before a video is shown, typicaly asking if the user would like to watch a video.</summary>
		public string PreRollMessage;

		/// <summary>The message displayed after a video is shown, typicaly confirming the reward the user got.</summary>
		public string RewardMessage;

		/// <summary>The item the user will get as a reward.</summary>
		public string RewardItem;

		/// <summary>The amount the user will get as a reward, used when the item is a currency.</summary>
		public int RewardAmount;

#if UNITY_EDITOR
		public static implicit operator RewardedInfo(FuseSDKDotNETLite.Util.RewardedInfo r)
		{
			return new RewardedInfo() { PreRollMessage = r.PreRollMessage, RewardMessage = r.RewardMessage, RewardAmount = r.RewardAmount, RewardItem = r.RewardItem };
		}

		public static implicit operator FuseSDKDotNETLite.Util.RewardedInfo(RewardedInfo r)
		{
			return new FuseSDKDotNETLite.Util.RewardedInfo() { PreRollMessage = r.PreRollMessage, RewardMessage = r.RewardMessage, RewardAmount = r.RewardAmount, RewardItem = r.RewardItem };
		}
#endif

		public override string ToString()
		{
			return string.Join("\0", new string[] { PreRollMessage, RewardMessage, RewardItem, RewardAmount.ToString() });
		}
	}

	/// <summary>Representation of an In-App Purchase Offer that can be presented to a player.</summary>
	public struct IAPOfferInfo
	{
		/// <summary>The id of the product the user will be asked to purchase.</summary>
		public string ProductId;

		/// <summary>The real currency price of the IAP.</summary>
		public float ProductPrice;

		/// <summary>The item the user will get from the IAP.</summary>
		public string ItemName;

		/// <summary>The amount the user will get from the IAP, used when the item is a currency.</summary>
		public int ItemAmount;

#if UNITY_EDITOR
		public static implicit operator IAPOfferInfo(FuseSDKDotNETLite.Util.IAPOfferInfo o)
		{
			return new IAPOfferInfo() { ProductId = o.ProductId, ProductPrice = o.ProductPrice, ItemName = o.ItemName, ItemAmount = o.ItemAmount };
		}

		public static implicit operator FuseSDKDotNETLite.Util.IAPOfferInfo(IAPOfferInfo o)
		{
			return new FuseSDKDotNETLite.Util.IAPOfferInfo() { ProductId = o.ProductId, ProductPrice = o.ProductPrice, ItemName = o.ItemName, ItemAmount = o.ItemAmount };
		}
#endif

		public override string ToString()
		{
			return string.Join("\0", new string[] { ProductId, ProductPrice.ToString(), ItemName, ItemAmount.ToString() });
		}
	}

	/// <summary>Representation of a Virtual Good Offer that can be presented to a player.</summary>
	public struct VGOfferInfo
	{
		/// <summary>The currency that will be spent to get the item.</summary>
		public string PurchaseCurrency;

		/// <summary>The amount of currency that will be spent to get the item.</summary>
		public float PurchasePrice;

		/// <summary>The item the user will receive from the offer.</summary>
		public string ItemName;

		/// <summary>The amount the user will get from the offer, used when the item is a currency.</summary>
		public int ItemAmount;

#if UNITY_EDITOR
		public static implicit operator VGOfferInfo(FuseSDKDotNETLite.Util.VGOfferInfo o)
		{
			return new VGOfferInfo() { PurchaseCurrency = o.PurchaseCurrency, PurchasePrice = o.PurchasePrice, ItemName = o.ItemName, ItemAmount = o.ItemAmount };
		}

		public static implicit operator FuseSDKDotNETLite.Util.VGOfferInfo(VGOfferInfo o)
		{
			return new FuseSDKDotNETLite.Util.VGOfferInfo() { PurchaseCurrency = o.PurchaseCurrency, PurchasePrice = o.PurchasePrice, ItemName = o.ItemName, ItemAmount = o.ItemAmount };
		}
#endif

		public override string ToString()
		{
			return string.Join("\0", new string[] { PurchaseCurrency, PurchasePrice.ToString(), ItemName, ItemAmount.ToString() });
		}
	}

	/// <summary>Error codes returned by the Fuse SDK.</summary>
	public enum FuseError
	{
		/// No error has occurred.
		NONE = 0,

		/// The user is not connected to the internet.
		NOT_CONNECTED,

		/// There was an error in establishing a connection with the server.
		REQUEST_FAILED,

		/// Data was received, but there was a problem parsing the xml
		SERVER_ERROR,

		/// The server has indicated the data it received was not valid.
		BAD_DATA,

		/// The session has recieved an error and the operation did not complete due to this error.
		SESSION_FAILURE,

		/// The request was not valid, and no action will be performed.
		INVALID_REQUEST,

		/// Unknown error
		UNDEFINED,
	}

	/// <summary>The type of transaction being recorded.</summary>
	public enum IAPState
	{
#if UNITY_IOS
		PURCHASING, PURCHASED, FAILED, RESTORED,	// IOS Specific
#elif UNITY_ANDROID
		PURCHASED, CANCELED, REFUNDED,				// Android Specific
#endif
	}

	/// <summary>The user's gender.</summary>
	public enum Gender
	{
		UNKNOWN,
		MALE,
		FEMALE,
		UNDECIDED,
		WITHHELD,
	}

	/// <summary>Type of account the player signed in with.</summary>
	public enum AccountType
	{
		NONE = 0,
		GAMECENTER = 1,
		FACEBOOK = 2,
		TWITTER = 3,
		OPENFEINT = 4,
		USER = 5,
		EMAIL = 6,
		DEVICE_ID = 7,
		GOOGLE_PLAY = 8,
	}

	/// <summary>Helpful extension functions.</summary>
	public static class FuseExtensions
	{
		public static long ToUnixTimestamp(this DateTime dateTime)
		{
			return (long)(dateTime - unixEpoch).TotalSeconds;
		}

		public static DateTime ToDateTime(this long timestamp)
		{
			return unixEpoch.AddSeconds(timestamp);
		}

		private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	}

#region Doxygen

	/*! \mainpage 
	 * \ref FuseSDK
	 * 
	 * \ref FuseMisc
	 */
#endregion
}