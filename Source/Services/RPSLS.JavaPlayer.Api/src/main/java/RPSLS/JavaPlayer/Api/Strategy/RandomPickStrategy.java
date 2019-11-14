package RPSLS.JavaPlayer.Api.Strategy;

import java.util.Random;

import RPSLS.JavaPlayer.Api.RPSLS;

public class RandomPickStrategy implements IPickStrategy {
    public RPSLS getPick() {
        Random random = new Random();
        RPSLS[] values = RPSLS.values();
		return values[random.nextInt(values.length)];
    }
}