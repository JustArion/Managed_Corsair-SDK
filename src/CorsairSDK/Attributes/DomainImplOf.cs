namespace Corsair.Attributes;

[AttributeUsage(AttributeTargets.Enum | AttributeTargets.Class)]
internal class DomainImplOfAttribute<T> : Attribute;
