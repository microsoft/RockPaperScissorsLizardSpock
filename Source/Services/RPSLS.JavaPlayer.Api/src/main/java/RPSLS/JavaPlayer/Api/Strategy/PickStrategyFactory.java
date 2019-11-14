package RPSLS.JavaPlayer.Api.Strategy;

import org.springframework.beans.factory.FactoryBean;
import RPSLS.JavaPlayer.Api.RPSLS;

public class PickStrategyFactory implements FactoryBean<IPickStrategy> {
    private final static String ROCK = "rock";
    private final static String PAPER = "paper";
    private final static String SCISSORS = "scissors";
    private final static String LIZARD = "lizard";
    private final static String SPOCK = "spock";
    private final static String RANDOM = "random";
    private final static String ITERATIVE = "iterative";

    private String defaultStrategy = RANDOM;

    @Override
    public IPickStrategy getObject() throws Exception {
        switch(defaultStrategy)
        {
            case ROCK:
                return new FixedPickStrategy(RPSLS.rock);
            case PAPER:
                return new FixedPickStrategy(RPSLS.paper);
            case SCISSORS:
                return new FixedPickStrategy(RPSLS.scissors);
            case LIZARD:
                return new FixedPickStrategy(RPSLS.lizard);
            case SPOCK:
                return new FixedPickStrategy(RPSLS.spock);
            case RANDOM:
                return new RandomPickStrategy();
            case ITERATIVE:
                return new IterativePickStrategy();
        }
        return null;
    }

    @Override
    public Class<?> getObjectType() {
        return IPickStrategy.class;
    }

    @Override
    public boolean isSingleton() {
        return true;
    }

	public void setDefaultStrategy(String pickStrategyChoice) {
        defaultStrategy = pickStrategyChoice.toLowerCase();
	}
}