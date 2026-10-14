public static class ElementReaction
{
    public static ElementReactionType GetReaction(ElementType hostElement, ElementType guestElement)
    {
        if (hostElement == guestElement)
            return ElementReactionType.None;

        if (IsPair(hostElement, guestElement, ElementType.Pyro, ElementType.Hydro))
            return ElementReactionType.Vaporize;

        if (IsPair(hostElement, guestElement, ElementType.Pyro, ElementType.Cryo))
            return ElementReactionType.Melt;

        if (IsPair(hostElement, guestElement, ElementType.Pyro, ElementType.Electro))
            return ElementReactionType.Overloaded;

        if (IsPair(hostElement, guestElement, ElementType.Hydro, ElementType.Cryo))
            return ElementReactionType.Frozen;

        if (IsPair(hostElement, guestElement, ElementType.Hydro, ElementType.Electro))
            return ElementReactionType.ElectroCharged;

        return ElementReactionType.None;
    }

    public static float GetElementDamageMultiplier(ElementReactionType elementReactionType)
    {
        return elementReactionType == ElementReactionType.Vaporize ? 2f : 1f;
    }

    private static bool IsPair(
        ElementType hostElement,
        ElementType guestElement,
        ElementType first,
        ElementType second)
    {
        return (hostElement == first && guestElement == second) ||
               (hostElement == second && guestElement == first);
    }
}
