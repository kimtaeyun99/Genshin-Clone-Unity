public static class ElementReaction
{
    public static ElementReactionType GetReaction(ElementType hostElement, ElementType guestElement)
    {
        if(hostElement == guestElement)
        {
            return ElementReactionType.None;
        }

        if((hostElement == ElementType.Pyro && guestElement == ElementType.Hydro) ||
            (hostElement == ElementType.Hydro && guestElement == ElementType.Pyro))
        {
            return ElementReactionType.Vaporize;
        }

        if((hostElement == ElementType.Pyro && guestElement == ElementType.Cryo) ||
            (hostElement == ElementType.Cryo && guestElement == ElementType.Pyro))
        {
            return ElementReactionType.Melt;
        }

        if((hostElement == ElementType.Pyro && guestElement == ElementType.Electro) ||
            (hostElement == ElementType.Electro && guestElement == ElementType.Pyro))
        {
            return ElementReactionType.Overloaded;
        }

        if((hostElement == ElementType.Hydro && guestElement == ElementType.Cryo) ||
            (hostElement == ElementType.Cryo && guestElement == ElementType.Hydro))
        {
            return ElementReactionType.Frozen;
        }

        if((hostElement == ElementType.Hydro && guestElement == ElementType.Electro) ||
            (hostElement == ElementType.Electro && guestElement == ElementType.Hydro))
        {
            return ElementReactionType.ElectroCharged;
        }

        return ElementReactionType.None;

    }
    public static float GetElementDamageMultiplier(ElementReactionType elementReactionType)
    {
        if(elementReactionType == ElementReactionType.None)
        {
            return 1f;
        }

        if(elementReactionType == ElementReactionType.Vaporize)
        {
            return 2f;
        }
        else
        {
            return 1f;
        }
    }
}
