const { RPSLSStrategies, RPSLSChoices } = require('../settings/rpslsOptions');
const IterativeStrategy = require('../strategies/iterative.strategy');
const FixedStrategy = require('../strategies/fixed.strategy');
const RandomStrategy = require('../strategies/random.strategy');

class PickStrategyFactory {

    constructor() {
        this._defaultStrategy = 'random';
    }

    getStrategy() {
        switch (this._defaultStrategy) {
            case RPSLSStrategies.ROCK:
                return new FixedStrategy(RPSLSChoices[0]);
            case RPSLSStrategies.PAPER:
                return new FixedStrategy(RPSLSChoices[1]);
            case RPSLSStrategies.SCISSORS:
                return new FixedStrategy(RPSLSChoices[2]);
            case RPSLSStrategies.LIZARD:
                return new FixedStrategy(RPSLSChoices[3]);
            case RPSLSStrategies.SPOCK:
                return new FixedStrategy(RPSLSChoices[4]);
            case RPSLSStrategies.ITERATIVE:
                return new IterativeStrategy();
            case RPSLSStrategies.RANDOM:
                return new RandomStrategy();
        }
    }

    setDefaultStrategy(newDefaultStrategy) {
        if (newDefaultStrategy) {
            this._defaultStrategy = newDefaultStrategy.toLowerCase();
        }
    }
}

module.exports =
    PickStrategyFactory;