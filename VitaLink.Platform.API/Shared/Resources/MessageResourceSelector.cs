using Microsoft.Extensions.Localization;

namespace VitaLink.Platform.API.Shared.Resources;

/// <summary>
///     Chooses the messages resources used to localize data annotation messages
///     (e.g. <c>[Required(ErrorMessage = "Key")]</c>).
/// </summary>
/// <remarks>
///     By convention each bounded context exposes one marker class named <c>*Messages</c> in its
///     <c>Resources</c> namespace (e.g. <c>IAM.Resources.IamMessages</c>). Keys are prefixed with their
///     context, so a key is looked up first in the messages of the context that owns the annotated type
///     and then in the rest of the messages (ASP.NET Core may pass the property type instead of the
///     resource type for positional records).
/// </remarks>
public static class MessageResourceSelector
{
    private const string Root = "VitaLink.Platform.API.";

    private static readonly Lazy<Type[]> AllMessageTypes = new(() => typeof(CommonMessages).Assembly.GetTypes()
        .Where(t => t.IsClass && t.Namespace is not null && t.Namespace.StartsWith(Root, StringComparison.Ordinal)
                    && t.Namespace.EndsWith(".Resources", StringComparison.Ordinal)
                    && t.Name.EndsWith("Messages", StringComparison.Ordinal))
        .ToArray());

    /// <summary>
    ///     Returns the messages resource type for the bounded context that owns <paramref name="modelType" />.
    /// </summary>
    /// <param name="modelType">The type that declares the annotated property.</param>
    /// <returns>The marker class of the context messages, or <see cref="CommonMessages" /> when there is none.</returns>
    public static Type For(Type modelType)
    {
        var ns = modelType.Namespace;
        if (ns is null || !ns.StartsWith(Root, StringComparison.Ordinal)) return typeof(CommonMessages);
        var resourcesNamespace = Root + ns[Root.Length..].Split('.')[0] + ".Resources";
        return AllMessageTypes.Value.FirstOrDefault(t => t.Namespace == resourcesNamespace) ?? typeof(CommonMessages);
    }

    /// <summary>
    ///     Creates a localizer that reads the context messages first and then the messages of the other contexts.
    /// </summary>
    /// <param name="modelType">The type passed by ASP.NET Core for the annotated member.</param>
    /// <param name="factory">The string localizer factory.</param>
    /// <returns>A localizer that resolves keys from every <c>*Messages</c> resource.</returns>
    public static IStringLocalizer Create(Type modelType, IStringLocalizerFactory factory)
    {
        var preferred = For(modelType);
        var ordered = new[] { preferred }.Concat(AllMessageTypes.Value.Where(t => t != preferred));
        return new FallbackStringLocalizer(ordered.Select(factory.Create).ToArray());
    }

    private sealed class FallbackStringLocalizer(IReadOnlyList<IStringLocalizer> localizers) : IStringLocalizer
    {
        public LocalizedString this[string name] => Find(localizer => localizer[name], name);

        public LocalizedString this[string name, params object[] arguments] =>
            Find(localizer => localizer[name, arguments], name);

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
            localizers.SelectMany(localizer => localizer.GetAllStrings(includeParentCultures));

        private LocalizedString Find(Func<IStringLocalizer, LocalizedString> read, string name)
        {
            foreach (var localizer in localizers)
            {
                var value = read(localizer);
                if (!value.ResourceNotFound) return value;
            }

            return new LocalizedString(name, name, resourceNotFound: true);
        }
    }
}
