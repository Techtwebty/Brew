﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Brew.TypeConverters {

	public class Int32ArrayConverter : TypeConverter {

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(string)) {
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string) || sourceType == typeof(ArrayList)) {
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			var stringValue = value as String;

			if(stringValue != null) {
				if(stringValue.StartsWith("[")) {
					stringValue = stringValue.Substring(1);
				}

				if(stringValue.EndsWith("]")) {
					stringValue = stringValue.Substring(0, stringValue.Length - 1);
				}

				var parts = stringValue.Split(new[] { ',' });
				var array = parts.Select(s => int.Parse(s, CultureInfo.InvariantCulture)).ToArray<int>();

				return array;
			}
			// handle data coming back from json, automatically converted into an arraylist rather than a string.
			else {
				ArrayList list = value as ArrayList;

				if(list != null) {
					return list.ToArray(typeof(int));
				}
			}

			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string)) {
				var intValue = value as int[];
				return string.Join(",", intValue.Select(i => i.ToString(CultureInfo.InvariantCulture)));
			}

			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
