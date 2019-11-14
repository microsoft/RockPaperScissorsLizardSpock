package RPSLS.JavaPlayer.Api.Strategy;

import RPSLS.JavaPlayer.Api.RPSLS;

public class FixedPickStrategy implements IPickStrategy {
    private final RPSLS pick;

    public FixedPickStrategy(RPSLS pick) {
        this.pick = pick;
    }

    public RPSLS getPick() {
        return pick;
    }
}
