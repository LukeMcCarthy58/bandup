namespace BandUp.Domain.Profiles;

/// <summary>How experienced a musician is. Shown on profiles and used to match adverts.</summary>
public enum ExperienceLevel
{
    Beginner = 1,
    Intermediate = 2,
    Advanced = 3,
    Professional = 4,
}

/// <summary>How much time a musician wants to give a band.</summary>
public enum Commitment
{
    /// <summary>Jams and the odd gig.</summary>
    Casual = 1,

    /// <summary>Weekly rehearsals and regular gigs.</summary>
    Regular = 2,

    /// <summary>Touring, recording, aiming to go pro.</summary>
    Serious = 3,
}
