using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Reflection;

namespace GRPick;

public class DynamicContractResolver : DefaultContractResolver {
	private readonly HashSet<string> _propertiesToSerialize;

	public DynamicContractResolver(IEnumerable<string> propertiesToSerialize) {
		_propertiesToSerialize = new HashSet<string>(propertiesToSerialize, StringComparer.OrdinalIgnoreCase);
	}

	protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) {
		var properties = base.CreateProperties(type, memberSerialization);
		return properties.Where(p => ShouldSerializeProperty(p)).ToList();
	}

	private bool ShouldSerializeProperty(JsonProperty property) {
		// Check if the property name is directly in the list
		if (_propertiesToSerialize.Contains(property.PropertyName)) {
			return true;
		}

		// Check for nested properties
		return _propertiesToSerialize.Any(s => IsNestedProperty(property.DeclaringType, s, property.PropertyName));
	}

	private bool IsNestedProperty(Type currentType, string propertyPath, string currentPropertyName) {
		// Split the property path by '.' to check each level
		var propertyNames = propertyPath.Split('.');

		// Traverse through the property names to check if they match
		for (int i = 0; i < propertyNames.Length; i++) {
			// Get the property info for the current name
			var propertyInfo = currentType.GetProperty(propertyNames[i], BindingFlags.Public | BindingFlags.Instance);
			if (propertyInfo == null) {
				return false; // Property not found
			}

			// If we are at the last property name, check if it matches
			if (i == propertyNames.Length - 1) {
				return propertyNames[i].Equals(currentPropertyName, StringComparison.OrdinalIgnoreCase);
			}

			// Move to the next nested property type
			currentType = propertyInfo.PropertyType;
		}

		return false; // If we exit the loop without returning, the path is invalid
	}
}