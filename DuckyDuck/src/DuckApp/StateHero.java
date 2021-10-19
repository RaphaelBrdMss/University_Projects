package DuckApp;

public enum StateHero {
    RANDOM,     // Random walk = erre
    WALK,       // Walk toward objectif
    SWIM,       // In water but does'nt see fish yet
    SWIMHUNT,   // in water and heads toward fish
    HUNT,       // in Ground and heads towards food
    EATING,     // is Eating fish
    DEAD,       // Is dead
}
