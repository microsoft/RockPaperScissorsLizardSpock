package RPSLS.JavaPlayer.Api.Strategy;

import RPSLS.JavaPlayer.Api.RPSLS;

public class IterativePickStrategy implements IPickStrategy {
    private int nextPick = 0;

    public RPSLS getPick() {
        RPSLS[] values = RPSLS.values();
        RPSLS pick = values[nextPick++];
        nextPick = nextPick % values.length;
        return pick;
    }
}