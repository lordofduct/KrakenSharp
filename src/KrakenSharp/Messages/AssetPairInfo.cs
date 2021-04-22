using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace KrakenSharp.Messages
{
    public class AssetPairInfo
	{

		[JsonProperty("altname")]
		public string AltName { get; set; }

		[JsonProperty("wsname")]
		public string WSName { get; set; }

		[JsonProperty("aclass_base")]
		public string AClass_Base { get; set; }

		[JsonProperty("base")]
		public string Base { get; set; }

		[JsonProperty("currency")]
		public string AClass_Quote { get; set; }

		[JsonProperty("quote")]
		public string Quote { get; set; }

		[JsonProperty("lot")]
		public string Lot { get; set; }

		[JsonProperty("pair_decimals")]
		public int MaxPricePrecision { get; set; }

		[JsonProperty("lot_decimals")]
		public int MaxVolumePrecision { get; set; }

		[JsonProperty("lot_multiplier")]
		public decimal LotMultiplier { get; set; }

		[JsonProperty("leverage_buy")]
		public decimal[] LeverageBuy { get; set; }

		[JsonProperty("leverage_sell")]
		public decimal[] LeverageSell { get; set; }

		[JsonProperty("fees")]
		public decimal[][] FeesTaker { get; set; }

		[JsonProperty("fees_maker")]
		public decimal[][] FeesMaker { get; set; }

		[JsonProperty("fee_volume_currency")]
		public string FeeVolumeCurrency { get; set; }

		[JsonProperty("margin_call")]
		public decimal MarginCall { get; set; }

		[JsonProperty("margin_stop")]
		public decimal MarginStop { get; set; }

		[JsonProperty("ordermin")]
		public string OrderMinRaw { get; set; } //it's a string in the json, not sure if that's because there are non-numeric results or not

		[JsonIgnore]
		public decimal OrderMin
        {
			get { return ConvertUtil.ToDecimal(OrderMinRaw); }
        }

    }
}
